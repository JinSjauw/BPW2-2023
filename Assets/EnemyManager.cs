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
        Executing
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
                    state = State.Executing;
                    TakeEnemyAction(SetStateTakingTurn);
                    TurnSystem.Instance.NextTurn();
                }
                break;
            case State.Executing:
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

    private void TakeEnemyAction(Action _OnEnemyAcionComplete)
    {
        
    }
}
