using UnityEngine;

namespace CorporateCrawler.PrinterRoomPanic
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class SinglePlayerPanicController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4.8f;
        [SerializeField] private float sprintSpeed = 7.2f;
        [SerializeField] private float acceleration = 18f;
        [SerializeField] private float gravity = -22f;

        [Header("Camera Feel")]
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float mouseSensitivity = 0.12f;
        [SerializeField] private float maxPitch = 78f;
        [SerializeField] private float aimFov = 48f;
        [SerializeField] private float hipFov = 64f;
        [SerializeField] private float fovLerpSpeed = 12f;

        [Header("Aiming")]
        [SerializeField] private float interactRange = 28f;
        [SerializeField] private LayerMask aimMask = ~0;
        [SerializeField] private PanicStapler stapler;

        private CharacterController controller;
        private Vector3 planarVelocity;
        private float verticalVelocity;
        private float pitch;

        public bool IsAiming { get; private set; }
        public Ray AimRay => new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>();
            }
            if (cameraPivot == null && playerCamera != null)
            {
                cameraPivot = playerCamera.transform.parent != null ? playerCamera.transform.parent : playerCamera.transform;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            UpdateLook();
            UpdateMovement();
            UpdateAimAndFire();
        }

        private void UpdateLook()
        {
            float yaw = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            float pitchDelta = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            transform.Rotate(Vector3.up, yaw, Space.World);
            pitch = Mathf.Clamp(pitch - pitchDelta, -maxPitch, maxPitch);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void UpdateMovement()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            input = Vector2.ClampMagnitude(input, 1f);
            float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            Vector3 desired = (transform.right * input.x + transform.forward * input.y) * targetSpeed;
            planarVelocity = Vector3.MoveTowards(planarVelocity, desired, acceleration * Time.deltaTime);

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
            verticalVelocity += gravity * Time.deltaTime;
            controller.Move((planarVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
        }

        private void UpdateAimAndFire()
        {
            IsAiming = Input.GetMouseButton(1);
            if (playerCamera != null)
            {
                float targetFov = IsAiming ? aimFov : hipFov;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, fovLerpSpeed * Time.deltaTime);
            }

            if (Input.GetMouseButtonDown(0) && stapler != null)
            {
                stapler.Fire(AimRay, interactRange, aimMask);
            }
        }
    }
}
