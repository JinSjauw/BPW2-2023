using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private enum TurnState
    {
        WaitingForTurn,
        TakingTurn,
        Busy
    }

    private TurnState turnState;
    private float timer;
    
    [SerializeField] private List<Unit> enemies;
    private Unit currentEnemy;
    private int enemyIndex = 0;

    private void Awake()
    {
        turnState = TurnState.WaitingForTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        currentEnemy = enemies[enemyIndex];
        
        switch (turnState)
        {
            case TurnState.WaitingForTurn:
                break;
            case TurnState.TakingTurn:
                //Timer
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    turnState = TurnState.Busy;
                }
                break;
            case TurnState.Busy:
                //Run BTree
                BehaviourNode.State enemyState = currentEnemy.RunTree();
                if (enemyState == BehaviourNode.State.Success || enemyState == BehaviourNode.State.Failure)
                {
                    if (enemies.Count - 1 > enemyIndex)
                    {
                        enemyIndex++;
                        SetStateTakingTurn();
                    }
                    else if(enemyIndex >= enemies.Count - 1)
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                    else
                    {
                        SetStateTakingTurn();
                    }
                }
                break;
        }
    }
    
    //Loop through all the enemies in Enemy List
    //Go to the next after current enemy ends turn
    //End enemy turns

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        turnState = TurnState.TakingTurn;
    }
    
    private void TurnSystem_OnTurnChanged(object _sender, EventArgs _e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            foreach (var enemy in enemies)
            {
                enemy.SetTreeState(BehaviourNode.State.Running);
            }

            enemyIndex = 0;
            turnState = TurnState.TakingTurn;
            timer = 1.0f;
        }
    }
}
