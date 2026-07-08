using UnityEngine;
using UnityEngine.AI;

namespace CorporateCrawler.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CoffeeZombieAI : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private float detectionRadius = 18f;
        [SerializeField] private float chargeDistance = 5f;
        [SerializeField] private float chargeSpeedMultiplier = 2.2f;
        [SerializeField] private float chargeDuration = 0.65f;
        [SerializeField] private float chargeCooldown = 2.5f;

        private NavMeshAgent agent;
        private Transform target;
        private float baseSpeed;
        private float chargeEndTime;
        private float nextChargeTime;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            baseSpeed = agent.speed;
        }

        private void Update()
        {
            AcquireTarget();

            if (target == null)
            {
                agent.ResetPath();
                return;
            }

            float distance = Vector3.Distance(transform.position, target.position);
            bool isCharging = Time.time < chargeEndTime;
            agent.speed = isCharging ? baseSpeed * chargeSpeedMultiplier : baseSpeed;
            agent.SetDestination(target.position);

            if (!isCharging && distance <= chargeDistance && Time.time >= nextChargeTime)
            {
                chargeEndTime = Time.time + chargeDuration;
                nextChargeTime = Time.time + chargeCooldown;
            }
        }

        private void AcquireTarget()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
            float bestDistance = detectionRadius;
            Transform bestTarget = null;

            foreach (GameObject player in players)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTarget = player.transform;
                }
            }

            target = bestTarget;
        }
    }
}
