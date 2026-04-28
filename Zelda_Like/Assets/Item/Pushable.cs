using UnityEngine;

public class Pushable : MonoBehaviour
{
    [SerializeField] private float pushSpeed = 3f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float playerSlowMultiplier = 0.4f;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool isBeingPushed;
    private MovementController pusherMovement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        targetPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (!isBeingPushed)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance < 0.05f)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = targetPosition;
            isBeingPushed = false;

            if (pusherMovement != null)
                pusherMovement.SetSpeedMultiplier(1f);

            return;
        }

        rb.linearVelocity = direction * pushSpeed;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (isBeingPushed)
            return;

        MovementController movement = collision.gameObject.GetComponent<MovementController>();

        if (movement == null)
            return;

        Vector2 playerInput = movement.GetMoveDirection();

        if (playerInput.magnitude < 0.1f)
            return;

        Vector3 pushDirection = GetSnapDirection(new Vector3(playerInput.x, 0f, playerInput.y));
        Vector3 nextTarget = targetPosition + pushDirection * gridSize;

        if (!CanMoveTo(nextTarget))
            return;

        targetPosition = nextTarget;
        isBeingPushed = true;

        pusherMovement = movement;
        pusherMovement.SetSpeedMultiplier(playerSlowMultiplier);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (pusherMovement != null)
            pusherMovement.SetSpeedMultiplier(1f);

        pusherMovement = null;
    }

    private Vector3 GetSnapDirection(Vector3 rawDirection)
    {
        if (Mathf.Abs(rawDirection.x) > Mathf.Abs(rawDirection.z))
            return new Vector3(Mathf.Sign(rawDirection.x), 0f, 0f);
        else
            return new Vector3(0f, 0f, Mathf.Sign(rawDirection.z));
    }

    private bool CanMoveTo(Vector3 position)
    {
        return !Physics.CheckSphere(position, gridSize * 0.4f, ~LayerMask.GetMask("Player"));
    }
}