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
    private LevelGrid grid;
    [SerializeField] private List<GridPosition> cellList;
    [SerializeField] private List<Room> roomList;
    Room startRoom, endRoom;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileWallPrefab;

    
    private Triangulation triangulation;
    [SerializeField] private List<Edge> dungeonPaths;
    [SerializeField] private List<Triangle> delaunayMesh;
    [SerializeField] private List<GridObject> hallWays;

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
    private Pathfinding pathfinder;

    // Start is called before the first frame update
    void Start()
    {
        grid = LevelGrid.Instance;
        cellList = grid.GetGridPositions();
        triangulation = new Triangulation();
        pathfinder = new Pathfinding();
        roomList = new List<Room>();
        hallWays = new List<GridObject>();
    }

    public void SpawnRooms()
    {
        int totalAmount = Random.Range(minAmountRoom, maxAmountRoom);
        int currentAmount = 0;
        bool roomsSpawned = false;

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
                GridPosition roomSize = new GridPosition(gridPosition.x + width, gridPosition.z + height);
                
                if (cellList.Contains(roomSize));
                {
                    List<GridPosition> roomPositions = new List<GridPosition>();
                    
                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < height; z++)
                        {
                            GridPosition roomTile = new GridPosition(gridPosition.x + x, gridPosition.z + z);

                            if (cellList.Contains(roomTile))
                            {
                                roomPositions.Add(roomTile);
                                cellList.Remove(roomTile);
                                
                                //Ensure 1 tile space between rooms
                                //Width spacing
                                if (x == 0)
                                {
                                    if (cellList.Contains(new GridPosition(gridPosition.x - 2, gridPosition.z + z)))
                                    {
                                        GridPosition spacing = new GridPosition(gridPosition.x - 2, gridPosition.z + z);
                                        cellList.Remove(spacing);
                                        Debug.Log("Removing negative width at:" + " X: " + (gridPosition.x - 1) + " Y: " + (gridPosition.z + 1));

                                    }
                                }

                                if (x == width - 2)
                                {
                                    if (cellList.Contains(new GridPosition(gridPosition.x + x + 2, gridPosition.z + z)))
                                    {
                                        GridPosition spacing = new GridPosition(gridPosition.x + x + 2, gridPosition.z + z);
                                        cellList.Remove(spacing);
                                        Debug.Log("Removing positive width at:" + " X: " + (gridPosition.x + x + 2) + " Y: " + gridPosition.z + z);

                                    }
                                }
                                
                                //Height spacing
                                if (z == 0)
                                {
                                    if (cellList.Contains(new GridPosition(gridPosition.x + x, gridPosition.z - 2)))
                                    {
                                        GridPosition spacing = new GridPosition(gridPosition.x + x, gridPosition.z - 2);
                                        cellList.Remove(spacing);
                                        //Debug.Log("Removing negative height at:" + " X: " + (gridPosition.x + x) + " Y: " + (gridPosition.z - 2));

                                    }
                                }
                                
                                if (z == height - 2)
                                {
                                    if (cellList.Contains(new GridPosition(gridPosition.x + x, gridPosition.z + z + 2)))
                                    {
                                        GridPosition spacing = new GridPosition(gridPosition.x + x, gridPosition.z + z + 2);
                                        cellList.Remove(spacing);
                                        //Debug.Log("Removing positive height at:" + " X: " + (gridPosition.x + x) + " Y: " + gridPosition.z + 1 + 1);
                                    }
                                }
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

        Room furthestRoom = new Room();
        furthestRoom.roomCenter = new GridPosition(int.MaxValue, int.MaxValue);
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
        
        //Render Rooms
        foreach (var room in roomList)
        {
            List<GridPosition> roomGrid = room.GetRoomGrid();

            foreach (var roomTile in roomGrid)
            {
                Instantiate(tilePrefab, grid.GetWorldPosition(roomTile), Quaternion.identity);
                grid.GetGridObject(roomTile).SetTileType(TILETYPE.FLOOR);
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

    public List<Edge> GeneratePath(List<Triangle> _delaunayMesh)
    {
        List<Edge> edges = new List<Edge>();
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
            
            if(!edges.Contains(new Edge(triangle.vertexA, triangle.vertexB)))
            {
                edges.Add(new Edge(triangle.vertexA, triangle.vertexB));
            }

            if (!edges.Contains(new Edge(triangle.vertexB, triangle.vertexC)))
            {
                edges.Add(new Edge(triangle.vertexB, triangle.vertexC));
            }

            if (!edges.Contains(new Edge(triangle.vertexC, triangle.vertexA)))
            {
                edges.Add(new Edge(triangle.vertexC, triangle.vertexA));
            }
        }

        List<Vector3> visitedList = new List<Vector3>();
        List<Edge> reachableList = new List<Edge>();
        List<Edge> path = new List<Edge>();
        //PRIM'S Algorithmn 
        //Pick Start Node - Random
        
        visitedList.Add(vertices[Random.Range(0, vertices.Count - 1)]);
        
        foreach (var edge in edges)
        {
            if (edge.vertexA.Equals(visitedList.First()))
            {
                reachableList.Add(edge);
            }
            else if (edge.vertexB.Equals(visitedList.First()))
            {
                reachableList.Add(edge);
            }
        }
        
        while (visitedList.Count <= vertices.Count)
        {
            //Find minimum Edge
            Edge minimumEdge = reachableList[0];
            
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
            
            reachableList.Remove(minimumEdge);
            path.Add(minimumEdge);
        }
        
        //Add Back a few Edges
        if (path.Count == edges.Count)
        {
            return path;
        }
        
        for (int i = 0; i < pathLoops;)
        {
            Edge edge = edges[Random.Range(0, edges.Count - 1)];
            if (!path.Contains(edge))
            {
                path.Add(edge);
                i++;
            }
           
        }
        
        return path;
    }

    public void SpawnHallways()
    {
        foreach (var edge in dungeonPaths)
        {
            List<GridObject> path = new List<GridObject>();
            
            GridObject start = grid.GetGridObject(grid.GetGridPosition(edge.vertexA));
            GridObject end = grid.GetGridObject(grid.GetGridPosition(edge.vertexB));

            bool foundDoorway = false;
            
            path = pathfinder.FindPath(start, end);

            foreach (var tile in path)
            {
                if (tile.tileType == TILETYPE.FLOOR || tile.tileType == TILETYPE.WALL || tile.tileType == TILETYPE.DOORWAY)
                {
                    if (!foundDoorway)
                    {
                        tile.SetTileType(TILETYPE.DOORWAY);

                        foreach (var neighBour in tile.neighbourList)
                        {
                            neighBour.SetTileType(TILETYPE.DOORWAY);
                        }
                        foundDoorway = true;
                    }
                    
                    continue;
                }
                
                tile.SetTileType(TILETYPE.HALLWAY);
                Instantiate(tilePrefab, tile.position, quaternion.identity);
                
                foreach (var neighbour in tile.neighbourList)
                {
                    if (neighbour.tileType == TILETYPE.EMPTY)
                    {
                        neighbour.SetTileType(TILETYPE.HALLWAY);
                        Instantiate(tilePrefab, neighbour.position, quaternion.identity);
                    }
                }
            }
        }
        
        SpawnWalls();
    }

    public void SpawnWalls()
    {
        foreach (var room in roomList)
        {
            List<GridPosition> roomGrid = room.GetRoomGrid();

            foreach (var roomTile in roomGrid)
            {
                GridObject tile = grid.GetGridObject(roomTile);
                
                hallWays.Add(tile);

                /*foreach (var neighbour in tile.neighbourList)
                {
                    if (neighbour.tileType == TILETYPE.EMPTY)
                    {
                        //Check the neighbour state;
                        
                        if(neighbour.gridPosition == new GridPosition(tile.gridPosition.x - 1, tile.gridPosition.z))
                        {
                           //Left
                           Instantiate(tileWallPrefab, neighbour.position, Quaternion.Euler(0, 90, 0));
                           neighbour.SetTileType(TILETYPE.WALL);
                        }
                        
                        if(neighbour.gridPosition == new GridPosition(tile.gridPosition.x + 1 , tile.gridPosition.z))
                        {
                            //Right
                            Instantiate(tileWallPrefab, neighbour.position, Quaternion.Euler(0, 270, 0));
                            neighbour.SetTileType(TILETYPE.WALL);
                        }
                        
                        if(neighbour.gridPosition == new GridPosition(tile.gridPosition.x, tile.gridPosition.z + 1))
                        {
                            //Up
                            Instantiate(tileWallPrefab, neighbour.position, Quaternion.Euler(0, 180, 0));
                            neighbour.SetTileType(TILETYPE.WALL);
                        }
                        
                        if(neighbour.gridPosition == new GridPosition(tile.gridPosition.x, tile.gridPosition.z - 1))
                        {
                            //Down
                            Instantiate(tileWallPrefab, neighbour.position, Quaternion.Euler(0, 0, 0));
                            neighbour.SetTileType(TILETYPE.WALL);
                        }
                    }
                }*/
            }
        }

        foreach (var tile in hallWays)
        {
            foreach (var neighbour in tile.neighbourList)
            {
                if (neighbour.tileType == TILETYPE.EMPTY)
                {
                    //Check the neighbour state;
                        
                    if(neighbour.gridPosition == new GridPosition(tile.gridPosition.x - 1, tile.gridPosition.z))
                    {
                        //Left
                        Instantiate(tileWallPrefab, neighbour.position, Quaternion.Euler(0, 90, 0));
                        neighbour.SetTileType(TILETYPE.WALL);
                    }
                        
                    if(neighbour.gridPosition == new GridPosition(tile.gridPosition.x + 1 , tile.gridPosition.z))
                    {
                        //Right
                        Instantiate(tileWallPrefab, neighbour.position, Quaternion.Euler(0, 270, 0));
                        neighbour.SetTileType(TILETYPE.WALL);
                    }
                        
                    if(neighbour.gridPosition == new GridPosition(tile.gridPosition.x, tile.gridPosition.z + 1))
                    {
                        //Up
                        Instantiate(tileWallPrefab, neighbour.position, Quaternion.Euler(0, 180, 0));
                        neighbour.SetTileType(TILETYPE.WALL);
                    }
                        
                    if(neighbour.gridPosition == new GridPosition(tile.gridPosition.x, tile.gridPosition.z - 1))
                    {
                        //Down
                        Instantiate(tileWallPrefab, neighbour.position, Quaternion.Euler(0, 0, 0));
                        neighbour.SetTileType(TILETYPE.WALL);
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
        
        /*foreach (var triangle in delaunayMesh)
        {
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawLine(triangle.vertexA, triangle.vertexB);
            Gizmos.DrawLine(triangle.vertexB, triangle.vertexC);
            Gizmos.DrawLine(triangle.vertexC, triangle.vertexA);
        }*/
        
        if (dungeonPaths == null)
        {
            return;
        }
        
        foreach (var edge in dungeonPaths)
        {
            Gizmos.color = new Color(0, 1, 0, 1);
            Gizmos.DrawLine(edge.vertexA, edge.vertexB);
        }

        for (int i = 0; i < hallWays.Count - 1; i++)
        {
            Gizmos.color = new Color(1, 0, 0, 1);
            Gizmos.DrawLine(hallWays[i].position, hallWays[i + 1].position);
        }
    }
}
