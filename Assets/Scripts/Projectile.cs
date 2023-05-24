using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform projectileVFXPrefab;
    private Vector3 target;
    private Unit targetUnit;
        
    public void Init(Unit _targetUnit, Vector3 _target)
    {
        targetUnit = _targetUnit;
        target = _target;
    }

    private void Update()
    {
        Vector3 moveDirection = (target - transform.position).normalized;

        float startDistance = Vector3.Distance(transform.position, target);
        
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        
        float endDistance = Vector3.Distance(transform.position, target);

        if (startDistance < endDistance)
        {
            transform.position = target;
            trailRenderer.transform.parent = null;
            Instantiate(projectileVFXPrefab, target, Quaternion.identity);
            Destroy(gameObject);
            targetUnit.Damage(25);
        }
    }
}
