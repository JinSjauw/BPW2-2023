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
        
    public void Init(Vector3 _target)
    {
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
        }
    }
}
