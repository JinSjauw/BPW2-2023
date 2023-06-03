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

    private Unit selectedUnit;
    // Start is called before the first frame update
    private void Awake()
    {
        actionButtons = new List<ActionButtonUI>();
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
    }

    void Start()
    {
        UnitActionManager.Instance.SelectedActionChanged += UnitManager_SelectedActionChanged;
        UnitActionManager.Instance.OnActionStarted += UnitManager_OnActionStarted;
        
        EnemyManager.OnCombatStart += EnemyManager_OnOnCombatStart;
        EnemyManager.OnCombatEnd += EnemyManager_OnOnCombatEnd;
        
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        InventoryUI.OnOpenInventory += InventoryUI_OnOpenInventory;
        
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        if (!unit.IsEnemy())
        {
            selectedUnit = unit;
            Init();
        }
    }

    private void EnemyManager_OnOnCombatEnd(object sender, EventArgs e)
    {
        actionPointCounter.gameObject.SetActive(false);
    }

    private void EnemyManager_OnOnCombatStart(object sender, EventArgs e)
    {
        actionPointCounter.gameObject.SetActive(true);
    }

    private void CreateActionButtons()
    {
        foreach (Transform buttonTransform in buttonContainer)
        {
            Destroy(buttonTransform.gameObject);
        }
        
        actionButtons.Clear();

        if (selectedUnit == null)
        {
            Debug.Log("Selected Unit is NULL");
            return;
        }
        
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
        
        gridSystemVisual.UpdateGridVisual();
    }
    
    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionManager.Instance.GetSelectedUnit();
        if (selectedUnit == null)
        {
            return;
        }
        actionPointCounter.text = "AP : " + selectedUnit.GetActionPoints();
    }

    private void Init()
    {
        Debug.Log("Init ");
        CreateActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }
    
    private void InventoryUI_OnOpenInventory(object sender, EventArgs e)
    {
        gridSystemVisual.HideAllTileVisuals();
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
        gridSystemVisual.HideAllTileVisuals();
    }

    private void Unit_OnAnyActionPointsChanged(object _sender, EventArgs _e)
    {
        UpdateActionPoints();
    }
}
