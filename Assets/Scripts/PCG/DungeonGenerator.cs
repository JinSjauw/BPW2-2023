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
    [SerializeField] private List<GridPosition> cellList, walkableList;
    [SerializeField] private List<Room> roomList;
    [SerializeField] private Room startRoom, endRoom;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileWallPrefab;
    [SerializeField] private List<GameObject> furniturePrefabsList;

    [SerializeField] private GameObject enemyWarriorPrefab;
    [SerializeField] private GameObject enemyMagePrefab;
    [SerializeField] private GameObject playerPrefab;
    
    private Pathfinding pathfinder;
    private Triangulation triangulation;
    private List<TriangleEdge> dungeonPaths;
    private List<Triangle> delaunayMesh;
    private List<GridObject> hallWays;

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
        cellList = grid.GetGridPositions();
        
        triangulation = new Triangulation();
        pathfinder = new Pathfinding();
        roomList = new List<Room>();
        hallWays = new List<GridObject>();
    }

    public List<GridPosition> Generate()
    {
        GenerateRooms();
        GenerateTriangulation();
        GenerateHallways();
        GenerateWalls();
        PopulateRooms();

        return walkableList;
    }

    private void PopulateRooms()
    {
        //spawn player on the startroom position
        Instantiate(playerPrefab, grid.GetWorldPosition(startRoom.roomCenter), Quaternion.identity);
       
        foreach (var room in roomList)
        {
            List<GridPosition> occupiedList = new List<GridPosition>();
            List<GridPosition> roomGrid = room.GetRoomGrid();
            
            //Spawn furniture
            int furnitureAmount = Random.Range(2, 7);
            for (int i = 0; i < furnitureAmount;)
            {
                GridPosition gridSpawnPosition = roomGrid[Random.Range(0, roomGrid.Count - 1)];
                TILETYPE tileType = grid.GetGridObject(gridSpawnPosition).tileType;
                if (occupiedList.Contains(gridSpawnPosition) || tileType != TILETYPE.FLOOR)
                {
                    continue;
                }
                
                Vector3 spawnPosition = grid.GetWorldPosition(gridSpawnPosition);
                GameObject furniturePrefab = furniturePrefabsList[Random.Range(0, furniturePrefabsList.Count - 1)];

                Instantiate(furniturePrefab, spawnPosition, Quaternion.identity);
                occupiedList.Add(gridSpawnPosition);
                walkableList.Remove(gridSpawnPosition);
                
                i++;
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
    }

    public void GenerateRooms()
    {
        int totalAmount = Random.Range(minAmountRoom, maxAmountRoom);
        int currentAmount = 0;
        bool roomsSpawned = false;
        
        //Trim Cell List edges

        List<GridPosition> temp = new List<GridPosition>(cellList);
        foreach (var tile in cellList)
        {
            if (tile.x == 0 )
            {
                temp.Remove(tile);
            }

            if (tile.x == grid.GetWidth())
            {
                temp.Remove(tile);
            }

            if (tile.z == 0)
            {
                temp.Remove(tile);
            }

            if (tile.z == grid.GetHeight())
            {
                temp.Remove(tile);
            }
        }

        cellList = new List<GridPosition>(temp);

        while (!roomsSpawned)
        {
            LoopStart:
            
            if (cellList.Count <= 0)
            {
                Debug.Log("OpenList is empty");
                break;
            }
            
            if (currentAmount < totalAmount)
            {
                //Pick a position
                int positionIndex = Random.Range(0, cellList.Count - 1);
                GridPosition gridPosition = cellList[positionIndex];

                Vector2 size = GetRatioSize();

                int width = (int)size.x;
                int height = (int)size.y;
                
                //Look in the grid.   
                //Verify if the rooms fits on the grid
                GridPosition roomSizePos = new GridPosition(gridPosition.x + width + 2, gridPosition.z + height + 2);
                GridPosition roomSizeMin = new GridPosition(gridPosition.x - 2, gridPosition.z - 2);

                
                if (cellList.Contains(roomSizePos) && cellList.Contains(roomSizeMin));
                {
                    List<GridPosition> roomPositions = new List<GridPosition>();
                    
                    for (int x = -2; x < width + 2; x++)
                    {
                        for (int z = -2; z < height + 2; z++)
                        {
                            GridPosition roomTile = new GridPosition(gridPosition.x + x, gridPosition.z + z);

                            if (cellList.Contains(roomTile))
                            {
                                if (x >= 0 && z >= 0 && x <= width && z <= height)
                                {
                                    roomPositions.Add(roomTile);
                                }
                                
                                cellList.Remove(roomTile);
                            }
                            else
                            {
                                //Debug.Log("openList didn't contain element.");
                                goto LoopStart;
                            }
                        }
                    }
                    
                    GridPosition center = new GridPosition(gridPosition.x + (width / 2), gridPosition.z + (height / 2));
                    Room room = new Room(currentAmount, width, height, center, ROOMTYPE.NORMAL, roomPositions);
                    if (currentAmount == 0)
                    {
                        startRoom = room;
                        startRoom.roomType = ROOMTYPE.START;
                    }
                    roomList.Add(room);
                    currentAmount++;
                }
            }
            else
            {
                roomsSpawned = true;
            }
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
        endRoom.roomType = ROOMTYPE.END;
        
        //Render Rooms
        foreach (var room in roomList)
        {
            List<GridPosition> roomGrid = room.GetRoomGrid();

            foreach (var roomTile in roomGrid)
            {
                Instantiate(tilePrefab, grid.GetWorldPosition(roomTile), Quaternion.identity);
                grid.GetGridObject(roomTile).SetTileType(TILETYPE.FLOOR);
                walkableList.Add(roomTile);
            }
        }
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
            for (int i = 0; i < pathLoops;)
            {
                TriangleEdge edge = edges[Random.Range(0, edges.Count - 1)];
                if (!path.Contains(edge))
                {
                    path.Add(edge);
                    i++;
                }
           
            }
        }
        
        return path;
    }

    public void GenerateHallways()
    {
        pathfinder.SetGrid(grid.GetGridPositions());

        foreach (var edge in dungeonPaths)
        {
            List<GridPosition> path = new List<GridPosition>();
            
            GridObject start = grid.GetGridObject(grid.GetGridPosition(edge.vertexA));
            GridObject end = grid.GetGridObject(grid.GetGridPosition(edge.vertexB));
            
            path = pathfinder.FindPath(start, end);

            //Assign correct tileTypes to path
            GridObject lastTile = null;
            GridObject nextTile = null;

            for (int i = 0; i < path.Count; i++)
            {
                GridObject tile = LevelGrid.Instance.GetGridObject(path[i]);

                if (i - 1 > 0)
                {
                    lastTile = grid.GetGridObject(path[i - 1]);
                }

                if (i + 1 < path.Count - 1)
                {
                    nextTile = grid.GetGridObject(path[i + 1]);
                }

                if (tile.tileType == TILETYPE.EMPTY)
                {
                    //Is it a doorway?
                    //Last tile was a floor?
                    if (lastTile != null && lastTile.tileType == TILETYPE.FLOOR)
                    {
                        //Yes?
                        //Set it as doorway
                        tile.SetTileType(TILETYPE.DOORWAY);
                        foreach (var neighbour in lastTile.neighbourList)
                        {
                            neighbour.SetTileType(TILETYPE.DOORWAY);
                        }
                    }
                    else
                    {
                        //No?
                        //Spawn Hallway!
                        tile.SetTileType(TILETYPE.HALLWAY);
                        foreach (var neighbour in tile.neighbourList)
                        {
                            if (neighbour.tileType == TILETYPE.EMPTY)
                            {
                                neighbour.SetTileType(TILETYPE.HALLWAY);
                            }
                        }
                    }
                }

                if (tile.tileType == TILETYPE.HALLWAY)
                {
                    if (nextTile != null)
                    {
                        if (nextTile.tileType == TILETYPE.FLOOR)
                        {
                            tile.SetTileType(TILETYPE.DOORWAY);
                            foreach (var neighbour in nextTile.neighbourList)
                            {
                                neighbour.SetTileType(TILETYPE.DOORWAY);
                            }
                        }
                    }
                }
                
                lastTile = tile;
            }

            for (int i = 0; i < path.Count; i++ )
            {
                GridObject tile = grid.GetGridObject(path[i]);
                Instantiate(tilePrefab, tile.position, quaternion.identity);
                
                foreach (var neighbour in tile.neighbourList)
                {
                    Instantiate(tilePrefab, neighbour.position, quaternion.identity);
                    walkableList.Add(neighbour.gridPosition);
                }
            }
        }
        GenerateWalls();
    }

    public void GenerateWalls()
    {
        foreach (var room in roomList)
        {
            List<GridPosition> roomGrid = room.GetRoomGrid();

            foreach (var roomTile in roomGrid)
            {
                GridObject tile = grid.GetGridObject(roomTile);
                
                hallWays.Add(tile);
            }
        }

        foreach (var tile in hallWays)
        {
            TILETYPE filterType = TILETYPE.EMPTY;
            
            if (tile.tileType == TILETYPE.FLOOR)
            {
                filterType = TILETYPE.HALLWAY;
            }

            if (tile.tileType == TILETYPE.DOORWAY)
            {
                continue;
            }
            
            foreach (var neighbour in tile.neighbourList)
            {
                if (neighbour.tileType == filterType || neighbour.tileType == TILETYPE.EMPTY)
                {
                    //Check the neighbour state;
                        
                    if(neighbour.gridPosition == new GridPosition(tile.gridPosition.x - 1, tile.gridPosition.z))
                    {
                        //Left
                        Instantiate(tileWallPrefab, neighbour.position, Quaternion.Euler(0, 90, 0));
                        neighbour.SetTileType(TILETYPE.WALL);
                        walkableList.Remove(neighbour.gridPosition);
                    }
                        
                    if(neighbour.gridPosition == new GridPosition(tile.gridPosition.x + 1 , tile.gridPosition.z))
                    {
                        //Right
                        Instantiate(tileWallPrefab, neighbour.position, Quaternion.Euler(0, 270, 0));
                        neighbour.SetTileType(TILETYPE.WALL);
                        walkableList.Remove(neighbour.gridPosition);

                    }
                        
                    if(neighbour.gridPosition == new GridPosition(tile.gridPosition.x, tile.gridPosition.z + 1))
                    {
                        //Up
                        Instantiate(tileWallPrefab, neighbour.position, Quaternion.Euler(0, 180, 0));
                        neighbour.SetTileType(TILETYPE.WALL);
                        walkableList.Remove(neighbour.gridPosition);
                    }
                        
                    if(neighbour.gridPosition == new GridPosition(tile.gridPosition.x, tile.gridPosition.z - 1))
                    {
                        //Down
                        Instantiate(tileWallPrefab, neighbour.position, Quaternion.Euler(0, 0, 0));
                        neighbour.SetTileType(TILETYPE.WALL);
                        walkableList.Remove(neighbour.gridPosition);
                    }
                }
            }
        }
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
            //Debug.Log("The Ratio between" + width + " : " + height + " Ratio: " + ratio);
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
