using UnityEngine;


public class DebugLogNode : ActionNode
{
    public string message;


    protected override void OnStart()
    {
        Debug.Log($"OnStart: <color=yellow>{message}</color>");
    }

    protected override State OnUpdate()
    {
        Debug.Log($"OnUpdate: <color=yellow>{message}</color>");
        return State.Success;
    }

    protected override void OnStop()
    {
        Debug.Log($"OnStop: <color=yellow>{message}</color>");
    }
}
