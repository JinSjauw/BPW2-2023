using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer renderer;

    public void Show()
    {
        renderer.enabled = true;
    }
    
    public void Hide()
    {
        renderer.enabled = false;
    }
}
