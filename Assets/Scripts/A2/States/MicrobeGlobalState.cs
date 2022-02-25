using A2.Agents;
using A2.Managers;
using UnityEngine;

namespace A2.States
{
    /// <summary>
    /// The global state which microbes are always in.
    /// </summary>
    [CreateAssetMenu(menuName = "A2/States/Microbe Global State")]
    public class MicrobeGlobalState : State
    {
        /// <summary>
        /// Called when an agent is in this state.
        /// </summary>
        /// <param name="agent">The agent.</param>
        public override void Execute(Agent agent)
        {
            base.Execute(agent);

            if (!(agent is Microbe microbe))
            {
                return;
            }

            // Determine if the microbe's hunger should increase.
            if (Random.value <= MicrobeManager.MicrobeManagerSingleton.hungerChance)
            {
                microbe.Hunger++;
            }

            // If the microbe is hungry, set the microbe to seek food.
            if (microbe.IsHungry)
            {
                if (microbe.State.GetType() == typeof(MicrobeSeekingFoodState))
                {
                    return;
                }
                microbe.State = AgentManager.Lookup(typeof(MicrobeSeekingFoodState));
                microbe.SetStateVisual(microbe.State);
                return;
            }

            // If the microbe is not hungry but it is not yet an adult, ensure the microbe is sleeping.
            if (!microbe.IsAdult)
            {
                if (microbe.State.GetType() == typeof(MicrobeSleepingState))
                {
                    return;
                }
                microbe.State = AgentManager.Lookup(typeof(MicrobeSleepingState));
                microbe.SetStateVisual(microbe.State);
                return;
            }

            // If the microbe is an adult and has not yet mated, set the microbe to seek a mate.
            if (!microbe.DidMate)
            {
                if (microbe.State.GetType() == typeof(MicrobeSeekingMateState))
                {
                    return;
                }
                microbe.State = AgentManager.Lookup(typeof(MicrobeSeekingMateState));
                microbe.SetStateVisual(microbe.State);
                return;
            }

            // Lastly, if the microbe is not hungry, is an adult, and has mated, set the microbe to look for pickups.
            if (microbe.State.GetType() == typeof(MicrobeSeekingPickupState))
            {
                return;
            }
            microbe.State = AgentManager.Lookup(typeof(MicrobeSeekingPickupState));
            microbe.SetStateVisual(microbe.State);
        }
        
        /// <summary>
        /// Overridden to handle receiving an event saying a microbe was eaten.
        /// </summary>
        /// <param name="agent">The agent.</param>
        /// <param name="aiEvent">The event to handle.</param>
        /// <returns>True if it was an eaten message, false otherwise.</returns>
        public override bool HandleEvent(Agent agent, AIEvent aiEvent)
        {
            // If the message was anything other than an eaten message, return false;
            if (aiEvent.EventId != (int) MicrobeManager.MicrobeEvents.Eaten || !(agent is Microbe microbe) || !(aiEvent.Sender is Microbe sender))
            {
                return false;
            }
            
            // Have the sender microbe eat the receiving microbe.
            sender.Eat(microbe);
            microbe.Die();
            return true;
        }
    }
}