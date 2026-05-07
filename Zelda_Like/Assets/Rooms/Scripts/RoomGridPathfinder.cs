using System.Collections.Generic;
using UnityEngine;

public class RoomGridPathfinder : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize = new Vector2Int(20, 20);
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private Vector3 gridOffset;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float obstacleCheckRadius = 0.4f;

    private Node[,] grid;

    private void Awake()
    {
        BuildGrid();
    }

    public void BuildGrid()
    {
        grid = new Node[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = GridToWorld(new Vector2Int(x, y));
                bool walkable = !Physics.CheckSphere(worldPos, obstacleCheckRadius, obstacleMask);
                grid[x, y] = new Node(new Vector2Int(x, y), walkable, worldPos);
            }
        }
    }

    public List<Vector3> FindPath(Vector3 startWorld, Vector3 targetWorld)
    {
        Vector2Int startGrid = WorldToGrid(startWorld);
        Vector2Int targetGrid = WorldToGrid(targetWorld);

        if (!IsInside(startGrid) || !IsInside(targetGrid))
            return null;

        Node startNode = grid[startGrid.x, startGrid.y];
        Node targetNode = grid[targetGrid.x, targetGrid.y];

        if (!startNode.walkable || !targetNode.walkable)
            return null;

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        ResetGrid();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node current = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < current.fCost || openList[i].fCost == current.fCost && openList[i].hCost < current.hCost)
                    current = openList[i];
            }

            openList.Remove(current);
            closedList.Add(current);

            if (current == targetNode)
                return RetracePath(startNode, targetNode);

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (!neighbor.walkable || closedList.Contains(neighbor))
                    continue;

                int newCost = current.gCost + 10;

                if (newCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = current;

                    if (!openList.Contains(neighbor))
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
        List<Node> path = new List<Node>();
        Node current = endNode;

        while (current != startNode)
        {
            path.Add(current);
            current = current.parent;

            if (current == null)
                return null;
        }

        path.Reverse();

        List<Vector3> worldPath = new List<Vector3>();

        for (int i = 0; i < path.Count; i++)
            worldPath.Add(path[i].worldPosition);

        return worldPath;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int next = node.gridPosition + directions[i];

            if (IsInside(next))
                neighbors.Add(grid[next.x, next.y]);
        }

        return neighbors;
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
        Gizmos.color = Color.white;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 pos = Application.isPlaying && grid != null ? grid[x, y].worldPosition : GridToWorld(new Vector2Int(x, y));
                Gizmos.color = Application.isPlaying && grid != null && !grid[x, y].walkable ? Color.white : Color.white;
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