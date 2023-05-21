using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNode : ActionNode
{
    public MoveAction moveAction;
    
    private void OnActionComplete()
    {
        state = State.Success;
    }
    protected override void OnStart()
    {
        moveAction.TakeAction( new GridPosition(0, 0),OnActionComplete);
    }

    protected override void OnStop()
    {
        Debug.Log("Stopped Moving!");
    }

    protected override State OnUpdate()
    {
        return state;
    }
}
