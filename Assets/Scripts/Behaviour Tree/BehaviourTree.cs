using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.Running;
    
    public List<Node> nodes = new List<Node>();

    public Node.State Update()
    {
        if (rootNode.state == Node.State.Running)
        {
            treeState = rootNode.Update();
        }
        return treeState;
    }

    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = Guid.NewGuid().ToString();
        nodes.Add(node);
        
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(Node node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode)
        {
            decoratorNode.child = child;
        }
        
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
         rootNode.child = child;   
        }
        
        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode)
        {
            compositeNode._children.Add(child);
        }
    }

    public void RemoveChild(Node parent, Node child)
    {
        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode)
        {
            decoratorNode.child = null;
        }
        
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            rootNode.child = null;   
        }
        
        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode)
        {
            compositeNode._children.Remove(child);
        }
    }

    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();
        
        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode && decoratorNode.child != null)
        {
            children.Add(decoratorNode.child);
        }
        
        RootNode rootNode = parent as RootNode;
        if (rootNode && rootNode.child != null)
        {
            children.Add(rootNode.child);   
        }
        
        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode)
        {
            return compositeNode._children;
        }
        
        return children;
    }

    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();
        return tree;
    }
}