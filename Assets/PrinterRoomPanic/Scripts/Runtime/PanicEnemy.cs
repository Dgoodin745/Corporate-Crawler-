using System;
using UnityEngine;
using UnityEngine.AI;

namespace CorporateCrawler.PrinterRoomPanic
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class PanicEnemy : MonoBehaviour
    {
        [SerializeField] private int hitPoints = 3;
        [SerializeField] private float attentionRange = 18f;
        [SerializeField] private float readableTurnSpeed = 540f;
        [SerializeField] private Renderer readabilityRenderer;
        [SerializeField] private Color idleColor = Color.yellow;
        [SerializeField] private Color aggroColor = Color.red;
        [SerializeField] private GameObject defeatFx;

        private NavMeshAgent agent;
        private Transform player;
        private int remainingHitPoints;

        public event Action<PanicEnemy> Defeated;
        public bool IsDefeated => remainingHitPoints <= 0;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            remainingHitPoints = hitPoints;
            SetReadableColor(idleColor);
        }

        public void SetPlayer(Transform target)
        {
            player = target;
        }

        private void Update()
        {
            if (player == null || IsDefeated)
            {
                return;
            }

            float distance = Vector3.Distance(transform.position, player.position);
            bool aggro = distance <= attentionRange;
            SetReadableColor(aggro ? aggroColor : idleColor);
            if (aggro)
            {
                agent.SetDestination(player.position);
                Vector3 look = player.position - transform.position;
                look.y = 0f;
                if (look.sqrMagnitude > 0.01f)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(look), readableTurnSpeed * Time.deltaTime);
                }
            }
            else
            {
                agent.ResetPath();
            }
        }

        public void TakeHit(int damage, Vector3 impactPoint)
        {
            if (IsDefeated)
            {
                return;
            }

            remainingHitPoints -= Mathf.Max(1, damage);
            if (remainingHitPoints <= 0)
            {
                if (defeatFx != null)
                {
                    Instantiate(defeatFx, impactPoint, Quaternion.identity);
                }
                Defeated?.Invoke(this);
                Destroy(gameObject);
            }
        }

        private void SetReadableColor(Color color)
        {
            if (readabilityRenderer != null)
            {
                readabilityRenderer.material.color = color;
            }
        }
    }
}
