using System.Collections.Generic;
using UnityEngine;

public class RoomGridPathfinder : MonoBehaviour
{
    private static readonly Vector2Int[] CardinalDirections =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    [SerializeField] private Vector2Int gridSize = new Vector2Int(20, 20);
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private Vector3 gridOffset;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float obstacleCheckRadius = 0.4f;

    private Node[,] grid;
    private readonly List<Node> openList = new();
    private readonly HashSet<Node> closedList = new();
    private readonly List<Node> retraceBuffer = new();

    private void Awake()
    {
        BuildGrid();
    }

    public void BuildGrid()
    {
        grid = new Node[gridSize.x, gridSize.y];
        Vector3 origin = transform.position + gridOffset;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = origin + new Vector3(x * cellSize, 0f, y * cellSize);
                bool walkable = !Physics.CheckSphere(worldPos, obstacleCheckRadius, obstacleMask);
                grid[x, y] = new Node(new Vector2Int(x, y), walkable, worldPos);
            }
        }
    }

    public List<Vector3> FindPath(Vector3 startWorld, Vector3 targetWorld)
    {
        if (grid == null || grid.GetLength(0) != gridSize.x || grid.GetLength(1) != gridSize.y)
            BuildGrid();

        Vector2Int startGrid = WorldToGrid(startWorld);
        Vector2Int targetGrid = WorldToGrid(targetWorld);

        if (!IsInside(startGrid) || !IsInside(targetGrid))
            return null;

        Node startNode = grid[startGrid.x, startGrid.y];
        Node targetNode = grid[targetGrid.x, targetGrid.y];

        if (!startNode.walkable || !targetNode.walkable)
            return null;

        ResetGrid();
        openList.Clear(); // OPTIMIZED: reuse pathfinding collections instead of reallocating them every call.
        closedList.Clear(); // OPTIMIZED: reuse pathfinding collections instead of reallocating them every call.
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            int currentIndex = 0;
            Node current = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                Node candidate = openList[i];

                if (candidate.fCost < current.fCost || candidate.fCost == current.fCost && candidate.hCost < current.hCost)
                {
                    current = candidate;
                    currentIndex = i;
                }
            }

            openList.RemoveAt(currentIndex);
            closedList.Add(current);

            if (current == targetNode)
                return RetracePath(startNode, targetNode);

            for (int i = 0; i < CardinalDirections.Length; i++)
            {
                Vector2Int next = current.gridPosition + CardinalDirections[i];

                if (!IsInside(next))
                    continue;

                Node neighbor = grid[next.x, next.y];

                if (!neighbor.walkable || closedList.Contains(neighbor))
                    continue;

                int newCost = current.gCost + 10;
                bool isInOpenList = openList.Contains(neighbor);

                if (newCost < neighbor.gCost || !isInOpenList)
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = current;

                    if (!isInOpenList)
                        openList.Add(neighbor);
                }
            }
        }

        return null;
    }

    private void ResetGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x, y].gCost = int.MaxValue;
                grid[x, y].hCost = 0;
                grid[x, y].parent = null;
            }
        }
    }

    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        retraceBuffer.Clear();
        Node current = endNode;

        while (current != startNode)
        {
            retraceBuffer.Add(current);
            current = current.parent;

            if (current == null)
                return null;
        }

        retraceBuffer.Reverse();
        List<Vector3> worldPath = new List<Vector3>(retraceBuffer.Count);

        for (int i = 0; i < retraceBuffer.Count; i++)
            worldPath.Add(retraceBuffer[i].worldPosition);

        return worldPath;
    }

    private int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int dstY = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
        return (dstX + dstY) * 10;
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3 local = worldPosition - (transform.position + gridOffset);

        int x = Mathf.RoundToInt(local.x / cellSize);
        int y = Mathf.RoundToInt(local.z / cellSize);

        x = Mathf.Clamp(x, 0, gridSize.x - 1);
        y = Mathf.Clamp(y, 0, gridSize.y - 1);

        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return transform.position + gridOffset + new Vector3(gridPosition.x * cellSize, 0f, gridPosition.y * cellSize);
    }

    public bool IsWalkableWorld(Vector3 worldPosition)
    {
        Vector2Int gridPos = WorldToGrid(worldPosition);
        return grid[gridPos.x, gridPos.y].walkable;
    }

    public Vector3 GetNearestWalkableWorld(Vector3 worldPosition)
    {
        Vector2Int center = WorldToGrid(worldPosition);

        if (grid[center.x, center.y].walkable)
            return grid[center.x, center.y].worldPosition;

        for (int radius = 1; radius < Mathf.Max(gridSize.x, gridSize.y); radius++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    Vector2Int test = center + new Vector2Int(x, y);

                    if (!IsInside(test))
                        continue;

                    if (grid[test.x, test.y].walkable)
                        return grid[test.x, test.y].worldPosition;
                }
            }
        }

        return transform.position + gridOffset;
    }

    private bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y;
    }

    private void OnDrawGizmosSelected()
    {
        bool useRuntimeGrid = Application.isPlaying && grid != null;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Node node = useRuntimeGrid ? grid[x, y] : null;
                Vector3 pos = node != null ? node.worldPosition : GridToWorld(new Vector2Int(x, y));
                Gizmos.color = node != null && !node.walkable ? Color.red : Color.green; // OPTIMIZED: use the correct gizmo color for walkable vs blocked cells.
                Gizmos.DrawWireCube(pos, new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f));
            }
        }
    }

    private class Node
    {
        public Vector2Int gridPosition;
        public bool walkable;
        public Vector3 worldPosition;
        public int gCost;
        public int hCost;
        public Node parent;
        public int fCost => gCost + hCost;

        public Node(Vector2Int gridPosition, bool walkable, Vector3 worldPosition)
        {
            this.gridPosition = gridPosition;
            this.walkable = walkable;
            this.worldPosition = worldPosition;
            gCost = int.MaxValue;
        }
    }
}
