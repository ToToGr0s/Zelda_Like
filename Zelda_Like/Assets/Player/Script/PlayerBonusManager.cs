using UnityEngine;
using System.Collections;

public class PlayerBonusManager : MonoBehaviour
{
    private MovementController movementController;
    private PlayerHealth playerHealth;
    private bool hasShield;
    private bool isInvincible;
    private bool hasRevive;
    private float damageMultiplier = 1f;

    public float DamageMultiplier => damageMultiplier;
    public bool HasShield => hasShield;
    public bool IsInvincible => isInvincible;
    public bool HasRevive => hasRevive;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private MovementController GetMovementController()
    {
        if (movementController == null)
            movementController = GetComponentInParent<MovementController>();
        if (movementController == null)
            movementController = FindFirstObjectByType<MovementController>();
        return movementController;
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    public void ApplyDamageBoost(float multiplier, float duration)
    {
        StartCoroutine(DamageBoostRoutine(multiplier, duration));
    }

    public void ApplyShield(float duration)
    {
        StartCoroutine(ShieldRoutine(duration));
    }

    public void ApplyRegeneration(float healPerSecond, float duration)
    {
        StartCoroutine(RegenerationRoutine(healPerSecond, duration));
    }

    public void ApplyInvincibility(float duration)
    {
        StartCoroutine(InvincibilityRoutine(duration));
    }

    public void ApplyBerserk(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostRoutine(multiplier, duration));
        StartCoroutine(DamageBoostRoutine(multiplier, duration));
    }

    public void ApplyRevive()
    {
        hasRevive = true;
    }

    public void ConsumeShield()
    {
        hasShield = false;
    }

    public void ConsumeRevive()
    {
        hasRevive = false;
    }

    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        MovementController ctrl = GetMovementController();

        if (ctrl == null)
        {
            Debug.LogWarning("[PlayerBonusManager] MovementController introuvable, SpeedBoost annulé.");
            yield break;
        }

        ctrl.SetSpeedMultiplier(multiplier);
        yield return new WaitForSeconds(duration);
        ctrl.SetSpeedMultiplier(1f);
    }

    private IEnumerator DamageBoostRoutine(float multiplier, float duration)
    {
        damageMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
    }

    private IEnumerator ShieldRoutine(float duration)
    {
        hasShield = true;
        yield return new WaitForSeconds(duration);
        hasShield = false;
    }

    private IEnumerator RegenerationRoutine(float healPerSecond, float duration)
    {
        if (playerHealth == null)
            yield break;

        float elapsed = 0f;
        float interval = healPerSecond > 0f ? 1f / healPerSecond : 1f;

        while (elapsed < duration)
        {
            yield return new WaitForSeconds(interval);
            playerHealth.Heal(1);
            elapsed += interval;
        }
    }

    private IEnumerator InvincibilityRoutine(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }
}
