using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CorporateCrawler.Enemies
{
    /// <summary>
    /// Coffee Zombie behaviour: finds the nearest player, chases, performs short melee
    /// charges, and applies damage either during the charge hit window or on contact.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    public class CoffeeZombieAI : MonoBehaviour
    {
        [Header("Targeting")]
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private float retargetInterval = 0.35f;
        [SerializeField] private float chaseRange = 18f;
        [SerializeField] private float attackRange = 2.2f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private float turnSpeed = 12f;
        [SerializeField] private NavMeshAgent navMeshAgent;

        [Header("Charge Attack")]
        [SerializeField] private float attackCooldown = 1.4f;
        [SerializeField] private float chargeDuration = 0.32f;
        [SerializeField] private float chargeSpeed = 8.5f;
        [SerializeField] private float attackDamage = 20f;
        [SerializeField] private float contactDamage = 8f;
        [SerializeField] private float contactDamageCooldown = 0.75f;
        [SerializeField] private LayerMask playerLayers = ~0;

        private readonly HashSet<GameObject> damagedThisCharge = new HashSet<GameObject>();

        private EnemyHealth health;
        private Transform target;
        private Rigidbody body;
        private float nextRetargetTime;
        private float nextAttackTime;
        private float chargeEndTime;
        private float nextContactDamageTime;
        private Vector3 chargeDirection;

        private bool IsCharging => Time.time < chargeEndTime;

        private void Awake()
        {
            health = GetComponent<EnemyHealth>();
            body = GetComponent<Rigidbody>();

            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }

            if (navMeshAgent != null)
            {
                navMeshAgent.speed = moveSpeed;
            }
        }

        private void Update()
        {
            if (health.IsDead)
            {
                StopMoving();
                return;
            }

            if (Time.time >= nextRetargetTime || target == null)
            {
                target = FindNearestPlayer();
                nextRetargetTime = Time.time + retargetInterval;
            }

            if (target == null)
            {
                StopMoving();
                return;
            }

            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget > chaseRange)
            {
                StopMoving();
                return;
            }

            FaceTarget(target.position);

            if (IsCharging)
            {
                MoveInCharge();
                return;
            }

            if (distanceToTarget <= attackRange && Time.time >= nextAttackTime)
            {
                BeginChargeAttack();
                return;
            }

            ChaseTarget();
        }

        private Transform FindNearestPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
            Transform nearest = null;
            float nearestSqrDistance = float.PositiveInfinity;

            foreach (GameObject player in players)
            {
                float sqrDistance = (player.transform.position - transform.position).sqrMagnitude;
                if (sqrDistance < nearestSqrDistance)
                {
                    nearest = player.transform;
                    nearestSqrDistance = sqrDistance;
                }
            }

            return nearest;
        }

        private void ChaseTarget()
        {
            if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = moveSpeed;
                navMeshAgent.SetDestination(target.position);
                return;
            }

            Vector3 nextPosition = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            transform.position = nextPosition;
        }

        private void BeginChargeAttack()
        {
            damagedThisCharge.Clear();
            nextAttackTime = Time.time + attackCooldown;
            chargeEndTime = Time.time + chargeDuration;
            chargeDirection = (target.position - transform.position).normalized;
            chargeDirection.y = 0f;

            if (chargeDirection.sqrMagnitude <= 0.001f)
            {
                chargeDirection = transform.forward;
            }

            if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.ResetPath();
                navMeshAgent.isStopped = true;
            }
        }

        private void MoveInCharge()
        {
            Vector3 movement = chargeDirection.normalized * (chargeSpeed * Time.deltaTime);

            if (body != null && !body.isKinematic)
            {
                body.MovePosition(body.position + movement);
                return;
            }

            transform.position += movement;
        }

        private void FaceTarget(Vector3 targetPosition)
        {
            Vector3 lookDirection = targetPosition - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude <= 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        private void StopMoving()
        {
            if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.ResetPath();
                navMeshAgent.isStopped = true;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleContact(collision.gameObject);
        }

        private void OnCollisionStay(Collision collision)
        {
            HandleContact(collision.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleContact(other.gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            HandleContact(other.gameObject);
        }

        private void HandleContact(GameObject other)
        {
            if (health.IsDead || !IsPlayer(other))
            {
                return;
            }

            if (IsCharging)
            {
                DamagePlayerOncePerCharge(other, attackDamage);
                return;
            }

            if (Time.time >= nextContactDamageTime)
            {
                nextContactDamageTime = Time.time + contactDamageCooldown;
                DamagePlayer(other, contactDamage);
            }
        }

        private void DamagePlayerOncePerCharge(GameObject player, float damage)
        {
            if (!damagedThisCharge.Add(player))
            {
                return;
            }

            DamagePlayer(player, damage);
        }

        private bool IsPlayer(GameObject other)
        {
            return other.CompareTag(playerTag) && ((playerLayers.value & (1 << other.layer)) != 0);
        }

        private void DamagePlayer(GameObject player, float damage)
        {
            player.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }
}
