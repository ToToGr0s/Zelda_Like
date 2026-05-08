using UnityEngine;

public class Restart : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private PlayerHealth playerHealth;

    public void RestartPlayer()
    {
        player.position = new Vector3(0f, 0.5f, 0f);
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
        playerHealth.FullReset();
    }
}