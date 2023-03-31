using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSystemUI : MonoBehaviour
{
    [SerializeField] private GridSystemVisual gridSystemVisual;
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform buttonContainer;

    private List<ActionButtonUI> actionButtons;
    // Start is called before the first frame update
    void Start()
    {
        UnitManager.Instance.SelectedUnitChanged += UnitManager_SelectedUnitChanged;
        UnitManager.Instance.SelectedActionChanged += UnitManager_SelectedActionChanged;
        
        actionButtons = new List<ActionButtonUI>();
        
        CreateActionButtons();
        UpdateSelectedVisual();
    }

    private void CreateActionButtons()
    {
        foreach (Transform buttonTransform in buttonContainer)
        {
            Destroy(buttonTransform.gameObject);
        }
        
        actionButtons.Clear();
        
        Unit selectedUnit = UnitManager.Instance.GetSelectedUnit();
        
        foreach (BaseAction action in selectedUnit.GetActionArray())
        {
           Transform buttonTransform = Instantiate(actionButtonPrefab, buttonContainer);
           ActionButtonUI button = buttonTransform.GetComponent<ActionButtonUI>();
           button.SetGridSystemVisual(gridSystemVisual);
           button.SetButton(action);
           actionButtons.Add(button);
        }
    }

    private void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI button in actionButtons)
        {
            button.UpdateSelectedVisual();
        }
    }
    
    private void UnitManager_SelectedUnitChanged(object _sender, EventArgs _e)
    {
        CreateActionButtons();
        UpdateSelectedVisual();
    }
    
    private void UnitManager_SelectedActionChanged(object _sender, EventArgs _e)
    {
        UpdateSelectedVisual();
    }
}
