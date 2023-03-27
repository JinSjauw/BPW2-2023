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
    [SerializeField] private bool CreateDebugGrid = false;
    [SerializeField] private bool GenerateDungeon = false;
    
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
        
        gridSystem = new GridSystem<GridObject>(width, height, cellSize, (GridSystem<GridObject> _grid, GridPosition _gridPosition, Vector3 _worldPosition) => new GridObject(_grid, _gridPosition, _worldPosition));
        SetNeighbours();
        if (CreateDebugGrid)
        {
            gridSystem.CreateDebugObjects(gridDebugObject);
        }
    }

    private void Start()
    {
        if (!GenerateDungeon)
        {
            return;
        }
        DungeonGenerator.Instance.Initialize();
        //gridSystem.CreateDebugObjects(DungeonGenerator.Instance.Generate(), gridDebugObject);
        DungeonGenerator.Instance.Generate();
    }

    private void SetNeighbours()
    {
        List<GridObject> nodeList = new List<GridObject>();
        List<GridPosition> grid = GetGridPositions();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //Add neighbours to node
                GridPosition position = new GridPosition(x, z);
                GridObject node = GetGridObject(position);
                
                //Left
                if (grid.Contains(new GridPosition(position.x - 1, position.z)))
                {
                    node.neighbourList.Add(GetGridObject(new GridPosition(position.x - 1, position.z)));
                    
                    //Left Down
                    if (grid.Contains(new GridPosition(position.x - 1, position.z - 1)))
                    {
                        node.neighbourList.Add(GetGridObject(new GridPosition(position.x - 1, position.z - 1)));
                    }
                    if (grid.Contains(new GridPosition(position.x - 1, position.z + 1)))
                    {
                        node.neighbourList.Add(GetGridObject(new GridPosition(position.x - 1, position.z + 1)));
                    }
                }
                
                //Right
                if (grid.Contains(new GridPosition(position.x + 1, position.z)))
                {
                    node.neighbourList.Add(GetGridObject(new GridPosition(position.x + 1, position.z)));
                    
                    //Right Down
                    if (grid.Contains(new GridPosition(position.x + 1, position.z - 1)))
                    {
                        node.neighbourList.Add(GetGridObject(new GridPosition(position.x + 1, position.z - 1)));
                    }
                    //Right Up
                    if (grid.Contains(new GridPosition(position.x + 1, position.z + 1)))
                    {
                        node.neighbourList.Add(GetGridObject(new GridPosition(position.x + 1, position.z + 1)));
                    }
                }

                //Down
                if (grid.Contains(new GridPosition(position.x, position.z - 1)))
                {
                    node.neighbourList.Add(GetGridObject(new GridPosition(position.x, position.z - 1)));
                }
                
                if (grid.Contains(new GridPosition(position.x, position.z + 1)))
                {
                    node.neighbourList.Add(GetGridObject(new GridPosition(position.x, position.z + 1)));
                }
                
                nodeList.Add(node);
            }
        }
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

    public List<GridObject> GetNodeGrid()
    {
        List<GridObject> nodeList = new List<GridObject>();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridObject node = GetGridObject(new GridPosition(x, z));
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
