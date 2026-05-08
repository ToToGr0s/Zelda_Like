using UnityEngine;

public class RoomCameraTrigger : MonoBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] private Transform cameraPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PlayerTag))
            return;

        if (RoomCameraController.Instance != null)
            RoomCameraController.Instance.MoveToPoint(cameraPoint);
    }
}
