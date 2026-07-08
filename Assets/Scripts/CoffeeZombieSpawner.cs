using System.Collections.Generic;
using UnityEngine;

namespace CorporateCrawler.Arenas
{
    /// <summary>
    /// Spawns a wave of simple Coffee Zombie placeholders and reports remaining enemies.
    /// </summary>
    public class CoffeeZombieSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject coffeeZombiePrefab;
        [SerializeField] private int spawnCount = 3;
        [SerializeField] private float spawnRadius = 1.5f;
        [SerializeField] private bool spawnOnStart = true;

        private readonly List<EnemyHealth> aliveEnemies = new();

        public int AliveCount => aliveEnemies.Count;

        private void Start()
        {
            if (spawnOnStart)
            {
                SpawnWave();
            }
        }

        public void SpawnWave()
        {
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 offset = Quaternion.Euler(0f, i * (360f / Mathf.Max(1, spawnCount)), 0f) * Vector3.forward * spawnRadius;
                GameObject enemy = Instantiate(coffeeZombiePrefab, transform.position + offset, Quaternion.identity);
                enemy.name = $"Coffee Zombie {i + 1}";
                enemy.SetActive(true);

                if (enemy.TryGetComponent(out EnemyHealth health))
                {
                    aliveEnemies.Add(health);
                    health.Defeated += OnEnemyDefeated;
                }
            }
        }

        private void OnEnemyDefeated(EnemyHealth health)
        {
            health.Defeated -= OnEnemyDefeated;
            aliveEnemies.Remove(health);
        }
    }
}
