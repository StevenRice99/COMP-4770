using System.Linq;
using A2.Agents;
using A2.Managers;
using UnityEngine;

namespace A2.Pickups
{
    /// <summary>
    /// Base class for pickups for microbes.
    /// </summary>
    public abstract class MicrobeBasePickup : MonoBehaviour
    {
        /// <summary>
        /// The behaviour of the pickup.
        /// </summary>
        /// <param name="microbe">The microbe which picked up this pickup.</param>
        protected abstract void Execute(Microbe microbe);

        private void Update()
        {
            // Get all microbes near to this pickup.
            Microbe[] microbes = AgentManager.Singleton.Agents.Where(a => Vector3.Distance(a.transform.position, transform.position) <= MicrobeManager.MicrobeManagerSingleton.MicrobeInteractRadius).Cast<Microbe>().ToArray();
            if (microbes.Length == 0)
            {
                return;
            }
            
            // Activate this pickup for the nearest microbe.
            Microbe microbe = microbes.OrderBy(m => Vector3.Distance(m.transform.position, transform.position)).First();
            microbe.AddMessage("Collecting pickup.");
            microbe.PlayPickupAudio();
            Execute(microbe);
            Instantiate(MicrobeManager.MicrobeManagerSingleton.PickupParticlesPrefab, microbe.transform.position, Quaternion.Euler(270, 0, 0));
            Destroy(gameObject);
        }
    }
}