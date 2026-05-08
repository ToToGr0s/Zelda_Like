using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private MovementController movementController;
    [SerializeField] private PlayerHealth playerHealth;

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveZ = Animator.StringToHash("MoveZ");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Hit = Animator.StringToHash("Hit");

    private bool lastIsMoving;
    private Vector2 lastMoveDirection;
    private bool hasAnimatorState;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>(); // OPTIMIZED: cache local Animator once when not assigned.

        if (movementController == null)
            movementController = GetComponent<MovementController>(); // OPTIMIZED: cache local movement controller once when not assigned.

        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (animator == null || movementController == null)
            return;

        Vector2 moveDir = movementController.GetMoveDirection();
        bool isMoving = moveDir != Vector2.zero;

        if (hasAnimatorState && lastIsMoving == isMoving && lastMoveDirection == moveDir)
            return;

        animator.SetBool(IsMoving, isMoving); // OPTIMIZED: avoid redundant Animator parameter writes.
        animator.SetFloat(MoveX, moveDir.x);
        animator.SetFloat(MoveZ, moveDir.y);

        lastIsMoving = isMoving;
        lastMoveDirection = moveDir;
        hasAnimatorState = true;
    }

    public void PlayAttack()
    {
        if (animator != null)
            animator.SetTrigger(Attack);
    }

    public void PlayHit()
    {
        if (animator != null)
            animator.SetTrigger(Hit);
    }

    public void PlayDeath()
    {
        if (animator != null)
            animator.SetTrigger(Death);
    }
}
