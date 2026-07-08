using System;
using CorporateCrawler.Combat;
using UnityEngine;

namespace CorporateCrawler.Enemies
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 40;

        private int currentHealth;

        public event Action<EnemyHealth> Died;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int amount, GameObject source)
        {
            if (amount <= 0 || currentHealth <= 0)
            {
                return;
            }

            currentHealth = Mathf.Max(0, currentHealth - amount);
            if (currentHealth == 0)
            {
                Died?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
