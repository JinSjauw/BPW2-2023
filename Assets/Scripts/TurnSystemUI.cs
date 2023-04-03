using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnCounter;
    [SerializeField] private GameObject enemyTurnVisual;

    private void Start()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        
        UpdateTurnText();
        UpdateEnemyVisual();
        UpdateEndTurnButton();
    }

    private void UpdateTurnText()
    {
        turnCounter.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
    }

    private void UpdateEnemyVisual()
    {
        enemyTurnVisual.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButton()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());

    }
    
    private void TurnSystem_OnTurnChanged(object _sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyVisual();
        UpdateEndTurnButton();
    }
}
