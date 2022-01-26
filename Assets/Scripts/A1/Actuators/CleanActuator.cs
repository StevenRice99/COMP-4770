﻿using System.Linq;
using A1.Actions;
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

            cleanAction.Complete = true;
            Floor floor = FloorManager.Singleton.Floors
                .OrderBy(f => Vector3.Distance(agent.transform.position, f.transform.position))
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