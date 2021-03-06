using System.Linq;
using A2.Agents;
using A2.Pickups;
using UnityEngine;

namespace A2.States
{
    /// <summary>
    /// State for microbes that are seeking a pickup.
    /// </summary>
    [CreateAssetMenu(menuName = "A2/States/Microbe Seeking Pickup State")]
    public class MicrobeSeekingPickupState : State
    {
        /// <summary>
        /// Called when an agent first enters this state.
        /// </summary>
        /// <param name="agent">The agent.</param>
        public override void Enter(Agent agent)
        {
            agent.AddMessage("Starting for a pickup.");
        }

        /// <summary>
        /// Called when an agent is in this state.
        /// </summary>
        /// <param name="agent">The agent.</param>
        public override void Execute(Agent agent)
        {
            if (agent is not Microbe microbe)
            {
                return;
            }

            // If the microbe is not tracking a pickup, search for one.
            if (microbe.TargetPickup == null)
            {
                MicrobeBasePickup[] pickups = FindObjectsOfType<MicrobeBasePickup>();
                if (pickups.Length > 0)
                {
                    microbe.TargetPickup = pickups.OrderBy(p => Vector3.Distance(agent.transform.position, p.transform.position)).FirstOrDefault();
                }
                
            }

            // If there are no pickups in detection range, roam.
            if (microbe.TargetPickup == null)
            {
                agent.AddMessage("Cannot find any pickups, wandering.");
                if (agent.MovesData.Count > 0)
                {
                    return;
                }

                agent.ClearMoveData();
                agent.Wander = true;
                return;
            }
            
            // Otherwise move towards the pickup it is tracking.
            agent.AddMessage($"Moving to {microbe.TargetPickup.name}.");
            agent.SetMoveData(Agent.MoveType.Seek, microbe.TargetPickup.transform);
        }

        /// <summary>
        /// Called when an agent exits this state.
        /// </summary>
        /// <param name="agent">The agent.</param>
        public override void Exit(Agent agent)
        {
            if (agent is not Microbe microbe)
            {
                return;
            }

            // Ensure the target pickup is null.
            microbe.TargetPickup = null;
            agent.AddMessage("No longer searching for a pickup.");
        }
    }
}