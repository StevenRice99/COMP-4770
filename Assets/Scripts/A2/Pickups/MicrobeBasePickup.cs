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
            Microbe[] microbes = AgentManager.Singleton.Agents.Cast<Microbe>().ToArray();
            if (microbes.Length == 0)
            {
                return;
            }

            Microbe microbe = microbes.Where(m => Vector3.Distance(m.transform.position, transform.position) < MicrobeManager.MicrobeManagerSingleton.MicrobeInteractRadius).OrderBy(m => Vector3.Distance(m.transform.position, transform.position)).FirstOrDefault();
            if (microbe == null)
            {
                return;
            }
            
            Execute(microbe);
            Destroy(gameObject);
        }
    }
}