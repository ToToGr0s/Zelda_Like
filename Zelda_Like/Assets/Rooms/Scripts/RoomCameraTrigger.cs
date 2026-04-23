using UnityEngine;

public class RoomCameraTrigger : MonoBehaviour
{
    [SerializeField] private Transform cameraPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (RoomCameraController.Instance != null)
            RoomCameraController.Instance.MoveToPoint(cameraPoint);
    }
}