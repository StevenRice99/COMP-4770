using System.Linq;
using ArtificialIntelligence.Actions;
using ArtificialIntelligence.Actuators;
using UnityEngine;

namespace A1
{
    public class CleanActuator : Actuator
    {
        protected override bool Act(Action action)
        {
            if (!(action is CleanAction cleanAction))
            {
                return false;
            }

            cleanAction.Complete = true;
            Floor floor = FloorManager.Singleton.Floors
                .OrderBy(f => Vector3.Distance(Agent.transform.position, f.transform.position))
                .FirstOrDefault();

            if (floor == null)
            {
                return false;
            }
            
            floor.Clean();
            return true;
        }
    }
}