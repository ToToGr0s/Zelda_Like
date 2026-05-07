using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector2 inputDirection;
    private float speedMultiplier = 1f;
    private Vector3 lastLookDirection = Vector3.forward;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            input.y = 0f;
            input.x = Mathf.Sign(input.x);
        }
        else if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
        {
            input.x = 0f;
            input.y = Mathf.Sign(input.y);
        }
        else if (input != Vector2.zero)
        {
            input.y = 0f;
            input.x = Mathf.Sign(input.x);
        }

        inputDirection = input;

        if (inputDirection != Vector2.zero)
            lastLookDirection = new Vector3(inputDirection.x, 0f, inputDirection.y);
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

    public Vector3 GetLastLookDirection()
    {
        return lastLookDirection;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
}