using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    public static RoomCameraController Instance { get; private set; }

    [SerializeField] private float smoothTime = 0.35f;

    private Vector3 velocity;
    private Vector3 targetPosition;

    private void Awake()
    {
        Instance = this;
        targetPosition = transform.position;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void MoveToPoint(Transform targetPoint)
    {
        if (targetPoint == null)
            return;

        targetPosition = targetPoint.position;
    }
}