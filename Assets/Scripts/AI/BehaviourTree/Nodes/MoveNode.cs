using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveNode : ActionNode
{
    public MoveAction moveAction;

    private void OnActionComplete()
    {
        state = State.Success;
    }

    public override void Init()
    {
        moveAction = (MoveAction)moveAction.Clone();
        Debug.Log(moveAction.name);
    }

    protected override void OnStart()
    {
        state = State.Running;
        moveAction.SetUnit(unit);
        moveAction.TakeAction( blackboard.moveToPosition, OnActionComplete);
    }

    protected override void OnStop()
    {
        Debug.Log("Stopped Moving!");
    }

    protected override State OnUpdate()
    {
        moveAction.Update();
        
        return state;
    }
}
