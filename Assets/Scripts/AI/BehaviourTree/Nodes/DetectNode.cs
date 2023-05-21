using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectNode : ActionNode
{
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        //if player comes in detect radius
        //set target to player
        //

        GameObject target = null;

        //Sphere overlapcast periodically
        //Return all objects on the playerlayer
        //
        
        blackboard.playerObject = target.transform;

        return state;
    }
}
