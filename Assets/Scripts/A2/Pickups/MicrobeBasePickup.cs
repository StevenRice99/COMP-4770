using System.Linq;
using A2.Agents;
using A2.Managers;
using UnityEngine;

namespace A2.Pickups
{
    public abstract class MicrobeBasePickup : MonoBehaviour
    {
        protected abstract void Execute(Microbe microbe);

        private void Update()
        {
            Microbe[] microbes = AgentManager.Singleton.Agents.Where(a => Vector3.Distance(a.transform.position, transform.position) <= MicrobeManager.MicrobeManagerSingleton.MicrobeInteractRadius).Cast<Microbe>().ToArray();
            if (microbes.Length == 0)
            {
                return;
            }
            
            Microbe microbe = microbes.OrderBy(m => Vector3.Distance(m.transform.position, transform.position)).First();
            microbe.AddMessage("Collecting pickup.");
            microbe.PlayPickupAudio();
            Execute(microbe);
            Instantiate(MicrobeManager.MicrobeManagerSingleton.PickupParticlesPrefab, microbe.transform.position, Quaternion.Euler(270, 0, 0));
            Destroy(gameObject);
        }
    }
}