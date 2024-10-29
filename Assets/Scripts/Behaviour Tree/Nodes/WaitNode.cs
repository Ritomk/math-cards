using UnityEngine;

public class WaitNode : ActionNode
{
    public float duration = 1.0f;
    private float startTime;


    protected override void OnStart()
    {
        startTime = Time.time;
    }

    protected override State OnUpdate()
    {
        return Time.time - startTime > duration ? State.Success : State.Running;
    }

    protected override void OnStop()
    {
        
    }
}