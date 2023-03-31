using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionSystemUI : MonoBehaviour
{
    [SerializeField] private GridSystemVisual gridSystemVisual;
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private TextMeshProUGUI actionPointCounter;

    private List<ActionButtonUI> actionButtons;
    // Start is called before the first frame update
    void Start()
    {
        UnitManager.Instance.SelectedUnitChanged += UnitManager_SelectedUnitChanged;
        UnitManager.Instance.SelectedActionChanged += UnitManager_SelectedActionChanged;
        UnitManager.Instance.OnActionStarted += UnitManager_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        
        
        actionButtons = new List<ActionButtonUI>();
        
        CreateActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
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
    
    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitManager.Instance.GetSelectedUnit();
        actionPointCounter.text = "AP : " + selectedUnit.GetActionPoints();
    }
    
    private void UnitManager_SelectedUnitChanged(object _sender, EventArgs _e)
    {
        CreateActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }
    
    private void UnitManager_SelectedActionChanged(object _sender, EventArgs _e)
    {
        UpdateSelectedVisual();
    }

    private void UnitManager_OnActionStarted(object _sender, EventArgs _e)
    {
        UpdateActionPoints();
    }
    
    private void TurnSystem_OnTurnChanged(object _sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChanged(object _sender, EventArgs _e)
    {
        UpdateActionPoints();
    }
}
