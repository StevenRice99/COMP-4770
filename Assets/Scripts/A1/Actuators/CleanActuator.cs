using System.Linq;
using A1.Actions;
using A1.Managers;
using SimpleIntelligence.Actions;
using SimpleIntelligence.Actuators;
using UnityEngine;

namespace A1.Actuators
{
    public class CleanActuator : Actuator
    {
        [SerializeField]
        [Min(0)]
        [Tooltip("The time it takes to clean a tile")]
        private float timeToClean;

        private float timeSpentCleaning;
        
        protected override bool Act(Action action)
        {
            if (!(action is CleanAction cleanAction))
            {
                return false;
            }

            timeSpentCleaning += agent.AgentDeltaTime;
            if (timeSpentCleaning < timeToClean)
            {
                AddMessage("Cleaning current floor tile.");
                return false;
            }

            Floor floor = FloorManager.FloorManagerSingleton.Floors
                .OrderBy(f => Vector3.Distance(agent.transform.position, f.transform.position))
                .FirstOrDefault();

            if (floor == null)
            {
                AddMessage("Unable to clean current floor tile.");
                return false;
            }
            
            AddMessage("Finished cleaning current floor tile.");
            timeSpentCleaning = 0;
            floor.Clean();
            cleanAction.Complete = true;
            return true;
        }
    }
}