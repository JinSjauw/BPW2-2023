using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBar;
    [SerializeField] private HealthSystem healthSystem;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        healthSystem.OnHealthChanged += HealthSystemOnHealthChanged;
        UpdateActionPointsText();
        UpdateHealthBar();
    }

    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }
    
    private void UpdateHealthBar()
    {
        healthBar.fillAmount = healthSystem.GetHealthNormalized();
    }

    private void Unit_OnAnyActionPointsChanged(object _sender, EventArgs _e)
    {
        UpdateActionPointsText();
    }

    private void HealthSystemOnHealthChanged(object _sender, EventArgs _e)
    {
        UpdateHealthBar();
    }
}
