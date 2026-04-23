using UnityEngine;

public class RoomDebugGizmo : MonoBehaviour
{
    [SerializeField] private Vector3 roomSize = new Vector3(20f, 3f, 20f);
    [SerializeField] private Vector3 roomOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private Color wireColor = Color.cyan;
    [SerializeField] private Color fillColor = new Color(0f, 1f, 1f, 0.08f);

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = fillColor;
        Gizmos.DrawCube(roomOffset, roomSize);
        Gizmos.color = wireColor;
        Gizmos.DrawWireCube(roomOffset, roomSize);
    }
}