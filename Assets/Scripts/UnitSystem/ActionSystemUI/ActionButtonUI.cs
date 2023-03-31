using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private GameObject selectedVisual;
    [SerializeField] private Button button;
    [SerializeField] private GridSystemVisual gridSystemVisual;
    private BaseAction action;
    // Start is called before the first frame update

    private void UnitManager_OnActionComplete(object sender, EventArgs e)
    {
        OnActionComplete();    
    }
    
    private void OnActionComplete()
    {
        gridSystemVisual.HideAllTileVisuals();
    }

    public void SetGridSystemVisual(GridSystemVisual _gridSystemVisual)
    {
        gridSystemVisual = _gridSystemVisual;
    }
    
    public void SetButton(BaseAction _action)
    {
        UnitManager.Instance.OnActionComplete -= UnitManager_OnActionComplete;
        UnitManager.Instance.OnActionComplete += UnitManager_OnActionComplete;
        
        textMeshPro.text = _action.GetActionName().ToUpper();
        action = _action;
        
        button.onClick.AddListener(() =>
        {
            UnitManager.Instance.SetSelectedAction(_action);
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedAction = UnitManager.Instance.GetSelectedAction();
        selectedVisual.SetActive(selectedAction == action);

        if (selectedAction == action)
        {
            if (gridSystemVisual != null)
            {
                gridSystemVisual.ShowTileList(action.GetValidActionPositionsList());
            }
        }
    }
}
