using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform projectilePrefab, shootPoint;
    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnMove += MoveAction_OnMove;
            moveAction.OnStop += MoveAction_OnStop;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
    }

    private void MoveAction_OnMove(object _sender, EventArgs _e)
    {
        animator.SetBool("isMoving", true);
    }

    private void MoveAction_OnStop(object _sender, EventArgs _e)
    {
        animator.SetBool("isMoving", false);
    }
    
    private void ShootAction_OnShoot(object _sender, ShootAction.OnShootEventArgs _e)
    {
        animator.SetTrigger("isShooting");

       Transform projectileTransform = Instantiate(projectilePrefab, shootPoint.position, quaternion.identity);
       Projectile projectile = projectileTransform.GetComponent<Projectile>();

       Vector3 shootAtTarget = _e.targetUnit.GetWorldPosition();
       shootAtTarget.y = shootPoint.transform.position.y;
       
       projectile.Init(shootAtTarget);
    }
}
