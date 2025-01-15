using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool DisplayGridGizmos;
    public LayerMask unwalkableMask;
    LayerMask walkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Node[,] grid;
    public TerrainType[] WalkableRegions;
    public int obstacleProximityPenalty = 10;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach(TerrainType region in WalkableRegions)
        {
            walkableMask.value = walkableMask |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value,2),region.terrainPenalty);
        }

        CreateGrid();
    }

    public int MaxSize
    {
        get { return gridSizeX*gridSizeY; }
    }
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 WorldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = WorldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                int movementPenalty = 0;

                Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, walkableMask))
                {
                    walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                }
                if (!walkable)
                {
                    movementPenalty += obstacleProximityPenalty;
                }


                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }

        BlurPenaltyMap(3);
    }

    void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] penalitiesHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penalitiesVerticalPass = new int[gridSizeX, gridSizeY];

        for(int y = 0; y< gridSizeY; y++)
        {
            for(int x = -kernelExtents; x<= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penalitiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
            }

            for(int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1,0,gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtents,0,gridSizeX-1);
                penalitiesHorizontalPass[x, y] = penalitiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;


            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penalitiesVerticalPass[x, 0] += penalitiesHorizontalPass[x, sampleY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)penalitiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].movementPenalty = blurredPenalty;

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);
                
                penalitiesVerticalPass[x, y] = penalitiesVerticalPass[x, y - 1] - penalitiesHorizontalPass[x, removeIndex] + penalitiesHorizontalPass[x, addIndex];
                
                 blurredPenalty = Mathf.RoundToInt((float)penalitiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid[x, y].movementPenalty = blurredPenalty;

                if (blurredPenalty > penaltyMax)
                {
                    penaltyMax = blurredPenalty;
                }
                if(blurredPenalty < penaltyMin)
                {
                    penaltyMin = blurredPenalty;
                }
            }
        }

    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for(int x = -1; x<= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }

        }

        return neighbors;
    }

    public Node NodeFromWorldPoint (Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.FloorToInt(Mathf.Clamp((gridSizeX) * percentX, 0, gridSizeX - 1));
        int y = Mathf.FloorToInt(Mathf.Clamp((gridSizeY) * percentY, 0, gridSizeY - 1));


        return grid[x, y];

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            Gizmos.color = Color.black;
            Node nPlayer = NodeFromWorldPoint(Player.transform.position);
            Gizmos.DrawCube(nPlayer.WorldPosition, Vector3.one * (nodeDiameter));
        }
        

        if (grid != null && DisplayGridGizmos)
            {
                foreach (Node n in grid)
                {
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                Gizmos.color = (n.Walkable) ? Gizmos.color : Color.red;
                Gizmos.DrawCube(n.WorldPosition - new Vector3(0,-1f,0), new Vector3(nodeDiameter,1, nodeDiameter));
            }
            }
        
    }
}

[System.Serializable]
public class TerrainType
{
    public LayerMask terrainMask;
    public int terrainPenalty;
}

public class Node : IHeapItem<Node>
{
    public bool Walkable;
    public Vector3 WorldPosition;
    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;

    public int gridX;
    public int gridY;
    public int movementPenalty;

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY, int _penalty)
    {
        Walkable = _walkable;
        WorldPosition = _worldPosition;
        gridX = _gridX; 
        gridY = _gridY;
        movementPenalty = _penalty;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
