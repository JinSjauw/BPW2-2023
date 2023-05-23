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
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitDead(object _sender, EventArgs e)
    {
        Unit unit = _sender as Unit;

        if (unit.IsEnemy())
        {
            enemies.Remove(unit);
        }
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        
        if(enemies.Count <= 0)
        {
            //Not in combat
            TurnSystem.Instance.NextTurn();
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
                BehaviourNode.BehaviourState enemyBehaviourState = currentEnemy.RunTree();
                if (enemyBehaviourState == BehaviourNode.BehaviourState.Success || enemyBehaviourState == BehaviourNode.BehaviourState.Failure)
                {
                    if (enemies.Count - 1 > enemyIndex)
                    {
                        if (currentEnemy.GetActionPoints() <= 0)
                        {
                            enemyIndex++;
                        }
                        SetStateTakingTurn();
                    }
                    else if(enemyIndex > enemies.Count - 1)
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

    private void SetStateTakingTurn()
    {
        timer = 1.5f;
        turnState = TurnState.TakingTurn;
        currentEnemy.SetTreeState(BehaviourNode.BehaviourState.Running);
    }
    
    private void TurnSystem_OnTurnChanged(object _sender, EventArgs _e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            foreach (var enemy in enemies)
            {
                enemy.SetTreeState(BehaviourNode.BehaviourState.Running);
            }

            enemyIndex = 0;
            turnState = TurnState.TakingTurn;
            timer = 1.0f;
        }
    }
}
