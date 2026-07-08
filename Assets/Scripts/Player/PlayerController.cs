using UnityEngine;

namespace CorporateCrawler.Player
{
    /// <summary>
    /// First-pass player controller: WASD movement, camera-relative facing, sprint, dodge placeholder, health-aware death lockout, and placeholder visuals.
    /// Requires no authored model; it creates a capsule body and cube direction marker when no renderer exists below the player.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerHealth))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4.5f;
        [SerializeField] private float sprintMultiplier = 1.6f;
        [SerializeField] private float rotationSharpness = 14f;
        [SerializeField] private float gravity = -25f;
        [SerializeField] private Transform cameraTransform;

        [Header("Dodge Placeholder")]
        [SerializeField] private KeyCode dodgeKey = KeyCode.Space;
        [SerializeField] private float dodgeSpeed = 10f;
        [SerializeField] private float dodgeDuration = 0.18f;
        [SerializeField] private float dodgeCooldown = 0.6f;

        [Header("Placeholder Visuals")]
        [SerializeField] private bool createPlaceholderVisuals = true;
        [SerializeField] private Color bodyColor = new Color(0.2f, 0.65f, 1f);
        [SerializeField] private Color forwardMarkerColor = new Color(1f, 0.85f, 0.2f);

        private CharacterController characterController;
        private PlayerHealth health;
        private Vector3 verticalVelocity;
        private Vector3 dodgeDirection;
        private float dodgeTimer;
        private float nextDodgeTime;

        private bool IsDodging => dodgeTimer > 0f;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            health = GetComponent<PlayerHealth>();

            characterController.height = Mathf.Max(characterController.height, 2f);
            characterController.radius = Mathf.Max(characterController.radius, 0.35f);
            characterController.center = new Vector3(0f, characterController.height * 0.5f, 0f);

            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }

            if (createPlaceholderVisuals)
            {
                EnsurePlaceholderVisuals();
            }
        }

        private void OnEnable()
        {
            health.Died += HandleDeath;
        }

        private void OnDisable()
        {
            health.Died -= HandleDeath;
        }

        private void Update()
        {
            if (health.IsDead)
            {
                ApplyGravityOnly();
                return;
            }

            Vector3 moveDirection = GetCameraRelativeMoveDirection();
            TryStartDodge(moveDirection);

            Vector3 horizontalVelocity;
            if (IsDodging)
            {
                dodgeTimer -= Time.deltaTime;
                horizontalVelocity = dodgeDirection * dodgeSpeed;
            }
            else
            {
                float speed = Input.GetKey(KeyCode.LeftShift) ? walkSpeed * sprintMultiplier : walkSpeed;
                horizontalVelocity = moveDirection * speed;
            }

            RotateToward(horizontalVelocity);
            ApplyGravity();
            characterController.Move((horizontalVelocity + verticalVelocity) * Time.deltaTime);
        }

        private Vector3 GetCameraRelativeMoveDirection()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            input = Vector2.ClampMagnitude(input, 1f);

            if (input.sqrMagnitude <= 0.001f)
            {
                return Vector3.zero;
            }

            Vector3 forward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
            Vector3 right = cameraTransform != null ? cameraTransform.right : Vector3.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            return (forward * input.y + right * input.x).normalized;
        }

        private void TryStartDodge(Vector3 moveDirection)
        {
            if (!Input.GetKeyDown(dodgeKey) || Time.time < nextDodgeTime || IsDodging)
            {
                return;
            }

            dodgeDirection = moveDirection.sqrMagnitude > 0.001f ? moveDirection : transform.forward;
            dodgeTimer = dodgeDuration;
            nextDodgeTime = Time.time + dodgeCooldown;
        }

        private void RotateToward(Vector3 horizontalVelocity)
        {
            Vector3 planar = new Vector3(horizontalVelocity.x, 0f, horizontalVelocity.z);
            if (planar.sqrMagnitude <= 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(planar.normalized, Vector3.up);
            float t = 1f - Mathf.Exp(-rotationSharpness * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
        }

        private void ApplyGravity()
        {
            if (characterController.isGrounded && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -2f;
            }

            verticalVelocity.y += gravity * Time.deltaTime;
        }

        private void ApplyGravityOnly()
        {
            ApplyGravity();
            characterController.Move(verticalVelocity * Time.deltaTime);
        }

        private void HandleDeath()
        {
            dodgeTimer = 0f;
            verticalVelocity = Vector3.zero;
        }

        private void EnsurePlaceholderVisuals()
        {
            if (GetComponentInChildren<Renderer>() != null)
            {
                return;
            }

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Placeholder Capsule Body";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0f, 1f, 0f);
            body.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
            Destroy(body.GetComponent<Collider>());
            ApplyColor(body, bodyColor);

            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = "Placeholder Forward Marker";
            marker.transform.SetParent(transform, false);
            marker.transform.localPosition = new Vector3(0f, 1.25f, 0.45f);
            marker.transform.localScale = new Vector3(0.35f, 0.25f, 0.2f);
            Destroy(marker.GetComponent<Collider>());
            ApplyColor(marker, forwardMarkerColor);
        }

        private static void ApplyColor(GameObject target, Color color)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
    }
}
