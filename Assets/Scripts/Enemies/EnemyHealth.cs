using UnityEngine;
using UnityEngine.Events;

namespace CorporateCrawler.Enemies
{
    /// <summary>
    /// Simple reusable health component for enemies. Raises events when damaged or killed
    /// so animation, audio, and score systems can hook in from the Inspector.
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private bool destroyOnDeath = true;
        [SerializeField] private float destroyDelay = 0.25f;

        [Header("Events")]
        [SerializeField] private UnityEvent<float, float> onDamaged;
        [SerializeField] private UnityEvent onDeath;

        private bool isDead;
        private float currentHealth;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public bool IsDead => isDead;

        private void Awake()
        {
            currentHealth = Mathf.Max(1f, maxHealth);
            maxHealth = currentHealth;
        }

        public void TakeDamage(float amount)
        {
            if (isDead || amount <= 0f)
            {
                return;
            }

            currentHealth = Mathf.Max(0f, currentHealth - amount);
            onDamaged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (isDead || amount <= 0f)
            {
                return;
            }

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }

        public void Die()
        {
            if (isDead)
            {
                return;
            }

            isDead = true;
            currentHealth = 0f;
            onDeath?.Invoke();

            if (destroyOnDeath)
            {
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}
