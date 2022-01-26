using System.Linq;
using ArtificialIntelligence.Actions;
using ArtificialIntelligence.Actuators;
using UnityEngine;

namespace A1
{
    public class CleanActuator : Actuator
    {
        [SerializeField]
        [Min(0)]
        private float timeToClean;

        private float _elapsedTime;
        
        protected override void Act(Action action)
        {
            _elapsedTime += Agent.ElapsedTime;
            if (!(action is CleanAction cleanAction) || _elapsedTime < timeToClean)
            {
                return;
            }

            _elapsedTime = 0;
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