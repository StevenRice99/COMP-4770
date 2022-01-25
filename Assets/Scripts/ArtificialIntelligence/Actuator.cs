using System.Collections.Generic;

namespace ArtificialIntelligence
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