using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private GameObject loadCanvas;
    [SerializeField] private GameObject deathCanvas;
    private float target;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(loadCanvas);
            DontDestroyOnLoad(deathCanvas);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        if (!unit.IsEnemy())
        {
            deathCanvas.SetActive(true);
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Unit.OnAnyUnitDead -= Unit_OnAnyUnitDead;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        loadCanvas.SetActive(false);
        deathCanvas.SetActive(false);
    }

    public void LoadNextScene(string _sceneName)
    {
        loadCanvas.SetActive(true);
        SceneManager.LoadScene(_sceneName);
    }
}
