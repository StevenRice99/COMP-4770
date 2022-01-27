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
        protected override bool Act(Action action)
        {
            if (!(action is CleanAction cleanAction))
            {
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
            
            AddMessage("Cleaned current floor tile.");
            floor.Clean();
            cleanAction.Complete = true;
            return true;
        }
    }
}