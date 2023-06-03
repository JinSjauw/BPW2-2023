using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform tileVisualPrefab;
    private TileVisualSingle[,] tileVisualArray;

    public static event EventHandler OnVisualActive;
    public static event EventHandler OnVisualOff;

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

    private void OnDestroy()
    {
        OnVisualActive = null;
        OnVisualOff = null;
    }

    public void HideAllTileVisuals()
    {
        OnVisualOff?.Invoke(this, EventArgs.Empty);

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                if (tileVisualArray == null)
                {
                    continue;
                }
                tileVisualArray[x, z].Hide();
            }
        }
    }

    public void ShowTileList(List<GridPosition> _gridPositions)
    {
        OnVisualActive?.Invoke(this, EventArgs.Empty);
        foreach (GridPosition tile in _gridPositions)
        {
            tileVisualArray[tile.x, tile.z].Show();
        }
    }

    public void UpdateGridVisual()
    {
        HideAllTileVisuals();

        BaseAction selectedAction = UnitActionManager.Instance.GetSelectedAction();

        if (selectedAction == null)
        {
            Debug.Log("selectedAction is NULL");
            return;
        }
        
        ShowTileList(selectedAction.GetValidActionPositionsList());
        //Debug.Log(selectedAction.GetValidActionPositionsList().Count);
    }
}
