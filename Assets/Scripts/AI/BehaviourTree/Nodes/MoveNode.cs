using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveNode : ActionNode
{
    public MoveAction moveAction;

    private void OnActionComplete()
    {
        behaviourState = BehaviourState.Success;
        unit.SpendActionPoints(moveAction.GetActionPointsCost());
    }

    public override void Init()
    {
        moveAction = (MoveAction)moveAction.Clone();
        //Debug.Log(moveAction.name);
    }

    protected override void OnStart()
    {
        behaviourState = BehaviourState.Running;
        if(unit.CanTakeAction(moveAction))
        {
            moveAction.SetUnit(unit);
            moveAction.TakeAction( blackboard.targetPosition, OnActionComplete, null);
        }
        else
        {
            behaviourState = BehaviourState.Failure;
        }
    }

    protected override void OnStop()
    {
        //Debug.Log("Stopped Moving!");
    }

    protected override BehaviourState OnUpdate()
    {
        moveAction.Update();
        
        return behaviourState;
    }
}
