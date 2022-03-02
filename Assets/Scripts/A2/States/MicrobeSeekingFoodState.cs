﻿using A2.Agents;
using A2.Managers;
using UnityEngine;

namespace A2.States
{
    /// <summary>
    /// State for microbes that are seeking food.
    /// </summary>
    [CreateAssetMenu(menuName = "A2/States/Microbe Seeking Food State")]
    public class MicrobeSeekingFoodState : State
    {
        /// <summary>
        /// Called when an agent first enters this state.
        /// </summary>
        /// <param name="agent">The agent.</param>
        public override void Enter(Agent agent)
        {
            agent.AddMessage("Starting to search for food.");
        }

        public override void Execute(Agent agent)
        {
            if (!(agent is Microbe microbe))
            {
                return;
            }

            // If the microbe is not tracking another microbe to eat yet, search for one.
            if (microbe.TargetMicrobe == null)
            {
                microbe.TargetMicrobe = MicrobeManager.MicrobeManagerSingleton.FindFood(microbe);
            }

            // If there are no microbes in detection range to eat, roam.
            if (microbe.TargetMicrobe == null)
            {
                agent.AddMessage("Cannot find any food, roaming.");
                if (agent.DidMove)
                {
                    return;
                }

                Vector3 position = Random.insideUnitSphere * MicrobeManager.MicrobeManagerSingleton.FloorRadius;
                agent.SetMoveData(Agent.MoveType.Seek, new Vector3(position.x, 0, position.z));
                return;
            }

            // If close enough to eat the microbe it is tracking, eat it.
            if (Vector3.Distance(microbe.transform.position, microbe.TargetMicrobe.transform.position) <= MicrobeManager.MicrobeManagerSingleton.MicrobeInteractRadius)
            {
                microbe.FireEvent(microbe.TargetMicrobe, (int) MicrobeManager.MicrobeEvents.Eaten);
                return;
            }
            
            // Otherwise move towards the microbe it is tracking.
            agent.AddMessage($"Hunting {microbe.TargetMicrobe.name}.");
            agent.SetMoveData(Agent.MoveType.Seek, microbe.TargetMicrobe.transform);
        }

        /// <summary>
        /// Called when an agent exits this state.
        /// </summary>
        /// <param name="agent">The agent.</param>
        public override void Exit(Agent agent)
        {
            if (!(agent is Microbe microbe))
            {
                return;
            }

            // Ensure the target microbe is null.
            microbe.TargetMicrobe = null;
            agent.AddMessage("No longer searching for food.");
        }
    }
}