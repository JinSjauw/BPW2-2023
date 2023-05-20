using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : BehaviourNode
{
    public BehaviourNode child;
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        //Go to the next Unit
        
    }

    protected override State OnUpdate()
    {
        return child.Update();
    }

    public override BehaviourNode Clone()
    {
        RootNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}
