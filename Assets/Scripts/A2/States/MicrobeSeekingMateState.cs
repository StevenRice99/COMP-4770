﻿using A2.Agents;
using A2.Managers;
using UnityEngine;

namespace A2.States
{
    [CreateAssetMenu(menuName = "A2/States/Microbe Seeking Mate State")]
    public class MicrobeSeekingMateState : State
    {
        public override void Enter(Agent agent)
        {
            agent.AddMessage("Looking for a mate.");
        }
        
        public override void Execute(Agent agent)
        {
            if (!(agent is Microbe microbe))
            {
                return;
            }

            if (microbe.TargetMicrobe == null)
            {
                Microbe potentialMate = MicrobeManager.MicrobeManagerSingleton.FindMate(microbe);
                if (potentialMate != null)
                {
                    agent.AddMessage($"Attempting to impress {potentialMate.name} to mate.");
                    bool accepted = microbe.FireEvent(potentialMate, (int) MicrobeManager.MicrobeEvents.Impress);
                    if (accepted)
                    {
                        agent.AddMessage($"{potentialMate.name} accepted advances to mate.");
                        microbe.TargetMicrobe = potentialMate;
                    }
                    else
                    {
                        agent.AddMessage($"Got rejected by {potentialMate.name}.");
                        microbe.Rejected(potentialMate);
                    }
                }
            }

            if (microbe.TargetMicrobe == null)
            {
                if (microbe.IsHungry)
                {
                    agent.AddMessage("Hungry, stopping search for mate as there were none.");
                    microbe.State = AgentManager.Singleton.Lookup(typeof(MicrobeSeekingFoodState));
                    return;
                }
                
                agent.AddMessage("Cannot find a mate.");
                if (agent.DidMove)
                {
                    return;
                }

                Vector3 position = Random.insideUnitSphere * MicrobeManager.MicrobeManagerSingleton.FloorRadius;
                agent.MoveToLookAtTarget(new Vector3(position.x, 0, position.z));
                return;
            }

            if (Vector3.Distance(microbe.transform.position, microbe.TargetMicrobe.transform.position) <= MicrobeManager.MicrobeManagerSingleton.MicrobeInteractRadius)
            {
                if (microbe.FireEvent(microbe.TargetMicrobe, (int) MicrobeManager.MicrobeEvents.Mate))
                {
                    agent.AddMessage($"Mating with {microbe.TargetMicrobe.name}.");
                    microbe.DidMate = true;
                    microbe.State = AgentManager.Singleton.Lookup(typeof(MicrobeSleepingState));
                }
                return;
            }
            
            agent.AddMessage($"Moving to mate with {microbe.TargetMicrobe.name}.");
            agent.MoveToLookAtTarget(microbe.TargetMicrobe.transform);
        }
        
        public override void Exit(Agent agent)
        {
            if (!(agent is Microbe microbe))
            {
                return;
            }

            microbe.TargetMicrobe = null;
            agent.AddMessage("No longer looking for a mate.");
        }
        
        public override bool HandleEvent(Agent agent, AIEvent aiEvent)
        {
            switch ((MicrobeManager.MicrobeEvents) aiEvent.EventId)
            {
                case MicrobeManager.MicrobeEvents.Impress:
                {
                    if (!(agent is Microbe { IsAdult: true } microbe) || microbe.DidMate || microbe.TargetMicrobe != null || !(aiEvent.Sender is Microbe sender))
                    {
                        agent.AddMessage($"Cannot mate with {aiEvent.Sender.name}.");
                        return false;
                    }

                    if (Random.value <= MicrobeManager.MicrobeManagerSingleton.rejectionChance)
                    {
                        agent.AddMessage($"Rejected {sender.name}.");
                        return false;
                    }

                    agent.AddMessage($"Accepted advances of {sender.name}.");
                    microbe.TargetMicrobe = sender;
                    return true;
                }
                case MicrobeManager.MicrobeEvents.Mate:
                {
                    if (!(agent is Microbe microbe))
                    {
                        return false;
                    }

                    microbe.DidMate = true;
                
                    int offspring = MicrobeManager.MicrobeManagerSingleton.Mate(microbe, aiEvent.Sender as Microbe);
                    agent.AddMessage(offspring == 0
                        ? $"Failed to have any offspring with {aiEvent.Sender.name}."
                        : $"Have {offspring} offspring with {aiEvent.Sender.name}.");
                    agent.State = AgentManager.Singleton.Lookup(typeof(MicrobeSleepingState));
                    return true;
                }
                case MicrobeManager.MicrobeEvents.Eaten:
                default:
                    return base.HandleEvent(agent, aiEvent);
            }
        }
    }
}