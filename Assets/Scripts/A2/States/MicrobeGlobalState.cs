using A2.Managers;
using UnityEngine;

namespace A2.States
{
    public class MicrobeGlobalState : State
    {
        [SerializeField]
        private float hungerChance = 0.05f;
        
        public override void Execute(Agent agent)
        {
            base.Execute(agent);
            
            if (Random.value < hungerChance && agent is Microbe microbe)
            {
                microbe.Hunger++;
            }
        }
        
        public override bool HandleEvent(Agent agent, AIEvent aiEvent)
        {
            if (aiEvent.EventId != (int) MicrobeManager.MicrobeEvents.Eaten || !(agent is Microbe microbe) || !(aiEvent.Sender is Microbe sender))
            {
                return false;
            }
            
            sender.Eat(microbe);
            microbe.Die();

            return base.HandleEvent(agent, aiEvent);
        }
    }
}