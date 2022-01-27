using System.Collections.Generic;
using System.Linq;
using SimpleIntelligence.Actions;
using SimpleIntelligence.Base;

namespace SimpleIntelligence.Actuators
{
    public abstract class Actuator : IntelligenceComponent
    {
        public void Act(IEnumerable<Action> actions)
        {
            if (agent == null || ElapsedTime < time)
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
            else
            {
                AddMessage("Did nothing.");
            }
        }
        
        protected abstract bool Act(Action action);
    }
}