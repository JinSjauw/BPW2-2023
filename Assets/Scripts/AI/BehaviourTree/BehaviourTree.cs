using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public BehaviourNode rootNode;
    public BehaviourNode.State treeState = BehaviourNode.State.Running;
    public List<BehaviourNode> nodes = new List<BehaviourNode>();

    public BehaviourNode.State Update()
    {
        if (rootNode.state == BehaviourNode.State.Running)
        {
            treeState = rootNode.Update();
        }

        return treeState;
    }

    public BehaviourNode CreateNode(System.Type type)
    {
        BehaviourNode node = ScriptableObject.CreateInstance(type) as BehaviourNode;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        
        Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
        nodes.Add(node);
        
        AssetDatabase.AddObjectToAsset(node, this);
        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(BehaviourNode node)
    {
        Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
        nodes.Remove(node);
        
        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);
        
        AssetDatabase.SaveAssets();
    }

    public void AddChild(BehaviourNode parent, BehaviourNode child)
    {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
            decorator.child = child;
            EditorUtility.SetDirty(decorator);
        }
        
        RootNode root = parent as RootNode;
        if (root)
        {
            Undo.RecordObject(root, "Behaviour Tree (AddChild)");
            root.child = child;
            EditorUtility.SetDirty(root);
        }
        
        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
            composite.children.Add(child);
            EditorUtility.SetDirty(composite);
        }
    }
    
    public void RemoveChild(BehaviourNode parent, BehaviourNode child)
    {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
            decorator.child = null;
            EditorUtility.SetDirty(decorator);
        }
        
        RootNode root = parent as RootNode;
        if (root)
        {
            Undo.RecordObject(root, "Behaviour Tree (RemoveChild)");
            root.child = null;
            EditorUtility.SetDirty(root);
        }
        
        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
            composite.children.Remove(child);
            EditorUtility.SetDirty(composite);
        }
    }

    public List<BehaviourNode> GetChildren(BehaviourNode parent)
    {
        List<BehaviourNode> children = new List<BehaviourNode>();
        
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator && decorator.child != null)
        {
            children.Add(decorator.child);
        }
        
        RootNode root = parent as RootNode;
        if (root && root.child != null)
        {
            children.Add(root.child);
        }
        
        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            return composite.children;
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
