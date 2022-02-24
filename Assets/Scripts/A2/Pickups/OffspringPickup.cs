using A2.Agents;
using A2.Managers;
using UnityEngine;

namespace A2.Pickups
{
    public class OffspringPickup : MicrobeBasePickup
    {
        [SerializeField]
        [Min(1)]
        private int spawnCount = 3;
        
        protected override void Execute(Microbe microbe)
        {
            microbe.AddMessage("Powered up - magically spawned offspring!");
            for (int i = 0; i < spawnCount; i++)
            {
                MicrobeManager.MicrobeManagerSingleton.Mate(microbe, microbe);
            }
        }
    }
}