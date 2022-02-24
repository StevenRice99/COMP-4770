using A2.Agents;
using A2.Managers;
using UnityEngine;

namespace A2.States
{
    [CreateAssetMenu(menuName = "A2/States/Microbe Global State")]
    public class MicrobeGlobalState : State
    {
        public override void Execute(Agent agent)
        {
            base.Execute(agent);

            if (!(agent is Microbe microbe))
            {
                return;
            }

            if (Random.value <= MicrobeManager.MicrobeManagerSingleton.hungerChance)
            {
                microbe.Hunger++;
            }

            if (microbe.IsHungry)
            {
                if (microbe.State.GetType() == typeof(MicrobeSeekingFoodState))
                {
                    return;
                }
                microbe.State = AgentManager.Singleton.Lookup(typeof(MicrobeSeekingFoodState));
                microbe.SetStateVisual(microbe.State);
                return;
            }

            if (!microbe.IsAdult)
            {
                if (microbe.State.GetType() == typeof(MicrobeSleepingState))
                {
                    return;
                }
                microbe.State = AgentManager.Singleton.Lookup(typeof(MicrobeSleepingState));
                microbe.SetStateVisual(microbe.State);
                return;
            }

            if (!microbe.DidMate)
            {
                if (microbe.State.GetType() == typeof(MicrobeSeekingMateState))
                {
                    return;
                }
                microbe.State = AgentManager.Singleton.Lookup(typeof(MicrobeSeekingMateState));
                microbe.SetStateVisual(microbe.State);
                return;
            }

            if (microbe.State.GetType() == typeof(MicrobeSeekingPickupState))
            {
                return;
            }
            microbe.State = AgentManager.Singleton.Lookup(typeof(MicrobeSeekingPickupState));
            microbe.SetStateVisual(microbe.State);
        }
        
        public override bool HandleEvent(Agent agent, AIEvent aiEvent)
        {
            if (aiEvent.EventId != (int) MicrobeManager.MicrobeEvents.Eaten || !(agent is Microbe microbe) || !(aiEvent.Sender is Microbe sender))
            {
                return base.HandleEvent(agent, aiEvent);
            }
            
            sender.Eat(microbe);
            microbe.Die();

            return true;
        }
    }
}