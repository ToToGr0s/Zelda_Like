using UnityEngine;

public class PlayerDirection : MonoBehaviour
{
    [SerializeField] private Vector3 lastDirection = Vector3.forward;

    public void SetDirection(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        lastDirection = direction.normalized;
    }

    public Vector3 GetLastDirection()
    {
        return lastDirection;
    }
}