using System.Collections;
using Project.Agents;
using Project.Managers;
using UnityEngine;

namespace Project.Pickups
{
    public class HealthWeaponPickup : PickupBase
    {
        private const float Speed = 180;
        
        [SerializeField]
        [Tooltip("Set to below 0 to be a health pickup, otherwise the weapon index of the player.")]
        public int weaponIndex = -1;

        [SerializeField]
        [Tooltip("The visuals object to rotate.")]
        private Transform visuals;
        
        public bool Ready { get; set; } = true;
            
        private MeshRenderer[] _meshRenderers;
        
        protected override void OnPickedUp(SoldierAgent soldier, int[] ammo)
        {
            if (!Ready)
            {
                return;
            }

            if (weaponIndex < 0)
            {
                if (soldier.Health >= SoldierAgentManager.SoldierAgentManagerSingleton.health)
                {
                    return;
                }
            
                soldier.Heal();
                StartCoroutine(ReadyDelay());

                return;
            }

            if (soldier.Weapons.Length <= weaponIndex || soldier.Weapons[weaponIndex].maxAmmo < 0 || ammo[weaponIndex] >= soldier.Weapons[weaponIndex].maxAmmo)
            {
                return;
            }
            
            soldier.Weapons[weaponIndex].Replenish();
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
            Ready = false;
            ToggleMeshes();
            
            yield return new WaitForSeconds(SoldierAgentManager.SoldierAgentManagerSingleton.pickupTimer);
            
            Ready = true;
            ToggleMeshes();
        }

        private void ToggleMeshes()
        {
            foreach (MeshRenderer meshRenderer in _meshRenderers)
            {
                meshRenderer.enabled = Ready;
            }
        }
    }
}