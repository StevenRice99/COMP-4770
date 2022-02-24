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
            
            if (Random.value < MicrobeManager.MicrobeManagerSingleton.hungerChance && agent is Microbe microbe)
            {
                microbe.Hunger++;
            }
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