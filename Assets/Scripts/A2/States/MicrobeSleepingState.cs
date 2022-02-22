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
                microbe.State = AgentManager.Singleton.Lookup(typeof(MicrobeSeekingFoodState));
                agent.AddMessage("Hungry.");
            }
            else
            {
                agent.AddMessage("Sleeping.");
            }
        }

        public override void Exit(Agent agent)
        {
            agent.AddMessage("Waking up.");
        }
    }
}