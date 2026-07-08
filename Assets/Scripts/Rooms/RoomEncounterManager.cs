using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CorporateCrawler.Rooms
{
    /// <summary>
    /// Spawns and tracks a room encounter, keeping the room locked until every
    /// spawned enemy has been defeated.
    /// </summary>
    public sealed class RoomEncounterManager : MonoBehaviour
    {
        [Serializable]
        public sealed class EnemySpawn
        {
            [Tooltip("Enemy prefab to spawn for this encounter entry.")]
            public GameObject EnemyPrefab;

            [Tooltip("Optional spawn point. If omitted, the room manager transform is used.")]
            public Transform SpawnPoint;
        }

        [Header("Encounter")]
        [SerializeField]
        [Tooltip("Enemies spawned when the encounter starts.")]
        private List<EnemySpawn> enemiesToSpawn = new List<EnemySpawn>();

        [SerializeField]
        [Tooltip("Start the encounter automatically when this component is enabled.")]
        private bool startOnEnable;

        [Header("Room Lock")]
        [SerializeField]
        [Tooltip("Objects enabled while the encounter is active, such as barrier doors or blockers.")]
        private List<GameObject> roomLockObjects = new List<GameObject>();

        [Header("Events")]
        [SerializeField]
        [Tooltip("Raised once after all spawned enemies have been defeated.")]
        private UnityEvent encounterCompleted = new UnityEvent();

        private readonly HashSet<GameObject> remainingEnemies = new HashSet<GameObject>();
        private bool encounterActive;
        private bool encounterCompletedRaised;

        public event Action<RoomEncounterManager> EncounterCompleted;

        public bool EncounterActive => encounterActive;

        public int RemainingEnemyCount => remainingEnemies.Count;

        public UnityEvent EncounterCompletedEvent => encounterCompleted;

        private void OnEnable()
        {
            if (startOnEnable)
            {
                StartEncounter();
            }
        }

        public void StartEncounter()
        {
            if (encounterActive || encounterCompletedRaised)
            {
                return;
            }

            encounterActive = true;
            SetRoomLocked(true);
            SpawnConfiguredEnemies();

            if (remainingEnemies.Count == 0)
            {
                CompleteEncounter();
            }
        }

        public void RegisterEnemy(GameObject enemy)
        {
            if (enemy == null || !remainingEnemies.Add(enemy))
            {
                return;
            }

            EnemyDefeatTracker tracker = enemy.GetComponent<EnemyDefeatTracker>();
            if (tracker == null)
            {
                tracker = enemy.AddComponent<EnemyDefeatTracker>();
            }

            tracker.Initialize(this, enemy);
        }

        public void NotifyEnemyDefeated(GameObject enemy)
        {
            if (enemy == null || !remainingEnemies.Remove(enemy) || !encounterActive)
            {
                return;
            }

            if (remainingEnemies.Count == 0)
            {
                CompleteEncounter();
            }
        }

        private void SpawnConfiguredEnemies()
        {
            for (int i = 0; i < enemiesToSpawn.Count; i++)
            {
                EnemySpawn spawn = enemiesToSpawn[i];
                if (spawn == null || spawn.EnemyPrefab == null)
                {
                    continue;
                }

                Transform spawnPoint = spawn.SpawnPoint != null ? spawn.SpawnPoint : transform;
                GameObject enemy = Instantiate(
                    spawn.EnemyPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation);

                RegisterEnemy(enemy);
            }
        }

        private void CompleteEncounter()
        {
            if (encounterCompletedRaised)
            {
                return;
            }

            encounterActive = false;
            encounterCompletedRaised = true;
            SetRoomLocked(false);
            EncounterCompleted?.Invoke(this);
            encounterCompleted.Invoke();
        }

        private void SetRoomLocked(bool locked)
        {
            for (int i = 0; i < roomLockObjects.Count; i++)
            {
                if (roomLockObjects[i] != null)
                {
                    roomLockObjects[i].SetActive(locked);
                }
            }
        }

        private sealed class EnemyDefeatTracker : MonoBehaviour
        {
            private RoomEncounterManager manager;
            private GameObject trackedEnemy;
            private bool notified;

            public void Initialize(RoomEncounterManager encounterManager, GameObject enemy)
            {
                manager = encounterManager;
                trackedEnemy = enemy;
                notified = false;
            }

            private void OnDestroy()
            {
                if (notified || manager == null)
                {
                    return;
                }

                notified = true;
                manager.NotifyEnemyDefeated(trackedEnemy);
            }
        }
    }
}
