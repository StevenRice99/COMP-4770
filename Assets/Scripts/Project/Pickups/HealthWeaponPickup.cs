using System.Collections;
using Project.Managers;
using Project.Minds;
using UnityEngine;

namespace Project.Pickups
{
    public class HealthWeaponPickup : PickupBase
    {
        private const float Speed = 180;
        
        private const int Delay = 10;
        
        [SerializeField]
        [Tooltip("Set to below 0 to be a health pickup, otherwise the weapon index of the player.")]
        private int weaponIndex = -1;

        [SerializeField]
        [Tooltip("The visuals object to rotate.")]
        private Transform visuals;
        
        private bool _ready = true;
            
        private MeshRenderer[] _meshRenderers;
        
        protected override void OnPickedUp(SoldierBrain soldierBrain, int[] ammo)
        {
            if (!_ready)
            {
                return;
            }

            if (weaponIndex < 0)
            {
                if (soldierBrain.Health >= SoldierAgentManager.SoldierAgentManagerSingleton.health)
                {
                    return;
                }
            
                soldierBrain.Heal();
                StartCoroutine(ReadyDelay());

                return;
            }

            if (soldierBrain.Weapons.Length <= weaponIndex)
            {
                return;
            }

            if (soldierBrain.Weapons[weaponIndex].maxAmmo < 0)
            {
                return;
            }

            if (ammo[weaponIndex] >= soldierBrain.Weapons[weaponIndex].maxAmmo)
            {
                return;
            }
            
            soldierBrain.Weapons[weaponIndex].Replenish();
            StartCoroutine(ReadyDelay());
        }
        
        private void Start()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        }

        private void Update()
        {
            visuals.Rotate(0, Speed * Time.deltaTime, 0, Space.Self);
        }

        private IEnumerator ReadyDelay()
        {
            _ready = false;
            ToggleMeshes();
            yield return new WaitForSeconds(Delay);
            _ready = true;
            ToggleMeshes();
        }

        private void ToggleMeshes()
        {
            foreach (MeshRenderer meshRenderer in _meshRenderers)
            {
                meshRenderer.enabled = _ready;
            }
        }
    }
}