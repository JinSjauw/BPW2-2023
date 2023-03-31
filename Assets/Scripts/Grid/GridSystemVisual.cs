using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform tileVisualPrefab;
    private TileVisualSingle[,] tileVisualArray;
    
    private void Start()
    {
        tileVisualArray = new TileVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()];
        
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform tile = Instantiate(tileVisualPrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                tileVisualArray[x, z] = tile.GetComponent<TileVisualSingle>();
            }
        }
        HideAllTileVisuals();
    }

    public void HideAllTileVisuals()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                tileVisualArray[x, z].Hide();
            }
        }
    }

    public void ShowTileList(List<GridPosition> _gridPositions)
    {
        foreach (GridPosition tile in _gridPositions)
        {
            tileVisualArray[tile.x, tile.z].Show();
        }
    }
}
