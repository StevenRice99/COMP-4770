using System.Linq;
using A2.Agents;
using A2.Managers;
using A2.Pickups;
using UnityEngine;

namespace A2.States
{
    [CreateAssetMenu(menuName = "A2/States/Microbe Seeking Pickup State")]
    public class MicrobeSeekingPickupState : State
    {
        public override void Enter(Agent agent)
        {
            agent.AddMessage("Starting for a pickup.");
        }

        public override void Execute(Agent agent)
        {
            if (!(agent is Microbe microbe))
            {
                return;
            }

            if (microbe.TargetPickup == null)
            {
                MicrobeBasePickup[] pickups = FindObjectsOfType<MicrobeBasePickup>();
                if (pickups.Length > 0)
                {
                    microbe.TargetPickup = pickups
                        .OrderBy(p => Vector3.Distance(agent.transform.position, p.transform.position)).FirstOrDefault();
                }
                
            }

            if (microbe.TargetPickup == null)
            {
                agent.AddMessage("Cannot find any pickups, roaming.");
                if (agent.DidMove)
                {
                    return;
                }

                Vector3 position = Random.insideUnitSphere * MicrobeManager.MicrobeManagerSingleton.FloorRadius;
                agent.MoveToLookAtTarget(new Vector3(position.x, 0, position.z));
                return;
            }

            if (Vector3.Distance(microbe.transform.position, microbe.TargetPickup.transform.position) <= MicrobeManager.MicrobeManagerSingleton.MicrobeInteractRadius)
            {
                agent.AddMessage("Collecting pickup.");
                return;
            }
            
            agent.AddMessage($"Moving to {microbe.TargetPickup.name}.");
            agent.MoveToLookAtTarget(microbe.TargetPickup.transform);
        }

        public override void Exit(Agent agent)
        {
            if (!(agent is Microbe microbe))
            {
                return;
            }

            microbe.TargetPickup = null;
            agent.AddMessage("No longer searching for a pickup.");
        }
    }
}