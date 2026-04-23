using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Start")]
    [SerializeField] private GameObject startRoomPrefab;

    [Header("Rooms")]
    [SerializeField] private GameObject[] roomPrefabs;
    [SerializeField] private int roomCount = 10;
    [SerializeField] private float roomWidth = 20f;
    [SerializeField] private float roomLength = 20f;
    [SerializeField] private float floorY = -0.5f;
    [SerializeField] private int maxChildrenPerRoom = 2;
    [SerializeField, Range(0f, 1f)] private float upExitChance = 0.65f;

    [Header("Lock and Key")]
    [SerializeField] private GameObject keyPrefab;

    [Header("Pushable")]
    [SerializeField] private GameObject pushablePrefab;

    private readonly Dictionary<Vector2Int, RoomNode> rooms = new();
    private readonly List<RoomNode> expandableRooms = new();

    private static readonly Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.left,
        Vector2Int.right
    };

    private bool startRoomExitCreated;

    private void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        ClearDungeon();
        startRoomExitCreated = false;

        RoomNode startNode = CreateStartRoom();
        int generatedNormalRooms = 0;

        while (generatedNormalRooms < roomCount && expandableRooms.Count > 0)
        {
            RoomNode parent = GetRandomExpandableRoom();

            if (parent == null)
                break;

            List<Vector2Int> availableDirections = GetAvailableDirections(parent);

            if (availableDirections.Count == 0 || parent.Children.Count >= maxChildrenPerRoom)
            {
                expandableRooms.Remove(parent);
                continue;
            }

            Vector2Int chosenDirection;

            if (!startRoomExitCreated && parent.IsStartRoom && availableDirections.Contains(Vector2Int.up))
            {
                chosenDirection = Vector2Int.up;
                startRoomExitCreated = true;
            }
            else
            {
                chosenDirection = GetRandomDirectionWithUpBias(availableDirections);
            }

            Vector2Int newGridPosition = parent.GridPosition + chosenDirection;
            GameObject roomPrefab = GetValidRoomPrefabForPosition(newGridPosition);

            if (roomPrefab == null)
            {
                parent.BlockedDirections.Add(chosenDirection);

                if (GetAvailableDirections(parent).Count == 0 || parent.Children.Count >= maxChildrenPerRoom)
                    expandableRooms.Remove(parent);

                continue;
            }

            RoomNode newNode = CreateRoom(roomPrefab, newGridPosition, parent, chosenDirection);
            parent.Children.Add(newNode);
            generatedNormalRooms++;

            if (parent.IsStartRoom && startRoomExitCreated)
                expandableRooms.Remove(parent);
            else if (parent.Children.Count >= maxChildrenPerRoom || GetAvailableDirections(parent).Count == 0)
                expandableRooms.Remove(parent);

            if (GetAvailableDirections(newNode).Count > 0)
                expandableRooms.Add(newNode);
        }

        ApplyRoomConnections();
        ApplyLockedDoorOnLockedRoom();
        PlaceKeyForLockedRoom();
        PlacePushableOnPushableRoom();

        Debug.Log("Dungeon generated with " + rooms.Count + " rooms.");
    }

    private void ClearDungeon()
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in transform)
            children.Add(child);

        foreach (Transform child in children)
            Destroy(child.gameObject);

        rooms.Clear();
        expandableRooms.Clear();
    }

    private RoomNode CreateStartRoom()
    {
        Vector2Int startGridPosition = Vector2Int.zero;
        Vector3 worldPosition = GridToWorld(startGridPosition);

        GameObject startRoomInstance = Instantiate(startRoomPrefab, worldPosition, Quaternion.identity, transform);
        RoomNode startNode = new RoomNode(startGridPosition, startRoomInstance, null, Vector2Int.zero, true);

        rooms.Add(startGridPosition, startNode);
        expandableRooms.Add(startNode);

        return startNode;
    }

    private RoomNode CreateRoom(GameObject prefab, Vector2Int gridPosition, RoomNode parent, Vector2Int entryDirection)
    {
        Vector3 worldPosition = GridToWorld(gridPosition);
        GameObject roomInstance = Instantiate(prefab, worldPosition, Quaternion.identity, transform);

        RoomNode node = new RoomNode(gridPosition, roomInstance, parent, entryDirection, false);
        rooms.Add(gridPosition, node);

        return node;
    }

    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * roomWidth, floorY, gridPosition.y * roomLength);
    }

    private RoomNode GetRandomExpandableRoom()
    {
        if (expandableRooms.Count == 0)
            return null;

        return expandableRooms[Random.Range(0, expandableRooms.Count)];
    }

    private Vector2Int GetRandomDirectionWithUpBias(List<Vector2Int> availableDirections)
    {
        if (availableDirections == null || availableDirections.Count == 0)
            return Vector2Int.zero;

        if (availableDirections.Count == 1)
            return availableDirections[0];

        bool canGoUp = availableDirections.Contains(Vector2Int.up);

        if (canGoUp && Random.Range(0f, 1f) < upExitChance)
            return Vector2Int.up;

        List<Vector2Int> fallbackDirections = new List<Vector2Int>(availableDirections);

        if (canGoUp)
            fallbackDirections.Remove(Vector2Int.up);

        if (fallbackDirections.Count == 0)
            return Vector2Int.up;

        return fallbackDirections[Random.Range(0, fallbackDirections.Count)];
    }

    private List<Vector2Int> GetAvailableDirections(RoomNode room)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        foreach (Vector2Int dir in directions)
        {
            if (room.BlockedDirections.Contains(dir))
                continue;

            Vector2Int targetPosition = room.GridPosition + dir;

            if (rooms.ContainsKey(targetPosition))
                continue;

            result.Add(dir);
        }

        return result;
    }

    private GameObject GetValidRoomPrefabForPosition(Vector2Int targetPosition)
    {
        List<GameObject> validPrefabs = new List<GameObject>();

        foreach (GameObject prefab in roomPrefabs)
        {
            if (prefab == null)
                continue;

            if (IsSamePrefabAdjacent(targetPosition, prefab))
                continue;

            validPrefabs.Add(prefab);
        }

        if (validPrefabs.Count == 0)
            return null;

        return validPrefabs[Random.Range(0, validPrefabs.Count)];
    }

    private bool IsSamePrefabAdjacent(Vector2Int targetPosition, GameObject prefab)
    {
        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighborPosition = targetPosition + dir;

            if (!rooms.TryGetValue(neighborPosition, out RoomNode neighbor))
                continue;

            if (neighbor.RoomObject == null)
                continue;

            if (neighbor.RoomObject.name.StartsWith(prefab.name))
                return true;
        }

        return false;
    }

    private bool HasConnection(RoomNode room, Vector2Int direction)
    {
        Vector2Int targetPosition = room.GridPosition + direction;

        if (!rooms.TryGetValue(targetPosition, out RoomNode otherRoom))
            return false;

        if (room.Parent == otherRoom)
            return true;

        if (otherRoom.Parent == room)
            return true;

        return false;
    }

    private void ApplyRoomConnections()
    {
        foreach (RoomNode room in rooms.Values)
        {
            bool openLeft = HasConnection(room, Vector2Int.left);
            bool openRight = HasConnection(room, Vector2Int.right);
            bool openTop = HasConnection(room, Vector2Int.up);
            bool openBottom = HasConnection(room, Vector2Int.down);

            RoomConnections connections = room.RoomObject.GetComponentInChildren<RoomConnections>(true);

            if (connections != null)
                connections.Setup(openLeft, openRight, openTop, openBottom);
        }
    }

    private void ApplyLockedDoorOnLockedRoom()
    {
        foreach (RoomNode room in rooms.Values)
        {
            RoomMarker marker = room.RoomObject.GetComponentInChildren<RoomMarker>(true);

            if (marker == null || marker.type != RoomMarkerType.Locked)
                continue;

            foreach (RoomNode child in room.Children)
            {
                Vector2Int exitDirection = child.GridPosition - room.GridPosition;
                RoomConnections connections = room.RoomObject.GetComponentInChildren<RoomConnections>(true);

                if (connections != null)
                    connections.ApplyLockedDoor(exitDirection);
            }
        }
    }

    private void PlaceKeyForLockedRoom()
    {
        foreach (RoomNode room in rooms.Values)
        {
            RoomMarker marker = room.RoomObject.GetComponentInChildren<RoomMarker>(true);

            if (marker == null || marker.type != RoomMarkerType.Locked)
                continue;

            if (marker.spawnPoint == null || keyPrefab == null)
                continue;

            Instantiate(keyPrefab, marker.spawnPoint.position, marker.spawnPoint.rotation, GetRoomContentParent(room));
        }
    }

    private void PlacePushableOnPushableRoom()
    {
        foreach (RoomNode room in rooms.Values)
        {
            RoomMarker marker = room.RoomObject.GetComponentInChildren<RoomMarker>(true);

            if (marker == null || marker.type != RoomMarkerType.Pushable)
                continue;

            if (pushablePrefab == null)
                continue;

            foreach (RoomNode child in room.Children)
            {
                Vector2Int exitDirection = child.GridPosition - room.GridPosition;
                RoomConnections connections = room.RoomObject.GetComponentInChildren<RoomConnections>(true);

                if (connections == null)
                    continue;

                Transform doorPoint = connections.GetDoorPoint(exitDirection);

                if (doorPoint == null)
                    continue;

                Instantiate(pushablePrefab, doorPoint.position, doorPoint.rotation, GetRoomContentParent(room));
            }
        }
    }

    private Transform GetRoomContentParent(RoomNode room)
    {
        ShowRoom showRoom = room.RoomObject.GetComponentInChildren<ShowRoom>(true);

        if (showRoom != null && showRoom.GetContentParent() != null)
            return showRoom.GetContentParent().transform;

        return room.RoomObject.transform;
    }

    private void OnDrawGizmos()
    {
        float maxExtent = roomCount * Mathf.Max(roomWidth, roomLength);

        Vector3 center = transform.position + new Vector3(0f, 1f, maxExtent / 2f);
        Vector3 size = new Vector3(maxExtent * 2f + roomWidth, 3f, maxExtent + roomLength);

        Gizmos.color = new Color(0f, 1f, 1f, 0.08f);
        Gizmos.DrawCube(center, size);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, size);
    }

    private class RoomNode
    {
        public Vector2Int GridPosition;
        public GameObject RoomObject;
        public RoomNode Parent;
        public Vector2Int EntryDirection;
        public bool IsStartRoom;
        public List<RoomNode> Children = new();
        public HashSet<Vector2Int> BlockedDirections = new();

        public RoomNode(Vector2Int gridPosition, GameObject roomObject, RoomNode parent, Vector2Int entryDirection, bool isStartRoom)
        {
            GridPosition = gridPosition;
            RoomObject = roomObject;
            Parent = parent;
            EntryDirection = entryDirection;
            IsStartRoom = isStartRoom;
        }
    }
}