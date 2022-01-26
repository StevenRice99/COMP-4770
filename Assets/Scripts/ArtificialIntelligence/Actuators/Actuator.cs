using System.Collections.Generic;
using ArtificialIntelligence.Actions;

namespace ArtificialIntelligence.Actuators
{
    public abstract class Actuator :  AIComponent
    {
        public void Act(IEnumerable<Action> actions)
        {
            foreach (Action action in actions)
            {
                Act(action);
            }
        }
        
        protected abstract void Act(Action action);
    }
}