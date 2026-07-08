using UnityEngine;

namespace CorporateCrawler.Combat
{
    /// <summary>
    /// Prototype ranged weapon that fires glowing keycap projectiles in short bursts.
    /// </summary>
    public class KeyboardBlaster : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform muzzle;
        [SerializeField] private int damage = 12;
        [SerializeField] private float projectileSpeed = 22f;
        [SerializeField] private float projectileLifetime = 2.5f;
        [SerializeField] private float shotsPerSecond = 6f;
        [SerializeField] private int burstCount = 3;
        [SerializeField] private float burstShotSpacing = 0.08f;

        private float nextFireTime;
        private int burstShotsRemaining;
        private float nextBurstShotTime;

        private void Update()
        {
            bool wantsToFire = Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space);
            if (wantsToFire && Time.time >= nextFireTime && burstShotsRemaining == 0)
            {
                burstShotsRemaining = burstCount;
                nextBurstShotTime = Time.time;
                nextFireTime = Time.time + 1f / shotsPerSecond;
            }

            if (burstShotsRemaining > 0 && Time.time >= nextBurstShotTime)
            {
                FireSingleKey();
                burstShotsRemaining--;
                nextBurstShotTime = Time.time + burstShotSpacing;
            }
        }

        private void FireSingleKey()
        {
            if (projectilePrefab == null)
            {
                Debug.LogWarning("KeyboardBlaster needs a Projectile prefab assigned.", this);
                return;
            }

            Transform spawnPoint = muzzle != null ? muzzle : transform;
            Projectile projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
            projectile.Initialize(gameObject, damage, projectileSpeed, projectileLifetime);
        }
    }
}
