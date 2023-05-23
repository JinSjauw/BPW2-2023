using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : CompositeNode
{
    private int current;
    protected override void OnStart()
    {
        current = 0;
    }

    protected override void OnStop()
    {
        
    }

    protected override BehaviourState OnUpdate()
    {
        var child = children[current];
        switch (child.Update())
        {
            case BehaviourState.Running:
                return BehaviourState.Running;
            case BehaviourState.Failure:
                return BehaviourState.Failure;
            case BehaviourState.Success:
                current++;
                break;
        }
        
        return current >= children.Count ? BehaviourState.Success : BehaviourState.Running;
    }
}
