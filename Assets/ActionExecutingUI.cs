using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionExecutingUI : MonoBehaviour
{
    void Start()
    {
        UnitManager.Instance.OnActionStarted += UnitManager_OnActionStarted;
        UnitManager.Instance.OnActionComplete += UnitManager_OnActionComplete;
        
        Hide();
    }

    private void UnitManager_OnActionStarted(object sender, EventArgs e)
    {
        Show();
    }
    
    private void UnitManager_OnActionComplete(object sender, EventArgs e)
    {
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
