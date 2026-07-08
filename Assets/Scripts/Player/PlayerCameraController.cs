using UnityEngine;

namespace CorporateCrawler.Player
{
    /// <summary>
    /// Lightweight third-person camera follow/orbit controller for the first playable pass.
    /// Attach it to a Camera and assign the player transform, or leave target empty to auto-find a PlayerController.
    /// </summary>
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

        [Header("Orbit")]
        [SerializeField] private float distance = 5f;
        [SerializeField] private float mouseSensitivity = 3f;
        [SerializeField] private float minPitch = -25f;
        [SerializeField] private float maxPitch = 70f;
        [SerializeField] private bool lockCursorOnStart = true;

        [Header("Smoothing")]
        [SerializeField] private float followSharpness = 12f;

        private float yaw;
        private float pitch = 20f;

        public Transform Target
        {
            get => target;
            set => target = value;
        }

        public Quaternion PlanarRotation => Quaternion.Euler(0f, yaw, 0f);

        private void Start()
        {
            if (target == null)
            {
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                {
                    target = player.transform;
                }
            }

            Vector3 euler = transform.rotation.eulerAngles;
            yaw = euler.y;
            pitch = NormalizePitch(euler.x);

            if (lockCursorOnStart)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * mouseSensitivity, minPitch, maxPitch);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPosition = target.position + targetOffset - rotation * Vector3.forward * distance;
            float t = 1f - Mathf.Exp(-followSharpness * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, t);
            transform.rotation = rotation;
        }

        private static float NormalizePitch(float angle)
        {
            return angle > 180f ? angle - 360f : angle;
        }
    }
}
