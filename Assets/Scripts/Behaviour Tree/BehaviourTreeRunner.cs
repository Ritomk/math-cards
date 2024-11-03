using System;
using Unity.VisualScripting;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    [SerializeField] private BehaviourTree _tree;

    private void Start()
    {
        _tree = _tree.Clone();
    }

    private void Update()
    {
        _tree.Update();
    }
}