using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance  { get; private set; }
    
    [Header("Grid Data")]
    [SerializeField] private Transform gridDebugObject;
    [SerializeField] private int width, height, cellSize;
    
    private GridSystem<GridObject> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance of LevelGrid");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        gridSystem = new GridSystem<GridObject>(width, height, cellSize, (GridSystem<GridObject> _grid, GridPosition _gridPosition) => new GridObject(_grid, _gridPosition));
        gridSystem.CreateDebugObjects(gridDebugObject);
    }

    public Vector3 GetTargetGridPosition(Vector3 _worldPosition)
    {
        return GetWorldPosition(GetGridPosition(_worldPosition));
    }
    
    public GridPosition GetGridPosition(Vector3 _worldPosition) => gridSystem.GetGridPosition(_worldPosition);
    
    public Vector3 GetWorldPosition(GridPosition _gridPosition) => gridSystem.GetWorldPosition(_gridPosition);

    public GridObject GetGridObject(GridPosition _gridPosition) => gridSystem.GetGridObject(_gridPosition);

    public List<GridPosition> GetGridPositions()
    {
        List<GridPosition> result = new List<GridPosition>();
        for(int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                result.Add(new GridPosition(x,z));
            }
        }
        
        return result;
    }

    public List<Node> GetNodeGrid()
    {
        List<Node> nodeList = new List<Node>();
        List<GridPosition> grid = GetGridPositions();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //Add neighbours to node
                GridPosition position = new GridPosition(x, z);
                Node node = GetGridObject(position).gridNode;
                
                //Left
                if (grid.Contains(new GridPosition(position.x - 1, position.z)))
                {
                    node.neighbourList.Add(GetGridObject(new GridPosition(position.x - 1, position.z)).gridNode);
                    //Left Down
                    if (grid.Contains(new GridPosition(position.x - 1, position.z - 1)))
                    {
                        node.neighbourList.Add(GetGridObject(new GridPosition(position.x - 1, position.z - 1)).gridNode);
                    }
                    if (grid.Contains(new GridPosition(position.x - 1, position.z + 1)))
                    {
                        node.neighbourList.Add(GetGridObject(new GridPosition(position.x - 1, position.z + 1)).gridNode);
                    }
                }
                
                //Right
                if (grid.Contains(new GridPosition(position.x + 1, position.z)))
                {
                    node.neighbourList.Add(GetGridObject(new GridPosition(position.x + 1, position.z)).gridNode);
                    //Right Down
                    if (grid.Contains(new GridPosition(position.x + 1, position.z - 1)))
                    {
                        node.neighbourList.Add(GetGridObject(new GridPosition(position.x + 1, position.z - 1)).gridNode);
                    }
                    //Right Up
                    if (grid.Contains(new GridPosition(position.x + 1, position.z + 1)))
                    {
                        node.neighbourList.Add(GetGridObject(new GridPosition(position.x + 1, position.z + 1)).gridNode);
                    }
                }

                //Down
                if (grid.Contains(new GridPosition(position.x, position.z - 1)))
                {
                    node.neighbourList.Add(GetGridObject(new GridPosition(position.x, position.z - 1)).gridNode);
                }
                
                if (grid.Contains(new GridPosition(position.x, position.z + 1)))
                {
                    node.neighbourList.Add(GetGridObject(new GridPosition(position.x, position.z + 1)).gridNode);
                }
                
                nodeList.Add(node);
            }
        }

        return nodeList;
    }

    public int GetWidth()
    {
        return width;
    }
    
    public int GetHeight()
    {
        return height;
    }
}
