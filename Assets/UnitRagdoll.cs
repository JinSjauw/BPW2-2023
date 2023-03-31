using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{

    [SerializeField] private Transform ragdollRootBone;

    public void Init(Transform originalRootBone)
    {
        MathAllChildBones(originalRootBone, ragdollRootBone);
    }

    private void MathAllChildBones(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if(cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;
                
                MathAllChildBones(child, cloneChild);
            }
        }
    }
}
