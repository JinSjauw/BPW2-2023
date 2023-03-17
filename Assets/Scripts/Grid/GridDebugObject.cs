using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    private GridObject gridObject;
    [SerializeField] private TextMeshPro textElement;
    public void SetGridObject(GridObject _gridObject)
    {
        gridObject = _gridObject;
    }

    private void Update()
    {
        textElement.text = gridObject.ToString();
    }
}
