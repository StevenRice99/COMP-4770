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
            agent.AddMessage("Searching for food.");
            agent.MoveToLookAtTarget(Vector3.zero);
        }

        public override void Exit(Agent agent)
        {
            agent.AddMessage("No longer searching for food.");
        }
    }
}