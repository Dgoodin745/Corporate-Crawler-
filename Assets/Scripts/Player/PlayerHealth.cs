using System;
using UnityEngine;

namespace CorporateCrawler.Player
{
    /// <summary>
    /// Tracks player health, damage intake, healing, and death state.
    /// Other systems can subscribe to the events to drive UI, animation, or respawn logic.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;
        [SerializeField] private bool destroyOnDeath;

        public event Action<float, float> HealthChanged;
        public event Action Died;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public bool IsDead { get; private set; }

        private void Awake()
        {
            maxHealth = Mathf.Max(1f, maxHealth);
            currentHealth = Mathf.Clamp(currentHealth <= 0f ? maxHealth : currentHealth, 0f, maxHealth);
            IsDead = currentHealth <= 0f;
        }

        public void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f)
            {
                return;
            }

            currentHealth = Mathf.Max(0f, currentHealth - amount);
            HealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f)
            {
                return;
            }

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            HealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void Revive(float healthAmount = -1f)
        {
            IsDead = false;
            currentHealth = Mathf.Clamp(healthAmount < 0f ? maxHealth : healthAmount, 1f, maxHealth);
            HealthChanged?.Invoke(currentHealth, maxHealth);
        }

        private void Die()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            Died?.Invoke();

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }
}
