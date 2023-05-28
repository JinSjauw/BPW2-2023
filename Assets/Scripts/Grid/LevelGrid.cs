using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance  { get; private set; }
    
    [Header("Grid Data")]
    [SerializeField] private Transform gridDebugObject;
    [SerializeField] private int width, height, cellSize;
    [SerializeField] private bool CreateDebugGrid = false;
    [SerializeField] private bool GenerateDungeon = false;
    [SerializeField] private List<GridPosition> walkableList;

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
        /*if (CreateDebugGrid)
        {
            //gridSystem.CreateDebugObjects(gridDebugObject);
            gridSystem.CreateWalkableDebugObjects(gridDebugObject, walkableList);
        }*/
    }

    private void Start()
    {
        if (!GenerateDungeon)
        {
            walkableList = gridSystem.GetGridPositions();
            return;
        }
        DungeonGenerator.Instance.Initialize();
        //gridSystem.CreateDebugObjects(DungeonGenerator.Instance.Generate(), gridDebugObject);
        walkableList = DungeonGenerator.Instance.Generate();
        if (CreateDebugGrid)
        {
            //gridSystem.CreateDebugObjects(gridDebugObject);
            gridSystem.CreateWalkableDebugObjects(gridDebugObject, walkableList);
        }
    }

    public List<GridPosition> GetWalkableList()
    {
        return walkableList;
    }

    public void SetUnitAtGridObject(GridPosition _gridPosition, Unit _unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(_gridPosition);
        if (!gridObject.HasUnit())
        {
            gridObject.SetUnit(_unit);
        }
    }

    public void SetUnitAtGridPosition(GridPosition _gridPosition, Unit _unit)
    {
        _unit.transform.position = GetWorldPosition(_gridPosition);
    }
    
    public Unit GetUnitAtGridPosition(GridPosition _gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(_gridPosition);
        
        return gridObject.GetUnit();
    }

    public void ClearUnitAtGridPosition(GridPosition _gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(_gridPosition);
        gridObject.ClearUnit();
    }
    
    public void UnitMovedGridPosition(Unit _unit, GridPosition _fromGridPosition, GridPosition _toGridPosition)
    {
        Unit occupyingUnit = GetUnitAtGridPosition(_fromGridPosition);
        if (occupyingUnit == _unit)
        {
            ClearUnitAtGridPosition(_fromGridPosition);
        }

        if (!GetGridObject(_toGridPosition).HasUnit())
        {
            SetUnitAtGridObject(_toGridPosition, _unit);
        }
    }

    public bool HasAnyUnit(GridPosition _gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(_gridPosition);
        return gridObject.HasUnit();
    }
    public Vector3 GetTargetGridPosition(Vector3 _worldPosition)
    {
        return GetWorldPosition(GetGridPosition(_worldPosition));
    }

    public List<GridPosition> GetWalkableTilesInCircle(Vector3 _center, float _radius)
    {
        List<GridPosition> inCircleList = gridSystem.GetTilesInCircle(_center, _radius);

        List<GridPosition> validList = new List<GridPosition>();

        foreach (var tile in inCircleList)
        {
            if (walkableList.Contains(tile))
            {
                validList.Add(tile);
            }
        }

        List<GridPosition> result = new List<GridPosition>();
        result = validList;
        
        
        
        //Pathfind towards the end zone tiles && check if it can reach
        //if it cant go a tile back
        //show only tiles that are reachable
        
        //Breadth first Search
        
        /*List<GridPosition> unWalkableList = new List<GridPosition>();
        //Go through this list to check if tile is reachable from the center
        //Raycast to check for wall
        Vector3 origin = _center;
        origin.y += 1f;
        foreach (var tile in validList)
        {
            Vector3 tileWorldPosition = GetWorldPosition(tile);
            tileWorldPosition.y += 1f;
            Vector3 direction = new Vector3(tileWorldPosition.x - origin.x, origin.y, tileWorldPosition.z - origin.z).normalized;
            float distance = Vector3.Distance(new Vector3(tileWorldPosition.x, origin.y, tileWorldPosition.z), origin);
            if(Physics.Raycast(origin, direction, distance, LayerMask.GetMask("Walls")))
            {
                unWalkableList.Add(tile);
                Debug.Log($"Hit Wall! {tileWorldPosition} Origin: {origin} Distance: {distance}");
            }
        }

        List<GridPosition> result = new List<GridPosition>();

        foreach (var tile in validList)
        {
            if (!unWalkableList.Contains(tile))
            {
                result.Add(tile);
            }
        }*/

        return result;
    }
    
    public bool insideCircle(Vector3 _center, Vector3 _tile, float _radius) => gridSystem.insideCircle(_center, _tile, _radius);

    public void RemoveUnitAtGridPosition(GridPosition _gridPosition) => gridSystem.RemoveUnitAtGridPosition(_gridPosition);
    public List<GridPosition> GetTilesInCircle(Vector3 _center, float _radius) => gridSystem.GetTilesInCircle(_center, _radius);
    public GridPosition GetGridPosition(Vector3 _worldPosition) => gridSystem.GetGridPosition(_worldPosition);
    public Vector3 GetWorldPosition(GridPosition _gridPosition) => gridSystem.GetWorldPosition(_gridPosition);
    public GridObject GetGridObject(GridPosition _gridPosition) => gridSystem.GetGridObject(_gridPosition);
    public bool IsValidGridPosition(GridPosition _gridPosition) => gridSystem.IsValidGridPosition(_gridPosition);

    public GridObject GetGridObject(Vector3 _worldPosition)
    {
        return gridSystem.GetGridObject(GetGridPosition(_worldPosition));
    }

    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
    public List<GridPosition> GetGridPositions() => gridSystem.GetGridPositions();
    public void SetNeighbours() => gridSystem.SetNeighbours();
    public List<GridObject> GetNodeGrid() => gridSystem.GetNodeGrid();
}
