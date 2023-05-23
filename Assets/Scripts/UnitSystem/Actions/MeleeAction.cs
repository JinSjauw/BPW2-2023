using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAction : BaseAction
{

    public event EventHandler OnMelee;

    public int width;
    public int depth;
    
    
    public override void TakeAction(GridPosition _targetPosition, Action _onActionComplete)
    {
        ActionStart(_onActionComplete);
    }

    public override List<GridPosition> GetValidActionPositionsList()
    {
        //Show available Grid positions for melee attack
        
        
        
        return null;
    }

    public override string GetActionName()
    {
        return "Melee";
    }

    public override void Update()
    {
       
    }
}
