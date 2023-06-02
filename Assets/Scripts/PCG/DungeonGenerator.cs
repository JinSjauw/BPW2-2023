using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance { get; private set; }
    
    private LevelGrid grid;
    private List<GridPosition> walkableList;
    [SerializeField] private List<Room> roomList;
    private Room startRoom, endRoom;

    [SerializeField] private GameObject walkableTilePrefab;
    [SerializeField] private GameObject tileWallPrefab;
    [SerializeField] private List<GameObject> furniturePrefabsList;

    [SerializeField] private GameObject enemyWarriorPrefab;
    [SerializeField] private GameObject enemyMagePrefab;
    [SerializeField] private GameObject playerPrefab;
    
    private Pathfinding pathfinder;
    private Triangulation triangulation;
    private List<TriangleEdge> dungeonPaths;
    private List<Triangle> delaunayMesh;
    private List<GridPosition> wallList;
    private List<GridObject> hallwayList;

    private int testIndex = 0;

    [Header("Room Data")]
    [SerializeField] private int minRoomWidth;
    [SerializeField] private int minRoomHeight;
    [SerializeField] private int maxRoomWidth;
    [SerializeField] private int maxRoomHeight;
    [SerializeField] private float maxRatio, minRatio;
    [SerializeField] private int minAmountRoom;
    [SerializeField] private int maxAmountRoom;

    [Header("Path Data")] 
    [SerializeField] private int pathLoops;

    [Header("Enemy Data")] 
    [SerializeField] private int maxAmountEnemies;
    [SerializeField] private int minAmountEnemies;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance of DungeonGenerator");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    public void Initialize()
    {
        grid = LevelGrid.Instance;
        //cellList = grid.GetGridPositions();
        
        triangulation = new Triangulation();
        pathfinder = new Pathfinding();
        walkableList = new List<GridPosition>();
        roomList = new List<Room>();
        wallList = new List<GridPosition>();
        hallwayList = new List<GridObject>();
    }

    public List<GridPosition> Generate()
    {
        GenerateRooms();
        GenerateTriangulation();
        MarkPathways();
        MarkWallTiles();
        PopulateRooms();
        RenderDungeon();

        return walkableList;
    }
    
    private void PopulateRooms()
    {
        //spawn player on the startroom position
        Instantiate(playerPrefab, grid.GetWorldPosition(startRoom.roomCenter), Quaternion.identity);
        List<GridPosition> occupiedList = new List<GridPosition>();
        occupiedList.Add(startRoom.roomCenter);
        occupiedList.Add(endRoom.roomCenter);

        foreach (var room in roomList)
        {
            List<GridPosition> roomGrid = room.GetRoomGrid();

            //Spawn furniture
            int furnitureAmount = Random.Range(2, 7);
            for (int i = 0; i <= furnitureAmount;)
            {
                GridPosition gridSpawnPosition = roomGrid[Random.Range(0, roomGrid.Count - 1)];

                Vector3 spawnPosition = grid.GetWorldPosition(gridSpawnPosition);
                GameObject furniturePrefab = furniturePrefabsList[Random.Range(0, furniturePrefabsList.Count - 1)];
                
                if (walkableList.Remove(gridSpawnPosition))
                {
                    Instantiate(furniturePrefab, spawnPosition, Quaternion.identity);
                    occupiedList.Add(gridSpawnPosition);
                    i++;
                }
            }

            if (room == startRoom)
            {
                continue;
            }
            
            //Spawn Enemies
            int enemiesAmount = Random.Range(minAmountEnemies, maxAmountEnemies);
            for (int i = 0; i < enemiesAmount; i++)
            {
                GridPosition gridSpawnPosition = roomGrid[Random.Range(0, roomGrid.Count - 1)];
                TILETYPE tileType = grid.GetGridObject(gridSpawnPosition).tileType;
                if (occupiedList.Contains(gridSpawnPosition) || tileType != TILETYPE.FLOOR)
                {
                    enemiesAmount -= 1;
                    continue;
                }
                
                Vector3 spawnPosition = grid.GetWorldPosition(gridSpawnPosition);
                GameObject enemyPrefab = null;
                int enemyType = Random.Range(0, 10);
                if (enemyType < 7)
                {
                    enemyPrefab = enemyWarriorPrefab;
                }
                else
                {
                    enemyPrefab = enemyMagePrefab;
                }

                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                occupiedList.Add(gridSpawnPosition);
            }
        }
        
        //Spawn a few enemies in the hallways
        foreach (GridObject hallWayTile in hallwayList)
        {
            if (occupiedList.Contains(hallWayTile.gridPosition))
            {
                continue;
            }
            
            int spawnChance = Random.Range(0, 100);
            GameObject enemyPrefab = null;
            if (spawnChance < 5)
            {
                int enemyType = Random.Range(0, 10);
                if (enemyType < 7)
                {
                    enemyPrefab = enemyWarriorPrefab;
                }
                else
                {
                    enemyPrefab = enemyMagePrefab;
                }
                Instantiate(enemyPrefab, hallWayTile.position, Quaternion.identity);
                occupiedList.Add(hallWayTile.gridPosition);
            }
        }
        
    }

    public void GenerateRooms()
    {
        int gridSize = grid.GetWidth();
        int splitAmount = (int)Mathf.Sqrt(gridSize - 2);
        //Split grid into subgrids
        int subGridSize = 9;
        int subGridIncrement = 9;

        if (maxAmountRoom > splitAmount)
        {
            maxAmountRoom = splitAmount - 1;
        } 
        
        int totalAmount = Random.Range(minAmountRoom, maxAmountRoom);
        int currentAmount = 0;
        List<int> occupiedSubGrids = new List<int>();
        
        for (int amount = 0; amount < totalAmount; amount++)
        {
            int subGridIndex = amount + 1;
            occupiedSubGrids.Add(subGridIndex);

            //Get room dimensions
            Vector2 size = GetRatioSize();
            int width = (int)size.x;
            int height = (int)size.y;
            int startX = Random.Range(0, (gridSize - width));
            //int startZ = Random.Range(0, subGridIncrement);
            GridPosition startPosition = new GridPosition();
            List<GridPosition> roomPositions = new List<GridPosition>();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition roomTile = new GridPosition(x + startX,
                       z + (subGridIndex * subGridIncrement));
                    if (x == 0 && z == 0)
                    {
                        startPosition = roomTile;
                    }
                    roomPositions.Add(roomTile);
                    walkableList.Add(roomTile);
                    GridObject gridObject = grid.GetGridObject(roomTile);
                    gridObject.SetTileType(TILETYPE.FLOOR);
                }
            }

            //Get center
            GridPosition center = new GridPosition(startPosition.x + width / 2, startPosition.z + height / 2);
            
            //Get tiles for roomGrid
            Room room = new Room(amount, width, height, center, ROOMTYPE.NORMAL, roomPositions);
            if (amount == 0)
            {
                room.roomType = ROOMTYPE.START;
                startRoom = room;
            }
            
            roomList.Add(room);
        }
        
        Room furthestRoom = roomList[0];
        foreach (var room in roomList)
        {
            float distanceA = Vector3.Distance(grid.GetWorldPosition(startRoom.roomCenter),
                grid.GetWorldPosition(room.roomCenter));
            float distanceB = Vector3.Distance(grid.GetWorldPosition(startRoom.roomCenter),
                grid.GetWorldPosition(furthestRoom.roomCenter));
            if (distanceA > distanceB)
            {
                furthestRoom = room;
            }
        }
        endRoom = furthestRoom;
    }
    
    public void GenerateTriangulation()
    {
        //Get list of roomCenters
        List<GridPosition> gridPositions = new List<GridPosition>();

        foreach (var room in roomList)
        {
            gridPositions.Add(room.roomCenter);
        }
        
        //Convert List<GridPositions> into List<Vector3> containing the world positions
        List<Vector3> vertices = new List<Vector3>();
        foreach (var position in gridPositions)
        {
            Vector3 gridToVert = grid.GetWorldPosition(position);
            vertices.Add(gridToVert);
        }

        delaunayMesh = triangulation.Triangulate(vertices);
        dungeonPaths = GeneratePath(delaunayMesh);
        
    }

    public List<TriangleEdge> GeneratePath(List<Triangle> _delaunayMesh)
    {
        List<TriangleEdge> edges = new List<TriangleEdge>();
        List<Vector3> vertices = new List<Vector3>();
        //Convert Triangle to Edge
        foreach (var triangle in _delaunayMesh)
        {
            if (!vertices.Contains(triangle.vertexA))
            {
                vertices.Add(triangle.vertexA);
            }
            if (!vertices.Contains(triangle.vertexB))
            {
                vertices.Add(triangle.vertexB);
            }
            if (!vertices.Contains(triangle.vertexC))
            {
                vertices.Add(triangle.vertexC);
            }
            
            if(!edges.Contains(new TriangleEdge(triangle.vertexA, triangle.vertexB)))
            {
                edges.Add(new TriangleEdge(triangle.vertexA, triangle.vertexB));
            }

            if (!edges.Contains(new TriangleEdge(triangle.vertexB, triangle.vertexC)))
            {
                edges.Add(new TriangleEdge(triangle.vertexB, triangle.vertexC));
            }

            if (!edges.Contains(new TriangleEdge(triangle.vertexC, triangle.vertexA)))
            {
                edges.Add(new TriangleEdge(triangle.vertexC, triangle.vertexA));
            }
        }

        List<Vector3> visitedList = new List<Vector3>();
        List<TriangleEdge> reachableList = new List<TriangleEdge>();
        List<TriangleEdge> path = new List<TriangleEdge>();
        //PRIM'S Algorithmn 
        //Pick Start Node - Random
        
        visitedList.Add(vertices[Random.Range(0, vertices.Count - 1)]);
        
        foreach (var edge in edges)
        {
            if (edge.vertexA.Equals(visitedList[0]))
            {
                reachableList.Add(edge);
            }
            else if (edge.vertexB.Equals(visitedList[0]))
            {
                reachableList.Add(edge);
            }
        }

        while (visitedList.Count < vertices.Count)
        {
            //Find minimum Edge
            TriangleEdge minimumEdge = reachableList[0];

            foreach (var edge in reachableList)
            {
                if (visitedList.Contains(edge.vertexA) && visitedList.Contains(edge.vertexB))
                {
                    continue;
                }
                
                float distanceA = edge.Length();
                float distanceB = minimumEdge.Length();
                if (distanceA < distanceB)
                {
                    minimumEdge = edge;
                }
            }
            
            path.Add(minimumEdge);
            
            //Add edges connected to minimumEdge
            foreach (var edge in edges)
            {
                if (edge.Equals(minimumEdge))
                {
                    continue;
                }
                
                if (edge.vertexA.Equals(minimumEdge.vertexA) || edge.vertexA.Equals(minimumEdge.vertexB) || 
                    edge.vertexB.Equals(minimumEdge.vertexA) || edge.vertexB.Equals(minimumEdge.vertexB))
                {
                    if (!reachableList.Contains(edge))
                    {
                        reachableList.Add(edge);   
                    }
                }
            }

            if (visitedList.Contains(minimumEdge.vertexA))
            {
                visitedList.Add(minimumEdge.vertexB);
            }
            else if (visitedList.Contains(minimumEdge.vertexB))
            {
                visitedList.Add(minimumEdge.vertexA);
            }
            else
            {
                continue;
            }
            
            reachableList.Remove(minimumEdge);
        }

        if (pathLoops > 0)
        {
            //Sort edges by longest length
            List<TriangleEdge> sortedEdges = edges.OrderByDescending((e) => e.Length()).ToList();

            for (int i = 0; i < pathLoops;)
            {
                //Add paths back, preference for longer ones
                TriangleEdge edge = sortedEdges[Random.Range(0, sortedEdges.Count - (sortedEdges.Count / 3))];
                if (!path.Contains(edge))
                {
                    path.Add(edge);
                    i++;
                }
            }
        }

        return path;
    }

    private void MarkPathways()
    {
        pathfinder.SetGrid(grid.GetGridPositions());
        
        foreach (TriangleEdge edge in dungeonPaths)
        {
            List<GridPosition> path = new List<GridPosition>();
            
            GridObject start = grid.GetGridObject(grid.GetGridPosition(edge.vertexA));
            GridObject end = grid.GetGridObject(grid.GetGridPosition(edge.vertexB));
            
            path = pathfinder.FindPath(start, end);

            for (int i = 0; i < path.Count; i++)
            {
                GridObject tile = LevelGrid.Instance.GetGridObject(path[i]);
                
                tile.SetTileType(TILETYPE.HALLWAY);
                hallwayList.Add(tile);
                if (!walkableList.Contains(tile.gridPosition))
                {
                    walkableList.Add(tile.gridPosition);
                }
            }

            List<GridObject> neighBours = new List<GridObject>();
            foreach (GridObject hallway in hallwayList)
            {
                foreach (GridObject neighbour in hallway.neighbourList)
                {
                    if (neighbour.tileType == TILETYPE.EMPTY || neighbour.tileType == TILETYPE.WALL)
                    {
                        neighBours.Add(neighbour);
                    }
                }
            }

            List<GridObject> hallWayTiles = neighBours.Distinct().ToList();
             
            foreach (GridObject tile in hallWayTiles)
            {
                tile.SetTileType(TILETYPE.HALLWAY);
                if (!walkableList.Contains(tile.gridPosition))
                {
                    walkableList.Add(tile.gridPosition);
                }
            }

            foreach (GridObject tile in hallWayTiles)
            {
                foreach (GridObject neighbour in tile.neighbourList)
                {
                    if (neighbour.tileType == TILETYPE.EMPTY)
                    {
                        neighbour.SetTileType(TILETYPE.WALL);
                        wallList.Add(neighbour.gridPosition);
                        //walkableList.Add(neighbour.gridPosition);
                    }
                }
            }
            
        }
    }
    
    public void MarkWallTiles()
    {
        foreach (var room in roomList)
        {
            List<GridPosition> roomGrid = room.GetRoomGrid();

            foreach (var roomTile in roomGrid)
            {
                GridObject gridTile = grid.GetGridObject(roomTile);

                foreach (GridObject tile in gridTile.neighbourList)
                {
                    if (tile.tileType == TILETYPE.EMPTY)
                    {
                        tile.SetTileType(TILETYPE.WALL);
                        //Debug.Log("Setting Wall: " + gridTile.gridPosition);
                        wallList.Add(tile.gridPosition);
                        //walkableList.Add(tile.gridPosition);
                    }
                }
            }
        }
    }

    public void RenderDungeon()
    {
        List<GridPosition> verifiedWallList = wallList.Where((x) => !walkableList.Contains(x)).ToList();

        foreach (GridPosition tile in walkableList)
        {
            Vector3 worldPosition = grid.GetWorldPosition(tile);

            Instantiate(walkableTilePrefab, worldPosition, quaternion.identity);
        }

        foreach (GridPosition wallTile in verifiedWallList)
        {
            Vector3 worldPosition = grid.GetWorldPosition(wallTile);
            Instantiate(tileWallPrefab, worldPosition, Quaternion.identity);
        }
    }

    public List<GridPosition> GenerateTutorial(List<Transform> _floorPositions)
    {
        List<GridObject> floorsList = new List<GridObject>();
        foreach (Transform floorPosition in _floorPositions)
        {
            GridObject floor = grid.GetGridObject(grid.GetGridPosition(floorPosition.position));
            floor.SetTileType(TILETYPE.FLOOR);
            floorsList.Add(floor);
            walkableList.Add(floor.gridPosition);
        }
        
        //Mark walls
        foreach (GridObject floor in floorsList)
        {
            foreach (GridObject neighbour in floor.neighbourList)
            {
                if (neighbour.tileType == TILETYPE.EMPTY)
                {
                    neighbour.SetTileType(TILETYPE.WALL);
                    wallList.Add(neighbour.gridPosition);
                }
            }
        }
        
        
        RenderDungeon();

        return walkableList;
    }

    public Vector2 GetRatioSize()
    {
        int width = 0;
        int height = 0;
        
        float ratio = Mathf.Infinity;
        
        while (ratio > maxRatio || ratio < minRatio)
        {
            width = Random.Range(minRoomWidth, maxRoomWidth);
            height = Random.Range(minRoomHeight, maxRoomHeight);

            int gcd = GCD(width, height);

            ratio = ((float)width / gcd) / ((float)height / gcd);
        }

        return new Vector2(width, height);
    }

    private int GCD(int a, int b)
    {
        return b == 0 ? Math.Abs(a) : GCD(b, a % b);
    }

    private void OnDrawGizmos()
    {
        if (delaunayMesh == null)
        {
            return;
        }
        
        foreach (var triangle in delaunayMesh)
        {
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawLine(triangle.vertexA, triangle.vertexB);
            Gizmos.DrawLine(triangle.vertexB, triangle.vertexC);
            Gizmos.DrawLine(triangle.vertexC, triangle.vertexA);
        }
        
        if (dungeonPaths == null)
        {
            return;
        }
        
        foreach (var edge in dungeonPaths)
        {
            Gizmos.color = new Color(0, 1, 0, 1);
            Gizmos.DrawLine(edge.vertexA + new Vector3(0, 1, 0), edge.vertexB + new Vector3(0, 1 ,0));
        }

        /*for (int i = 0; i < hallWays.Count - 1; i++)
        {
            Gizmos.color = new Color(1, 0, 0, 1);
            Gizmos.DrawLine(hallWays[i].position, hallWays[i + 1].position);
        }*/
    }
}
