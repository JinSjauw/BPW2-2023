using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : BehaviourNode
{
    [HideInInspector] public List<BehaviourNode> children = new List<BehaviourNode>();
    
    public override BehaviourNode Clone()
    {
        CompositeNode node = Instantiate(this);
        node.children = children.ConvertAll(c => c.Clone());
        return node;
    }
}
