using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    private LevelGrid grid;
    [SerializeField] private List<GridPosition> closedList, openList;
    [SerializeField] private List<Room> roomList;
    [SerializeField] private GameObject tilePrefab;
    
    [Header("Room Data")]
    [SerializeField] private int minRoomWidth;
    [SerializeField] private int minRoomHeight;
    [SerializeField] private int maxRoomWidth;
    [SerializeField] private int maxRoomHeight;
    [SerializeField] private float maxRatio, minRatio;
    [SerializeField] private int minAmountRoom;
    [SerializeField] private int maxAmountRoom;
    

    // Start is called before the first frame update
    void Start()
    {
        grid = LevelGrid.Instance;
        openList = grid.GetGridPositions();
        
        closedList = new List<GridPosition>();
        roomList = new List<Room>();
    }

    public void SpawnRooms()
    {
        int totalAmount = Random.Range(minAmountRoom, maxAmountRoom);
        int currentAmount = 0;
        bool roomsSpawned = false;
        
        while (!roomsSpawned)
        {
            LoopStart:
            
            if (openList.Count <= 0)
            {
                Debug.Log("OpenList is empty");
                break;
            }
            
            if (currentAmount < totalAmount)
            {
                //Pick a position
                int positionIndex = Random.Range(0, openList.Count - 1);
                GridPosition gridPosition = openList[positionIndex];

                Vector2 size = GetRatioSize();

                int width = (int)size.x;
                int height = (int)size.y;
                
                //Look in the grid.   
                //Verify if the rooms fits on the grid
                GridPosition roomSize = new GridPosition(gridPosition.x + width, gridPosition.z + height);
                
                if (openList.Contains(roomSize));
                {
                    List<GridPosition> roomPositions = new List<GridPosition>();
                    
                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < height; z++)
                        {
                            GridPosition roomTile = new GridPosition(gridPosition.x + x, gridPosition.z + z);

                            if (openList.Contains(roomTile))
                            {
                                roomPositions.Add(roomTile);
                                openList.Remove(roomTile);
                                
                                //Ensure 1 tile space between rooms
                                //Width spacing
                                if (x == 0)
                                {
                                    if (openList.Contains(new GridPosition(gridPosition.x - 1, gridPosition.z + z)))
                                    {
                                        GridPosition spacing = new GridPosition(gridPosition.x - 1, gridPosition.z + z);
                                        openList.Remove(spacing);
                                        Debug.Log("Removing negative width at:" + " X: " + (gridPosition.x - 1) + " Y: " + (gridPosition.z + 1));

                                    }
                                }

                                if (x == width - 1)
                                {
                                    if (openList.Contains(new GridPosition(gridPosition.x + x + 1, gridPosition.z + z)))
                                    {
                                        GridPosition spacing = new GridPosition(gridPosition.x + x + 1, gridPosition.z + z);
                                        openList.Remove(spacing);
                                        Debug.Log("Removing positive width at:" + " X: " + (gridPosition.x + x + 1) + " Y: " + gridPosition.z + z);

                                    }
                                }
                                
                                //Height spacing
                                if (z == 0)
                                {
                                    if (openList.Contains(new GridPosition(gridPosition.x + x, gridPosition.z - 1)))
                                    {
                                        GridPosition spacing = new GridPosition(gridPosition.x + x, gridPosition.z - 1);
                                        openList.Remove(spacing);
                                        Debug.Log("Removing negative height at:" + " X: " + (gridPosition.x + x) + " Y: " + (gridPosition.z - 1));

                                    }
                                }
                                
                                if (z == height - 1)
                                {
                                    if (openList.Contains(new GridPosition(gridPosition.x + x, gridPosition.z + z + 1)))
                                    {
                                        GridPosition spacing = new GridPosition(gridPosition.x + x, gridPosition.z + z + 1);
                                        openList.Remove(spacing);
                                        Debug.Log("Removing positive height at:" + " X: " + (gridPosition.x + x) + " Y: " + gridPosition.z + 1 + 1);
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
                    roomList.Add(new Room(currentAmount, width, height, center,ROOMTYPE.NORMAL, roomPositions));
                    currentAmount++;
                }
            }
            else
            {
                roomsSpawned = true;
            }
        }
        
        //Render Rooms
        foreach (var room in roomList)
        {
            List<GridPosition> roomGrid = room.GetRoomGrid();

            foreach (var roomTile in roomGrid)
            {
                Instantiate(tilePrefab, grid.GetWorldPosition(roomTile), Quaternion.identity);
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
    
    
}
