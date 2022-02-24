using System.Linq;
using A2.Agents;
using UnityEngine;

namespace A2.Pickups
{
    public class NukePickup : MicrobeBasePickup
    {
        [SerializeField]
        [Min(float.Epsilon)]
        private float radius = 5;
        
        protected override void Execute(Microbe microbe)
        {
            microbe.AddMessage("Powered up - killed all nearby microbes!");
            Agent[] agents = AgentManager.Singleton.Agents.Where(a => a != microbe && Vector3.Distance(a.transform.position, microbe.transform.position) <= radius).ToArray();
            for (int i = agents.Length - 1; i >= 0; i--)
            {
                if (agents[i] is Microbe m)
                {
                    m.Die();
                }
            }
        }
    }
}