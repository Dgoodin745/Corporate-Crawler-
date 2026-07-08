using System.Collections.Generic;
using UnityEngine;

namespace CorporateCrawler.PrinterRoomPanic
{
    public sealed class PrinterRoomEncounter : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private List<PanicEnemy> enemies = new List<PanicEnemy>();
        [SerializeField] private float clearDelaySeconds = 1.5f;
        [SerializeField] private GameObject roomClearBanner;
        [SerializeField] private Behaviour[] unlockOnClear;

        private int remainingEnemies;
        private bool roomCleared;

        public int RemainingEnemies => remainingEnemies;
        public bool RoomCleared => roomCleared;

        private void Awake()
        {
            if (player == null)
            {
                SinglePlayerPanicController controller = FindObjectOfType<SinglePlayerPanicController>();
                if (controller != null)
                {
                    player = controller.transform;
                }
            }

            remainingEnemies = 0;
            foreach (PanicEnemy enemy in enemies)
            {
                RegisterEnemy(enemy);
            }

            if (roomClearBanner != null)
            {
                roomClearBanner.SetActive(false);
            }
        }

        public void RegisterEnemy(PanicEnemy enemy)
        {
            if (enemy == null)
            {
                return;
            }

            enemy.SetPlayer(player);
            enemy.Defeated += OnEnemyDefeated;
            remainingEnemies++;
        }

        private void OnEnemyDefeated(PanicEnemy enemy)
        {
            enemy.Defeated -= OnEnemyDefeated;
            remainingEnemies = Mathf.Max(0, remainingEnemies - 1);
            if (remainingEnemies == 0 && !roomCleared)
            {
                Invoke(nameof(ClearRoom), clearDelaySeconds);
            }
        }

        private void ClearRoom()
        {
            roomCleared = true;
            if (roomClearBanner != null)
            {
                roomClearBanner.SetActive(true);
            }

            foreach (Behaviour behaviour in unlockOnClear)
            {
                if (behaviour != null)
                {
                    behaviour.enabled = true;
                }
            }
        }
    }
}
