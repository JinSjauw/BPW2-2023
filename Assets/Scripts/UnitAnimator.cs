using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform projectilePrefab, shootPoint;
    private void Start()
    {
        Unit unit = GetComponent<Unit>();
        BehaviourTree tree = unit.GetTree();
        
        if (!unit.IsEnemy())
        {
            foreach (var action in unit.GetActionArray())
            {
                MoveAction moveAction = action as MoveAction;
                if (moveAction != null)
                {
                    moveAction.OnMove += MoveAction_OnMove;
                    moveAction.OnStop += MoveAction_OnStop;
                
                    continue;
                }
            
                ShootAction shootAction = action as ShootAction;
                if (shootAction != null)
                {
                    shootAction.OnShoot += ShootAction_OnShoot;
                }
                
                MeleeAction meleeAction = action as MeleeAction;
                if (meleeAction != null)
                {
                    meleeAction.OnMelee += MeleeAction_OnMelee;
                }
            }
        }
        else if(tree != null && unit.IsEnemy())
        {
            tree.nodes.ForEach((n) =>
            {
                MoveNode moveNode = n as MoveNode;
                if (moveNode != null)
                {
                    Debug.Log("Subbin enemy eventhandlers!");
                    moveNode.moveAction.OnMove += MoveAction_OnMove;
                    moveNode.moveAction.OnStop += MoveAction_OnStop;
                }
                MeleeNode meleeNode = n as MeleeNode;
                if (meleeNode != null)
                {
                    meleeNode.meleeAction.OnMelee += MeleeAction_OnMelee;
                }
            });
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

    private void MeleeAction_OnMelee(object _sender, EventArgs _e)
    {
        animator.SetTrigger("isMeleeing");
    }
}
