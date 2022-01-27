using System.Collections.Generic;
using System.Linq;
using SimpleIntelligence.Actions;
using SimpleIntelligence.Components;

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

            bool actionComplete = false;
            foreach (Action action in actions.Where(a => !a.Complete))
            {
                if (!Act(action))
                {
                    continue;
                }

                AddMessage($"Finished action {action.GetType().ToString().Split('.').Last()}.");
                actionComplete = true;
            }

            if (actionComplete)
            {
                ElapsedTime = 0;
            }
            else
            {
                AddMessage("Performed no actions.");
            }
        }
        
        protected abstract bool Act(Action action);
    }
}