using UnityEngine;

public enum RoomMarkerType
{
    Locked,
    Pushable
}

public class RoomMarker : MonoBehaviour
{
    public RoomMarkerType type;
    public Transform spawnPoint;
}