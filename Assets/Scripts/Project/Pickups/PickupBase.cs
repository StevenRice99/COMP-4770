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
            SoldierAgent soldierBrain = other.gameObject.GetComponent<SoldierAgent>();
            if (soldierBrain == null)
            {
                return;
            }

            // soldierBrain.weapons.Select(w => w.ammo).ToArray()
            OnPickedUp(soldierBrain, new int[1]);
        }
    }
}