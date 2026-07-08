using UnityEngine;

/// <summary>
/// Placeholder keycap projectile that travels in a straight line, damages enemies, and expires at range.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private int defaultDamage = 1;
    [SerializeField] private float defaultSpeed = 12f;
    [SerializeField] private float defaultRange = 8f;
    [SerializeField] private string enemyTag = "Enemy";

    private int damage;
    private float speed;
    private float range;
    private Vector2 direction;
    private Vector3 startPosition;
    private GameObject owner;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Awake()
    {
        damage = defaultDamage;
        speed = defaultSpeed;
        range = defaultRange;
        direction = transform.right;
        startPosition = transform.position;
    }

    public void Launch(Vector2 launchDirection, int projectileDamage, float projectileSpeed, float projectileRange, GameObject projectileOwner)
    {
        direction = launchDirection.sqrMagnitude > 0f ? launchDirection.normalized : Vector2.right;
        damage = projectileDamage;
        speed = projectileSpeed;
        range = projectileRange;
        owner = projectileOwner;
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryHit(collision.gameObject);
    }

    private void TryHit(GameObject target)
    {
        if (target == owner || !target.CompareTag(enemyTag))
        {
            return;
        }

        ApplyDamage(target);
        Destroy(gameObject);
    }

    private void ApplyDamage(GameObject target)
    {
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            return;
        }

        target.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
    }
}

public interface IDamageable
{
    void TakeDamage(int amount);
}
