using A2.Agents;
using UnityEngine;

namespace A2.Pickups
{
    public abstract class MicrobeBasePickup : MonoBehaviour
    {
        public void TriggerPickup(Microbe microbe)
        {
            Execute(microbe);
            Destroy(gameObject);
        }
        
        protected abstract void Execute(Microbe microbe);
    }
}