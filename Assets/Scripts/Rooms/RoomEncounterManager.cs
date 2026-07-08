using System.Collections.Generic;
using CorporateCrawler.Enemies;
using UnityEngine;
using UnityEngine.Events;

namespace CorporateCrawler.Rooms
{
    public class RoomEncounterManager : MonoBehaviour
    {
        [SerializeField] private List<EnemyHealth> enemyPrefabs = new List<EnemyHealth>();
        [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
        [SerializeField] private UnityEvent encounterStarted;
        [SerializeField] private UnityEvent encounterCleared;

        private readonly List<EnemyHealth> livingEnemies = new List<EnemyHealth>();
        private bool started;

        public void StartEncounter()
        {
            if (started)
            {
                return;
            }

            started = true;
            encounterStarted?.Invoke();

            for (int i = 0; i < spawnPoints.Count; i++)
            {
                EnemyHealth prefab = enemyPrefabs.Count == 0 ? null : enemyPrefabs[i % enemyPrefabs.Count];
                if (prefab == null || spawnPoints[i] == null)
                {
                    continue;
                }

                EnemyHealth enemy = Instantiate(prefab, spawnPoints[i].position, spawnPoints[i].rotation);
                enemy.Died += HandleEnemyDied;
                livingEnemies.Add(enemy);
            }

            CheckForClear();
        }

        private void HandleEnemyDied(EnemyHealth enemy)
        {
            enemy.Died -= HandleEnemyDied;
            livingEnemies.Remove(enemy);
            CheckForClear();
        }

        private void CheckForClear()
        {
            if (started && livingEnemies.Count == 0)
            {
                encounterCleared?.Invoke();
            }
        }
    }
}
