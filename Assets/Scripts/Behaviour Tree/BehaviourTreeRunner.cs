using System;
using Unity.VisualScripting;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    private BehaviourTree _tree;

    private void Start()
    {
        _tree = ScriptableObject.CreateInstance<BehaviourTree>();

        var log1 = ScriptableObject.CreateInstance<DebugLogNode>();
        log1.message = "This is a behaviour tree1";
        
        var pause1 = ScriptableObject.CreateInstance<WaitNode>();
        
        var log3 = ScriptableObject.CreateInstance<DebugLogNode>();
        log3.message = "This is a behaviour tree3";
        
        var sequence = ScriptableObject.CreateInstance<SequencerNode>();
        sequence._children.Add(log1);
        sequence._children.Add(pause1);
        sequence._children.Add(log3);
        
        var loop = ScriptableObject.CreateInstance<RepeatNode>();
        loop.child = sequence;
        
        _tree.rootNode = loop;
    }

    private void Update()
    {
        _tree.Update();
    }
}