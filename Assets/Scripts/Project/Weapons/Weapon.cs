using System.Collections;
using Project.Managers;
using Project.Minds;
using UnityEngine;

namespace Project.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        protected struct AttackedInfo
        {
            public SoldierBrain Attacked;

            public int Hits;
        }

        [Tooltip("The maximum ammo of the weapon, setting to less than 0 will give unlimited ammo.")]
        public int maxAmmo = -1;

        public int Index { get; set; }

        public SoldierBrain SoldierBrain { get; set; }

        [Tooltip("The sound to make upon bullet impact.")]
        public AudioClip impactSound;

        [Tooltip("The effect prefab to show upon bullet impact.")]
        public GameObject impactEffectPrefab;
        
        [SerializeField]
        [Tooltip("The barrel of the weapon.")]
        protected Transform barrel;
        
        [SerializeField]
        [Min(1)]
        [Tooltip("How much damage the weapon should do.")]
        protected int damage;
        
        [SerializeField]
        [Min(0)]
        [Tooltip("How long between shots should there be.")]
        protected float delay;
        
        [SerializeField]
        [Min(0)]
        [Tooltip("How long bullet trails or projectiles last for.")]
        protected float time;
        
        private MeshRenderer[] _renderers;

        private bool _canShoot = true;

        private int _ammo;

        private AudioSource _shootSound;

        public void ImpactAudio(Vector3 p, int numImpacts)
        {
            GameObject impactObj = new($"{name} Audio")
            {
                transform =
                {
                    position = p
                }
            };
            AudioSource impact = impactObj.AddComponent<AudioSource>();
            impact.clip = impactSound;
            impact.volume = SoldierAgentManager.SoldierAgentManagerSingleton.sound / numImpacts;
            impact.spatialBlend = 1;
            impact.dopplerLevel = _shootSound.dopplerLevel;
            impact.spread = _shootSound.spread;
            impact.rolloffMode = _shootSound.rolloffMode;
            impact.minDistance = _shootSound.minDistance;
            impact.maxDistance = _shootSound.maxDistance;
            impact.Play();
            Destroy(impactObj, impactSound.length);
        }

        public void ImpactVisual(Vector3 p, Vector3 lookAt)
        {
            GameObject effect = Instantiate(impactEffectPrefab, p, Quaternion.identity);
            if (lookAt == Vector3.zero)
            {
                effect.transform.rotation = Quaternion.Euler(Random.Range(0, 360), 0, Random.Range(0, 360));
            }
            else
            {
                effect.transform.LookAt(lookAt);
            }
            effect.name = $"{name} Effect";
            Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
        }

        public void Replenish()
        {
            _ammo = maxAmmo;
        }
        
        public void Visible(bool visible)
        {
            foreach (MeshRenderer meshRenderer in _renderers)
            {
                meshRenderer.enabled = visible;
            }
        }
        
        public void Shoot()
        {
            if (Index != SoldierBrain.WeaponIndex || !_canShoot)
            {
                return;
            }

            Shoot(out Vector3[] positions);
            ShootVisuals(positions);
            StartDelay();
        }

        protected abstract void Shoot(out Vector3[] positions);

        protected virtual void ShootVisuals(Vector3[] positions)
        {
            _shootSound.Play();
        }

        protected virtual void Awake()
        {
            _ammo = maxAmmo;
        }

        private void Start()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>();
            _shootSound = GetComponent<AudioSource>();
            _shootSound.volume = SoldierAgentManager.SoldierAgentManagerSingleton.sound;
        }

        private void StartDelay()
        {
            if (_ammo > 0)
            {
                _ammo--;
            }
            
            StartCoroutine(ShotDelay());
        }
        
        private IEnumerator ShotDelay()
        {
            _canShoot = false;
            yield return new WaitForSeconds(delay);
            _canShoot = true;
        }
    }
}