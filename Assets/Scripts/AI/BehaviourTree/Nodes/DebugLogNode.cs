using UnityEngine;


public class DebugLogNode : ActionNode
{
    public string messsage;
    protected override void OnStart()
    {
        Debug.Log($"OnStart{messsage}");
    }

    protected override void OnStop()
    {
        
    }

    protected override BehaviourState OnUpdate()
    {
        return BehaviourState.Success;
    }
}
