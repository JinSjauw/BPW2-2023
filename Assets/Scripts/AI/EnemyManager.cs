using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private enum State
    {
        WaitingForTurn,
        TakingTurn,
        Busy
    }

    private State state;
    private float timer;
    
    private void Awake()
    {
        state = State.WaitingForTurn;
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

        switch (state)
        {
            case State.WaitingForTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = State.Busy;
                    if (TryTakeEnemyAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }
    
    private void TurnSystem_OnTurnChanged(object _sender, EventArgs _e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 1.5f;
        }
    }

    private bool TryTakeEnemyAction(Action _OnEnemyAcionComplete)
    {
        foreach (Unit enemy in UnitManager.Instance.GetEnemiesList())
        {
            TryTakeEnemyAction(enemy, _OnEnemyAcionComplete);
            return true;
        }

        return false;
    }

    private bool TryTakeEnemyAction(Unit _enemy, Action _OnEnemyAcionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestAction = null;
        
        foreach (BaseAction action in _enemy.GetActionArray())
        {
            if (!_enemy.CanTakeAction(action))
            {
                continue;
            }

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = action.GetBestAIAction();
                bestAction = action;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = action.GetBestAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestAction = action;
                }
            }
        }

        if (bestEnemyAIAction != null && _enemy.TryTakeAction(bestAction))
        {
            bestAction.TakeAction(bestEnemyAIAction.gridPosition, _OnEnemyAcionComplete);
        }
        else
        {
            return false;
        }
        
        return false;
    }
    
}
