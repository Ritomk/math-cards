using System;
using UnityEngine;

public class SequencerNode : CompositeNode
{
    private int _current;
    
    protected override void OnStart()
    {
        _current = 0;
    }

    protected override State OnUpdate()
    {
        var child = _children[_current];
        
        switch (child.Update())
        {
            case State.Running:
                return State.Running;
                break;
            case State.Failure:
                return State.Failure;
                break;
            case State.Success:
                _current++;
                break;
        }
        
        return _current == _children.Count ? State.Success : State.Running;
    }

    protected override void OnStop()
    {
        
    }
}