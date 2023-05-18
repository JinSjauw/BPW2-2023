using UnityEngine;

public abstract class DecoratorNode : BehaviourNode
{
    [HideInInspector] public BehaviourNode child;
    
    public override BehaviourNode Clone()
    {
        DecoratorNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}
