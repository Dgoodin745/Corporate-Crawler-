using UnityEngine;

namespace CorporateCrawler.PrinterRoomPanic
{
    public sealed class PanicStapler : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float cooldownSeconds = 0.18f;
        [SerializeField] private int magazineSize = 12;
        [SerializeField] private float reloadSeconds = 1.1f;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private AudioSource fireAudio;

        private int staplesLoaded;
        private float nextFireTime;
        private bool reloading;

        public int StaplesLoaded => staplesLoaded;
        public int MagazineSize => magazineSize;
        public bool IsReloading => reloading;

        private void Awake()
        {
            staplesLoaded = magazineSize;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                BeginReload();
            }
        }

        public bool Fire(Ray ray, float range, LayerMask mask)
        {
            if (Time.time < nextFireTime || reloading)
            {
                return false;
            }

            if (staplesLoaded <= 0)
            {
                BeginReload();
                return false;
            }

            nextFireTime = Time.time + cooldownSeconds;
            staplesLoaded--;
            muzzleFlash?.Play();
            fireAudio?.Play();

            if (Physics.Raycast(ray, out RaycastHit hit, range, mask, QueryTriggerInteraction.Ignore)
                && hit.collider.TryGetComponent(out PanicEnemy enemy))
            {
                enemy.TakeHit(damage, hit.point);
            }

            if (staplesLoaded == 0)
            {
                BeginReload();
            }
            return true;
        }

        private void BeginReload()
        {
            if (!reloading && staplesLoaded < magazineSize)
            {
                reloading = true;
                Invoke(nameof(FinishReload), reloadSeconds);
            }
        }

        private void FinishReload()
        {
            staplesLoaded = magazineSize;
            reloading = false;
        }
    }
}
