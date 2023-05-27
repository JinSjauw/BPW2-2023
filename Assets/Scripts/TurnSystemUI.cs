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
        
        //Should be enemy Manager
        EnemyManager.OnCombatStart += EnemyManager_OnOnCombatStart;
        EnemyManager.OnCombatEnd += EnemyManager_OnOnCombatEnd;
        
        UpdateTurnText();
        UpdateEnemyVisual();
        
        //UpdateEndTurnButton();
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
    private void EnemyManager_OnOnCombatStart(object sender, EventArgs e)
    {
        endTurnButton.gameObject.SetActive(true);
        turnCounter.gameObject.SetActive(true);
    }
    private void EnemyManager_OnOnCombatEnd(object sender, EventArgs e)
    {
        endTurnButton.gameObject.SetActive(false);
        turnCounter.gameObject.SetActive(false);
        
    }
    private void TurnSystem_OnTurnChanged(object _sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyVisual();
        UpdateEndTurnButton();
    }
}
