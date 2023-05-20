using System;
using Unity.VisualScripting;
using UnityEngine;


public class TreeRunner : MonoBehaviour
{
    public BehaviourTree tree;
    
    private void Start()
    {
        tree = tree.Clone();
        tree.Bind(GetComponent<Unit>());
    }

    private void Update()
    {
        tree.Update();
    }
}
