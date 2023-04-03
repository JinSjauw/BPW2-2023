using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private GameObject visual;
    [SerializeField] private Unit unit;
    
    private void Start()
    {
        UnitActionManager.Instance.SelectedUnitChanged += UnitManager_SelectedUnitChanged;
    }

    private void UnitManager_SelectedUnitChanged(object sender, EventArgs empty)
    {
        if (UnitActionManager.Instance.GetSelectedUnit() == unit)
        {
            visual.SetActive(true);
        }
        else
        {
            visual.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        UnitActionManager.Instance.SelectedUnitChanged -= UnitManager_SelectedUnitChanged;

    }
}
