using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootNode : ActionNode
{
    public ShootAction shootAction;

    private void OnActionComplete()
    {
        behaviourState = BehaviourState.Success;
        unit.SpendActionPoints(shootAction.GetActionPointsCost());
    }

    private void OnActionFail()
    {
        behaviourState = BehaviourState.Failure;
    }

    public override void Init()
    {
        shootAction = (ShootAction)shootAction.Clone();
        //Debug.Log(shootAction.name);
    }

    protected override void OnStart()
    {
        behaviourState = BehaviourState.Running;
        Debug.Log("In Shoot Node!");
        if (unit.CanTakeAction(shootAction))
        {
            shootAction.SetUnit(unit);
            shootAction.TakeAction(LevelGrid.Instance.GetGridPosition(blackboard.playerTransform.position), OnActionComplete, OnActionFail);
        }
        else
        {
            behaviourState = BehaviourState.Failure;
        }
    }

    protected override void OnStop()
    {
        
    }

    protected override BehaviourState OnUpdate()
    {
        shootAction.Update();
        
        return behaviourState;
    }
}
