using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeNode : ActionNode
{
    public MeleeAction meleeAction;

    private void OnActionComplete()
    {
        behaviourState = BehaviourState.Success;
        unit.SpendActionPoints(meleeAction.GetActionPointsCost());
    }

    private void OnActionFail()
    {
        behaviourState = BehaviourState.Failure;
    }

    public override void Init()
    {
        meleeAction = (MeleeAction)meleeAction.Clone();
        Debug.Log(meleeAction.name);
    }

    protected override void OnStart()
    {
        behaviourState = BehaviourState.Running;
        Debug.Log("In Melee Node!");
        if (unit.CanTakeAction(meleeAction))
        {
            meleeAction.SetUnit(unit);
            meleeAction.TakeAction(blackboard.targetPosition, OnActionComplete, OnActionFail);
        }
        else
        {
            behaviourState = BehaviourState.Failure;
        }
    }

    protected override void OnStop()
    {
        
    }

    protected override BehaviourState OnUpdate()
    {
        meleeAction.Update();
        
        return behaviourState;
    }
}
