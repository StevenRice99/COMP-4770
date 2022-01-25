using System.Linq;
using ArtificialIntelligence;
using UnityEngine;

namespace A1
{
    public class CleanActuator : Actuator
    {
        [SerializeField]
        [Min(1)]
        private int ticksToClean;

        private int tick;
        
        protected override void Act(Action action)
        {
            if (!(action is CleanAction cleanAction))
            {
                return;
            }

            tick++;
            if (tick < ticksToClean)
            {
                return;
            }

            tick = 0;
            cleanAction.Complete = true;
            Floor floor = FloorManager.Singleton.Floors
                .OrderBy(f => Vector3.Distance(Agent.transform.position, f.transform.position))
                .FirstOrDefault();

            if (floor == null)
            {
                return;
            }
            
            floor.Clean();
        }
    }
}