using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : ActionNode
{
    public float duration = 1;
    private float startTime;
    
    protected override void OnStart()
    {
        startTime = Time.time;
    }

    protected override void OnStop()
    {
       
    }

    protected override BehaviourState OnUpdate()
    {
        if (Time.time - startTime > duration)
        {
            return BehaviourState.Success;
        }

        return BehaviourState.Running;
    }
}
