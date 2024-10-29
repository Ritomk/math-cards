using UnityEngine;

public class RepeatNode : DecoratorNode
{
    protected override void OnStart()
    {
    }

    protected override State OnUpdate()
    {
        child.Update();
        return State.Running;
    }

    protected override void OnStop()
    {
    }
}