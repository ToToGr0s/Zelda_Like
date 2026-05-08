using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Death = Animator.StringToHash("Death");

    private Rigidbody rb;
    private bool lastIsMoving;
    private bool hasLastIsMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
            animator = GetComponent<Animator>(); // OPTIMIZED: cache local Animator once when not assigned in the inspector.
    }

    private void Update()
    {
        if (animator == null)
            return;

        bool isMoving = rb != null && rb.linearVelocity.sqrMagnitude > 0.01f;

        if (hasLastIsMoving && lastIsMoving == isMoving)
            return;

        animator.SetBool(IsMoving, isMoving); // OPTIMIZED: avoid redundant Animator writes every frame.
        lastIsMoving = isMoving;
        hasLastIsMoving = true;
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
