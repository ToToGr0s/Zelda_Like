using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector2 inputDirection;
    private float speedMultiplier = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnMove(InputValue value)
    {
        inputDirection = value.Get<Vector2>().normalized;
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(inputDirection.x, 0f, inputDirection.y);
        rb.linearVelocity = move * moveSpeed * speedMultiplier;
    }
    public Vector2 GetMoveDirection()
    {
        return inputDirection;
    }
    
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
}