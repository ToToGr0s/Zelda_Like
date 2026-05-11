using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveZ = Animator.StringToHash("MoveZ");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Death = Animator.StringToHash("Death");

    private Vector3 previousPosition;
    private bool lastIsMoving;
    private Vector2 lastMoveDir;
    private bool initialized;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        previousPosition = transform.position;
        initialized = true;
    }

    private void Update()
    {
        if (animator == null || !initialized)
            return;

        // Detect movement via position delta — works with both transform.position and Rigidbody movement.
        Vector3 delta = transform.position - previousPosition;
        previousPosition = transform.position;

        bool isMoving = delta.sqrMagnitude > 0.000001f;

        // Preserve the last facing direction so idle animations stay correctly oriented.
        if (isMoving)
            lastMoveDir = new Vector2(delta.x, delta.z).normalized;

        if (lastIsMoving == isMoving)
            return;

        animator.SetBool(IsMoving, isMoving);
        // Always write direction (preserves facing when transitioning to idle).
        animator.SetFloat(MoveX, lastMoveDir.x);
        animator.SetFloat(MoveZ, lastMoveDir.y);

        lastIsMoving = isMoving;
    }

    public void PlayAttack()
    {
        if (animator != null)
            animator.SetTrigger(Attack);
    }

    public void PlayDeath()
    {
        if (animator != null)
            animator.SetTrigger(Death);
    }
}
