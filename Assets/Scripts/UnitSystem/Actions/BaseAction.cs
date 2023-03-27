using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction
{
   protected bool isActive = false;
   protected Unit unit;

   public abstract void SetUnit(Unit _unit, object _obj);
   public abstract void Execute();
}
