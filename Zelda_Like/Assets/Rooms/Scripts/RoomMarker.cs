using UnityEngine;

public enum RoomMarkerType
{
    Locked,
    Pushable,
    Merchant,
    Boss
}

public class RoomMarker : MonoBehaviour
{
    public RoomMarkerType type;
    public Transform spawnPoint;
}