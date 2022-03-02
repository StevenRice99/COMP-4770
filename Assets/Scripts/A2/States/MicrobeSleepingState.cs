using UnityEngine;

namespace A2.States
{
    /// <summary>
    /// Sleeping state for the microbe, doesn't have any actions and only logs messages.
    /// </summary>
    [CreateAssetMenu(menuName = "A2/States/Microbe Sleeping State")]
    public class MicrobeSleepingState : State
    {
        /// <summary>
        /// Called when an agent first enters this state.
        /// </summary>
        /// <param name="agent">The agent.</param>
        public override void Enter(Agent agent)
        {
            agent.AddMessage("Going to sleep.");
        }

        /// <summary>
        /// Called when an agent is in this state.
        /// </summary>
        /// <param name="agent">The agent.</param>
        public override void Execute(Agent agent)
        {
            agent.AddMessage("Sleeping.");
            agent.ClearMoveData();
        }

        /// <summary>
        /// Called when an agent exits this state.
        /// </summary>
        /// <param name="agent">The agent.</param>
        public override void Exit(Agent agent)
        {
            agent.AddMessage("Waking up.");
        }
    }
}