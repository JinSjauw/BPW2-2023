using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectNode : ActionNode
{
    public LayerMask targetLayer;
    public float detectRadius;
    private float timer = 0.3f;
    
    
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        Debug.Log($"Detected {blackboard.playerTransform.name} !!");
    }

    protected override BehaviourState OnUpdate()
    {
        if (blackboard.playerTransform != null)
        {
            blackboard.targetPosition = LevelGrid.Instance.GetGridPosition(blackboard.playerTransform.position);
            
            return BehaviourState.Success;
        }
        
        Collider[] hits = Physics.OverlapSphere(unit.transform.position, detectRadius, targetLayer);
        if(hits.Length > 0)
        {
            blackboard.playerTransform = hits[0].transform.root;
            blackboard.targetPosition = LevelGrid.Instance.GetGridPosition(blackboard.playerTransform.position);
            behaviourState = BehaviourState.Success;
        }

        return behaviourState;
    }
}
