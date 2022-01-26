using System.Collections.Generic;
using ArtificialIntelligence.Actions;

namespace ArtificialIntelligence.Actuators
{
    public abstract class Actuator :  AIComponent
    {
        public void Act(IEnumerable<Action> actions)
        {
            ElapsedTime += Agent.ElapsedTime;
            if (ElapsedTime < timeRequired)
            {
                return;
            }

            bool actionPerformed = false;
            foreach (Action action in actions)
            {
                if (Act(action))
                {
                    actionPerformed = true;
                }
            }

            if (actionPerformed)
            {
                ElapsedTime = 0;
            }
        }
        
        protected abstract bool Act(Action action);
    }
}