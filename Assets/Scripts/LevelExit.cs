using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Unit unit = collision.transform.root.GetComponent<Unit>();
        if (unit != null)
        {
            if (!unit.IsEnemy())
            {
                //Change to end scene
                LevelManager.Instance.LoadNextScene("MainMenu");
            }
        }
    }
}
