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
        Debug.Log($"OnStop{messsage}");
    }

    protected override State OnUpdate()
    {
        return State.Success;
    }
}
