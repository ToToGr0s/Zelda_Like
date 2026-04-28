using UnityEngine;

public class RoomConnections : MonoBehaviour
{
    [SerializeField] private GameObject wallLeft;
    [SerializeField] private GameObject wallRight;
    [SerializeField] private GameObject wallTop;
    [SerializeField] private GameObject wallBottom;
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GameObject doorRight;
    [SerializeField] private GameObject doorTop;
    [SerializeField] private GameObject doorBottom;

    [SerializeField] private GameObject lockedDoorLeft;
    [SerializeField] private GameObject lockedDoorRight;
    [SerializeField] private GameObject lockedDoorTop;
    [SerializeField] private GameObject lockedDoorBottom;

    public Transform GetDoorPoint(Vector2Int direction)
    {
        if (direction == Vector2Int.left && doorLeft != null) return doorLeft.transform;
        if (direction == Vector2Int.right && doorRight != null) return doorRight.transform;
        if (direction == Vector2Int.up && doorTop != null) return doorTop.transform;
        if (direction == Vector2Int.down && doorBottom != null) return doorBottom.transform;
        return null;
    }

    public void Setup(bool openLeft, bool openRight, bool openTop, bool openBottom)
    {
        SetSide(wallLeft, doorLeft, openLeft);
        SetSide(wallRight, doorRight, openRight);
        SetSide(wallTop, doorTop, openTop);
        SetSide(wallBottom, doorBottom, openBottom);
    }

    public void ApplyLockedDoor(Vector2Int exitDirection)
    {
        if (exitDirection == Vector2Int.left && lockedDoorLeft != null) lockedDoorLeft.SetActive(true);
        if (exitDirection == Vector2Int.right && lockedDoorRight != null) lockedDoorRight.SetActive(true);
        if (exitDirection == Vector2Int.up && lockedDoorTop != null) lockedDoorTop.SetActive(true);
        if (exitDirection == Vector2Int.down && lockedDoorBottom != null) lockedDoorBottom.SetActive(true);
    }

    private void SetSide(GameObject wall, GameObject door, bool open)
    {
        if (wall != null) wall.SetActive(!open);
        if (door != null) door.SetActive(open);
    }
}