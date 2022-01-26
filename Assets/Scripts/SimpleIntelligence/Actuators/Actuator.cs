using System.Collections.Generic;
using System.Linq;
using SimpleIntelligence.Actions;

namespace SimpleIntelligence.Actuators
{
    public abstract class Actuator :  AIComponent
    {
        public void Act(IEnumerable<Action> actions)
        {
            if (ElapsedTime < time)
            {
                return;
            }

            bool actionPerformed = false;
            foreach (Action action in actions.Where(a => !a.Complete))
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