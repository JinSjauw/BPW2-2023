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
    private int enemyIndex = 0;

    private void Awake()
    {
        turnState = TurnState.WaitingForTurn;
        //enemies = new List<Unit>();
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
                BehaviourNode.State enemyState = enemies[enemyIndex].RunTree();
                if (enemyState == BehaviourNode.State.Success || enemyState == BehaviourNode.State.Failure)
                {
                    if (enemies.Count - 1 > enemyIndex)
                    {
                        enemyIndex++;
                        SetStateTakingTurn();
                    }
                    else
                    {
                        TurnSystem.Instance.NextTurn();
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
            turnState = TurnState.TakingTurn;
            timer = 1.0f;
        }
    }
}
