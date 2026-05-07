using UnityEngine;

public class RoomConnections : MonoBehaviour
{
    [Header("Openings")]
    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject topWall;
    [SerializeField] private GameObject bottomWall;

    [Header("Doors")]
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private GameObject topDoor;
    [SerializeField] private GameObject bottomDoor;

    [Header("Locked Doors")]
    [SerializeField] private GameObject leftLockedDoor;
    [SerializeField] private GameObject rightLockedDoor;
    [SerializeField] private GameObject topLockedDoor;
    [SerializeField] private GameObject bottomLockedDoor;

    [Header("Door Points")]
    [SerializeField] private Transform leftDoorPoint;
    [SerializeField] private Transform rightDoorPoint;
    [SerializeField] private Transform topDoorPoint;
    [SerializeField] private Transform bottomDoorPoint;

    private bool openLeft;
    private bool openRight;
    private bool openTop;
    private bool openBottom;

    private RoomMaterialManager materialManager;

    private void Awake()
    {
        materialManager = GetComponentInChildren<RoomMaterialManager>(true);
    }

    public void Setup(bool openLeft, bool openRight, bool openTop, bool openBottom)
    {
        this.openLeft = openLeft;
        this.openRight = openRight;
        this.openTop = openTop;
        this.openBottom = openBottom;

        ApplyOpenState();
        ApplyMaterial();
    }

    private void ApplyOpenState()
    {
        SetSideState(leftWall, leftDoor, leftLockedDoor, openLeft);
        SetSideState(rightWall, rightDoor, rightLockedDoor, openRight);
        SetSideState(topWall, topDoor, topLockedDoor, openTop);
        SetSideState(bottomWall, bottomDoor, bottomLockedDoor, openBottom);
    }

    private void SetSideState(GameObject wall, GameObject door, GameObject lockedDoor, bool isOpen)
    {
        if (wall != null)
            wall.SetActive(!isOpen);

        if (door != null)
            door.SetActive(isOpen);

        if (lockedDoor != null)
            lockedDoor.SetActive(false);
    }

    private void ApplyMaterial()
    {
        if (materialManager != null)
            materialManager.ApplyFromConnections(openLeft, openRight, openTop, openBottom);
    }

    public void ApplyLockedDoor(Vector2Int exitDirection)
    {
        if (exitDirection == Vector2Int.left)
            SetLockedSide(leftWall, leftDoor, leftLockedDoor);
        else if (exitDirection == Vector2Int.right)
            SetLockedSide(rightWall, rightDoor, rightLockedDoor);
        else if (exitDirection == Vector2Int.up)
            SetLockedSide(topWall, topDoor, topLockedDoor);
        else if (exitDirection == Vector2Int.down)
            SetLockedSide(bottomWall, bottomDoor, bottomLockedDoor);
    }

    private void SetLockedSide(GameObject wall, GameObject door, GameObject lockedDoor)
    {
        if (wall != null)
            wall.SetActive(false);

        if (door != null)
            door.SetActive(false);

        if (lockedDoor != null)
            lockedDoor.SetActive(true);
    }

    public Transform GetDoorPoint(Vector2Int direction)
    {
        if (direction == Vector2Int.left)
            return leftDoorPoint;

        if (direction == Vector2Int.right)
            return rightDoorPoint;

        if (direction == Vector2Int.up)
            return topDoorPoint;

        if (direction == Vector2Int.down)
            return bottomDoorPoint;

        return null;
    }
}