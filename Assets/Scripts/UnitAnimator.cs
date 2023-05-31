using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RuntimeAnimatorController unarmedController;
    [SerializeField] private RuntimeAnimatorController armedController;
    private void Start()
    {
        Unit unit = GetComponent<Unit>();
        BehaviourTree tree = unit.GetTree();

        unit.isHit += Unit_IsHit;
        unit.EquippedWeapon += Unit_EquippedWeapon;
        unit.UnequippedWeapon += Unit_UnequippedWeapon;
        
        if (!unit.IsEnemy())
        {
            animator.runtimeAnimatorController = unarmedController;
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

    private void Unit_UnequippedWeapon(object sender, EventArgs e)
    {
        animator.runtimeAnimatorController = unarmedController;
    }

    private void Unit_EquippedWeapon(object sender, EventArgs e)
    {
        animator.runtimeAnimatorController = armedController;
    }

    private void Unit_IsHit(object sender, EventArgs e)
    {
        animator.SetTrigger("isHit");
    }

    private void MoveAction_OnMove(object _sender, EventArgs _e)
    {
        animator.SetBool("isMoving", true);
    }

    private void MoveAction_OnStop(object _sender, EventArgs _e)
    {
        animator.SetBool("isMoving", false);
    }
    
    private void ShootAction_OnShoot(object _sender, EventArgs _e)
    {
        animator.SetTrigger("isShooting");
    }

    private void MeleeAction_OnMelee(object _sender, EventArgs _e)
    {
        animator.SetTrigger("isMeleeing");
    }
}
