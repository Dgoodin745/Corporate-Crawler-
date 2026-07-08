using UnityEngine;

namespace CorporateCrawler.Combat
{
    /// <summary>
    /// Simple forward-moving projectile that damages the first valid target it hits.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 18f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private int damage = 10;
        [SerializeField] private LayerMask hitMask = ~0;

        private float age;
        private GameObject owner;

        public void Initialize(GameObject projectileOwner, int projectileDamage, float projectileSpeed, float projectileLifetime)
        {
            owner = projectileOwner;
            damage = projectileDamage;
            speed = projectileSpeed;
            lifetime = projectileLifetime;
            age = 0f;
        }

        private void Reset()
        {
            Collider projectileCollider = GetComponent<Collider>();
            projectileCollider.isTrigger = true;
        }

        private void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            age += Time.deltaTime;

            if (age >= lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (owner != null && other.gameObject == owner)
            {
                return;
            }

            if ((hitMask.value & (1 << other.gameObject.layer)) == 0)
            {
                return;
            }

            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, owner);
            }

            Destroy(gameObject);
        }
    }

    public interface IDamageable
    {
        void TakeDamage(int amount, GameObject source);
    }
}
