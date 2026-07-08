using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CorporateCrawler.Arenas
{
    /// <summary>
    /// Completes the arena once the player enters the trigger and every spawner has no living enemies.
    /// </summary>
    public class RoomClearTrigger : MonoBehaviour
    {
        [SerializeField] private CoffeeZombieSpawner[] spawners;
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private UnityEvent onRoomCleared;

        public bool PlayerEntered { get; private set; }
        public bool RoomCleared { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                PlayerEntered = true;
                CheckForClear();
            }
        }

        private void Update()
        {
            if (PlayerEntered && !RoomCleared)
            {
                CheckForClear();
            }
        }

        private void CheckForClear()
        {
            if (spawners != null && spawners.All(spawner => spawner == null || spawner.AliveCount == 0))
            {
                RoomCleared = true;
                onRoomCleared?.Invoke();
                Debug.Log("PrinterRoomPanic cleared: all Coffee Zombies defeated.", this);
            }
        }
    }
}
