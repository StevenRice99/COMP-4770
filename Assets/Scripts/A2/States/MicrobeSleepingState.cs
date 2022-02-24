using A2.Agents;
using UnityEngine;

namespace A2.States
{
    [CreateAssetMenu(menuName = "A2/States/Microbe Sleeping State")]
    public class MicrobeSleepingState : State
    {
        public override void Enter(Agent agent)
        {
            agent.AddMessage("Going to sleep.");
        }

        public override void Execute(Agent agent)
        {
            if (!(agent is Microbe microbe))
            {
                return;
            }

            if (microbe.IsHungry)
            {
                agent.AddMessage("Hungry.");
                microbe.State = AgentManager.Singleton.Lookup(typeof(MicrobeSeekingFoodState));
                microbe.SetStateVisual(microbe.State);
                return;
            }

            if (microbe.IsAdult && !microbe.DidMate)
            {
                agent.AddMessage("Want to find a mate.");
                microbe.State = AgentManager.Singleton.Lookup(typeof(MicrobeSeekingMateState));
                microbe.SetStateVisual(microbe.State);
                return;
            }
            
            agent.AddMessage("Sleeping.");
        }

        public override void Exit(Agent agent)
        {
            agent.AddMessage("Waking up.");
        }
    }
}