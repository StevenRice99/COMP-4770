using A2.Managers;
using UnityEngine;

namespace A2.States
{
    [CreateAssetMenu(menuName = "A2/States/Microbe Seeking Food State")]
    public class MicrobeSeekingFoodState : State
    {
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

            if (microbe.TargetMicrobe == null)
            {
                microbe.TargetMicrobe = MicrobeManager.MicrobeManagerSingleton.FindFood(microbe);
            }

            if (microbe.TargetMicrobe == null)
            {
                agent.AddMessage("Cannot find any food, roaming.");
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
                microbe.FireEvent(microbe.TargetMicrobe, (int) MicrobeManager.MicrobeEvents.Eaten);
                if (microbe.Hunger <= MicrobeManager.MicrobeManagerSingleton.HungerThreshold)
                {
                    microbe.State = AgentManager.Singleton.Lookup(typeof(MicrobeSleepingState));
                }
                return;
            }
            
            agent.AddMessage($"Hunting {microbe.TargetMicrobe.name}.");
            agent.MoveToLookAtTarget(microbe.TargetMicrobe.transform);
        }

        public override void Exit(Agent agent)
        {
            if (!(agent is Microbe microbe))
            {
                return;
            }

            microbe.TargetMicrobe = null;
            agent.AddMessage("No longer searching for food.");
        }
    }
}