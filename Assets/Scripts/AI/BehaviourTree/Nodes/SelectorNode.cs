using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : CompositeNode
{
    private int current;

    protected override void OnStart()
    {
        Debug.Log("In Selector");
        current = 0;
    }

    protected override void OnStop()
    {
        Debug.Log("Stopped Selector");
        
    }

    protected override BehaviourState OnUpdate()
    {
        var child = children[current];
        BehaviourState state = child.Update();
        //Debug.Log($"Name: {child.name} : {state}");
        switch (state)
        {
            case BehaviourState.Running:
                return BehaviourState.Running;
            case BehaviourState.Failure:
                current++;
                break;
            case BehaviourState.Success:
                return BehaviourState.Success;
        }

        return current >= children.Count ? BehaviourState.Failure : BehaviourState.Running;
    }
}
