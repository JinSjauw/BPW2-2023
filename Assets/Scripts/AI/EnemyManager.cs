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

    public static event EventHandler OnCombatStart;
    public static event EventHandler OnCombatEnd;
    
    private TurnState turnState;
    private float timer;
    
    [SerializeField] private List<Unit> enemies;
    private Unit currentEnemy;
    private int enemyIndex = 0;
    private bool isActive = false;

    private void Awake()
    {
        turnState = TurnState.WaitingForTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        Unit.OnAnyUnitAlert += Unit_OnAnyUnitAlert;
    }

    private void Unit_OnAnyUnitAlert(object _sender, EventArgs e)
    {
        Unit unit = _sender as Unit;
        
        if (unit.IsEnemy())
        {
            enemies.Add(unit);
            unit.SetState(Unit.UnitState.COMBAT);
        }

        if (!isActive)
        {
            isActive = true;
            OnCombatStart?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Unit_OnAnyUnitDead(object _sender, EventArgs e)
    {
        Unit unit = _sender as Unit;

        if (unit.IsEnemy())
        {
            enemies.Remove(unit);
        }
        
        if(enemies.Count <= 0)
        {
            //Not in combat
            if (!TurnSystem.Instance.IsPlayerTurn())
            {
                TurnSystem.Instance.NextTurn();
            }

            isActive = false;
            OnCombatEnd?.Invoke(this,EventArgs.Empty);
        }
        
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        if(enemyIndex > enemies.Count - 1)
        {
            TurnSystem.Instance.NextTurn();
        }
        else
        {
            currentEnemy = enemies[enemyIndex];
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
                BehaviourNode.BehaviourState enemyBehaviourState = currentEnemy.RunTree();
                if (enemyBehaviourState == BehaviourNode.BehaviourState.Success || enemyBehaviourState == BehaviourNode.BehaviourState.Failure)
                {
                    if (enemies.Count - 1 >= enemyIndex)
                    {
                        if (currentEnemy.GetActionPoints() <= 0)
                        {
                            enemyIndex++;
                        }
                        SetStateTakingTurn();
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
