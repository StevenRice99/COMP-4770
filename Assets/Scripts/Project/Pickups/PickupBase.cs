using System.Linq;
using Project.Agents;
using UnityEngine;

namespace Project.Pickups
{
    public abstract class PickupBase : MonoBehaviour
    {
        protected abstract void OnPickedUp(SoldierAgent soldier, int[] ammo);
        
        private void OnTriggerEnter(Collider other)
        {
            DetectPickup(other);
        }

        private void OnTriggerStay(Collider other)
        {
            DetectPickup(other);
        }

        private void DetectPickup(Component other)
        {
            SoldierAgent soldier = other.gameObject.GetComponent<SoldierAgent>();
            if (soldier != null && soldier.Alive)
            {
                OnPickedUp(soldier, soldier.Weapons.Select(w => w.Ammo).ToArray());
            }
        }
    }
}