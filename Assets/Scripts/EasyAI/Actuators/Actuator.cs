using System.Collections.Generic;
using System.Linq;
using EasyAI.Actions;
using EasyAI.Components;

namespace EasyAI.Actuators
{
    public abstract class Actuator : IntelligenceComponent
    {
        protected float TimeSinceLastActed => agent == null ? 0 : agent.AgentDeltaTime;
        
        public void Act(IEnumerable<Action> actions)
        {
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

            if (!actionComplete)
            {
                AddMessage("Performed no actions.");
            }
        }
        
        protected abstract bool Act(Action action);
    }
}