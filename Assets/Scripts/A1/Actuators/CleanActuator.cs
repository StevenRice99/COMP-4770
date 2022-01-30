﻿using A1.Actions;
using EasyAI.Actions;
using EasyAI.Actuators;
using UnityEngine;

namespace A1.Actuators
{
    public class CleanActuator : Actuator
    {
        [SerializeField]
        [Min(0)]
        [Tooltip("The time in seconds it takes to clean a floor tile.")]
        private float timeToClean;

        private float timeSpentCleaning;
        
        protected override bool Act(Action action)
        {
            if (!(action is CleanAction cleanAction))
            {
                return false;
            }

            timeSpentCleaning += DeltaTime;
            if (timeSpentCleaning < timeToClean)
            {
                AddMessage("Cleaning current floor tile.");
                return false;
            }

            if (cleanAction.floor == null)
            {
                AddMessage("Unable to clean current floor tile.");
                return false;
            }
            
            AddMessage("Finished cleaning current floor tile.");
            timeSpentCleaning = 0;
            cleanAction.floor.Clean();
            cleanAction.Complete = true;
            return true;
        }
    }
}