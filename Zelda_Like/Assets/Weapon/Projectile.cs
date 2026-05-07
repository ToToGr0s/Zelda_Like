using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;

    private Vector3 moveDirection;
    private float speed;

    public void Init(Vector3 direction, float projectileSpeed)
    {
        moveDirection = direction.normalized;
        speed = projectileSpeed;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        if (other.transform.root.CompareTag("Player")) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyKillable enemy = other.GetComponentInParent<EnemyKillable>();
            if (enemy != null)
                enemy.Kill();

            Destroy(gameObject);
            return;
        }
        if(other.CompareTag("Wall"))
            Destroy(gameObject);
    }


}