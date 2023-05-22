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

    protected override State OnUpdate()
    {
        //if player comes in detect radius
        //set target to player

        //Sphere overlapcast periodically
        //Return all objects on the playerlayer

        if (blackboard.playerTransform != null)
        {
            blackboard.moveToPosition = LevelGrid.Instance.GetGridPosition(blackboard.playerTransform.position);
            
            return State.Success;
        }
        
        Collider[] hits = Physics.OverlapSphere(unit.transform.position, detectRadius, targetLayer);
        if(hits.Length > 0)
        {
            blackboard.playerTransform = hits[0].transform.root;
            blackboard.moveToPosition = LevelGrid.Instance.GetGridPosition(blackboard.playerTransform.position);
            state = State.Success;
        }

        return state;
    }
}
