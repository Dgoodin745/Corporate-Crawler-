using System.Collections;
using UnityEngine;

/// <summary>
/// Simple ranged weapon that fires short bursts of placeholder keycap projectiles.
/// Attach this to the player and assign a projectile prefab that has a Projectile component.
/// </summary>
public class KeyboardBlaster : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Vector2 fireDirection = Vector2.right;

    [Header("Weapon Tuning")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float fireRate = 4f;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float range = 8f;

    [Header("Burst")]
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstSpacing = 0.08f;

    private float nextFireTime;
    private bool isBursting;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            TryFireBurst();
        }
    }

    public void TryFireBurst()
    {
        if (isBursting || Time.time < nextFireTime || projectilePrefab == null)
        {
            return;
        }

        StartCoroutine(FireBurst());
    }

    private IEnumerator FireBurst()
    {
        isBursting = true;
        nextFireTime = Time.time + 1f / Mathf.Max(0.01f, fireRate);

        int shotsToFire = Mathf.Max(1, burstCount);
        for (int i = 0; i < shotsToFire; i++)
        {
            FireProjectile();

            if (i < shotsToFire - 1)
            {
                yield return new WaitForSeconds(Mathf.Max(0f, burstSpacing));
            }
        }

        isBursting = false;
    }

    private void FireProjectile()
    {
        Transform spawnPoint = firePoint != null ? firePoint : transform;
        Vector2 direction = GetFireDirection();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Projectile projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.Euler(0f, 0f, angle));
        projectile.Launch(direction, damage, projectileSpeed, range, gameObject);
    }

    private Vector2 GetFireDirection()
    {
        Vector2 direction = fireDirection.sqrMagnitude > 0f ? fireDirection.normalized : Vector2.right;

        if (transform.localScale.x < 0f)
        {
            direction.x *= -1f;
        }

        return direction;
    }
}
