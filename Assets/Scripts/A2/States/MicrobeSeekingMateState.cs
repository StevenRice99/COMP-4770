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
                        microbe.RejectedBy.Add(potentialMate);
                    }
                }
            }

            if (microbe.TargetMicrobe == null)
            {
                agent.AddMessage("Cannot find a mate, roaming.");
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
                if (microbe.FireEvent(microbe.TargetMicrobe, (int) MicrobeManager.MicrobeEvents.Mated))
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
            if (aiEvent.EventId == (int) MicrobeManager.MicrobeEvents.Impress)
            {
                if (!(agent is Microbe { IsAdult: true } microbe) || microbe.DidMate || microbe.TargetMicrobe != null)
                {
                    agent.AddMessage($"Rejected {aiEvent.Sender.name}.");
                    return false;
                }

                agent.AddMessage($"Accepted advances of {aiEvent.Sender.name}.");
                microbe.TargetMicrobe = aiEvent.Sender as Microbe;
                return true;
            }
            
            if (aiEvent.EventId == (int) MicrobeManager.MicrobeEvents.Mated)
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

            return base.HandleEvent(agent, aiEvent);
        }
    }
}