using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    public void LoadNextScene(string _sceneName)
    {
        Debug.Log("Clicked " + transform.name);
        LevelManager.Instance.LoadNextScene(_sceneName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
