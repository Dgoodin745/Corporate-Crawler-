using System;
using UnityEngine;

namespace CorporateCrawler.Arenas
{
    /// <summary>
    /// Minimal graybox enemy health used by prototype spawners and room-clear checks.
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 3;

        public event Action<EnemyHealth> Defeated;

        public int CurrentHealth { get; private set; }

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void Damage(int amount)
        {
            CurrentHealth -= Mathf.Max(1, amount);
            if (CurrentHealth <= 0)
            {
                Defeated?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
