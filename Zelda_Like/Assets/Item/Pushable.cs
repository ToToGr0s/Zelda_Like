using UnityEngine;

public class Pushable : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string PlayerLayerName = "Player";
    private const float ArrivalThresholdSqr = 0.0025f;

    [SerializeField] private float pushSpeed = 3f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float playerSlowMultiplier = 0.4f;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool isBeingPushed;
    private MovementController pusherMovement;
    private GameObject cachedPlayerObject;
    private int pushBlockMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        targetPosition = transform.position;
        pushBlockMask = ~LayerMask.GetMask(PlayerLayerName); // OPTIMIZED: cache the layer mask used by collision checks.
    }

    private void FixedUpdate()
    {
        if (!isBeingPushed)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 toTarget = targetPosition - transform.position;

        if (toTarget.sqrMagnitude < ArrivalThresholdSqr)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = targetPosition;
            isBeingPushed = false;

            if (pusherMovement != null)
                pusherMovement.SetSpeedMultiplier(1f);

            return;
        }

        rb.linearVelocity = toTarget.normalized * pushSpeed;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag(PlayerTag) || isBeingPushed)
            return;

        if (cachedPlayerObject != collision.gameObject)
        {
            cachedPlayerObject = collision.gameObject;
            pusherMovement = collision.gameObject.GetComponent<MovementController>(); // OPTIMIZED: cache the pushing player controller.
        }

        if (pusherMovement == null)
            return;

        Vector2 playerInput = pusherMovement.GetMoveDirection();

        if (playerInput.sqrMagnitude < 0.01f)
            return;

        Vector3 pushDirection = GetSnapDirection(new Vector3(playerInput.x, 0f, playerInput.y));
        Vector3 nextTarget = targetPosition + pushDirection * gridSize;

        if (!CanMoveTo(nextTarget))
            return;

        targetPosition = nextTarget;
        isBeingPushed = true;
        pusherMovement.SetSpeedMultiplier(playerSlowMultiplier);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (cachedPlayerObject != collision.gameObject)
            return;

        if (pusherMovement != null)
            pusherMovement.SetSpeedMultiplier(1f);

        cachedPlayerObject = null;
        pusherMovement = null;
    }

    private static Vector3 GetSnapDirection(Vector3 rawDirection)
    {
        if (Mathf.Abs(rawDirection.x) > Mathf.Abs(rawDirection.z))
            return new Vector3(Mathf.Sign(rawDirection.x), 0f, 0f);

        return new Vector3(0f, 0f, Mathf.Sign(rawDirection.z));
    }

    private bool CanMoveTo(Vector3 position)
    {
        return !Physics.CheckSphere(position, gridSize * 0.4f, pushBlockMask);
    }
}
