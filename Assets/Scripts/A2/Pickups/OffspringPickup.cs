using A2.Agents;
using A2.Managers;
using UnityEngine;

namespace A2.Pickups
{
    /// <summary>
    /// Class to spawn offspring instantly without having to mate.
    /// </summary>
    public class OffspringPickup : MicrobeBasePickup
    {
        [SerializeField]
        [Min(1)]
        [Tooltip("How many offspring to spawn.")]
        private int spawnCount = 3;
        
        /// <summary>
        /// The behaviour of the pickup.
        /// </summary>
        /// <param name="microbe">The microbe which picked up this pickup.</param>
        protected override void Execute(Microbe microbe)
        {
            microbe.AddMessage("Powered up - magically spawned offspring!");
            for (int i = 0; i < spawnCount && AgentManager.Singleton.Agents.Count < MicrobeManager.MicrobeManagerSingleton.maxMicrobes; i++)
            {
                // Treat this as mating but with the same parent passed in for both values.
                MicrobeManager.MicrobeManagerSingleton.Mate(microbe, microbe);
            }
        }
    }
}