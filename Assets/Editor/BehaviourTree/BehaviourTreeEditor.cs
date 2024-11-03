using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeEditor : EditorWindow
{
    private BehaviourTreeView _treeView;
    private InspectorView _inspectorView;
    
    [MenuItem("BehaviourTreeEditor/Editor...")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviourTree/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);
        
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTree/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);
        
        _treeView = root.Q<BehaviourTreeView>();
        _inspectorView = root.Q<InspectorView>();
        _treeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        BehaviourTree behaviourTree = Selection.activeObject as BehaviourTree;
        if (behaviourTree && AssetDatabase.CanOpenAssetInEditor(behaviourTree.GetInstanceID()))
        {
            _treeView.PopulateView(behaviourTree);
        }
    }

    private void OnNodeSelectionChanged(NodeView node)
    {
        _inspectorView.UpdateSelection(node);
    }
}
