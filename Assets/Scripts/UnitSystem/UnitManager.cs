using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private List<Unit> unitList;
    private List<Unit> allyList;
    private List<Unit> enemiesList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance of UnitManager");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        unitList = new List<Unit>();
        allyList = new List<Unit>();
        enemiesList = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitSpawned(object _sender, EventArgs _e)
    {
        Unit unit = _sender as Unit;
        unitList.Add(unit);
        if (unit.IsEnemy())
        {
            enemiesList.Add(unit);
        }
        else
        {
            allyList.Add(unit);
            //UnitActionManager.Instance.SetSelectedUnit(unit);
        }
    }
    private void Unit_OnAnyUnitDead(object _sender, EventArgs _e)
    {
        Unit unit = _sender as Unit;

        unitList.Remove(unit);
        if (unit.IsEnemy())
        {
            enemiesList.Remove(unit);
        }
        else
        {
            allyList.Remove(unit);
        }
    }
}
