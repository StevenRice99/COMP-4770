using Project.Minds;
using UnityEngine;

namespace Project.Pickups
{
    public abstract class PickupBase : MonoBehaviour
    {
        protected abstract void OnPickedUp(SoldierBrain soldierBrain, int[] ammo);
        
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
            SoldierBrain soldierBrain = other.gameObject.GetComponent<SoldierBrain>();
            if (soldierBrain == null)
            {
                return;
            }

            // soldierBrain.weapons.Select(w => w.ammo).ToArray()
            OnPickedUp(soldierBrain, new int[1]);
        }
    }
}