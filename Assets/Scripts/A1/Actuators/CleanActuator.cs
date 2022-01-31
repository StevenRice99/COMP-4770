﻿using A1.Actions;
using EasyAI.Actuators;
using UnityEngine;
using Action = EasyAI.Actions.Action;

namespace A1.Actuators
{
    /// <summary>
    /// Actuator to clean a floor tile.
    /// </summary>
    public class CleanActuator : Actuator
    {
        [SerializeField]
        [Min(0)]
        [Tooltip("The time in seconds it takes to clean a floor tile.")]
        private float timeToClean = 0.25f;

        [SerializeField]
        [Tooltip("Dirt particles system to display when cleaning.")]
        private ParticleSystem dirtParticles;

        /// <summary>
        /// How long the floor tile has been getting cleaned for.
        /// </summary>
        private float timeSpentCleaning;

        private void Start()
        {
            StopCleaning();
        }

        /// <summary>
        /// Clean a floor tile.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>True if the floor tile has finished being cleaned, false otherwise.</returns>
        protected override bool Act(Action action)
        {
            // Only act if there is a clean action.
            if (!(action is CleanAction cleanAction))
            {
                StopCleaning();
                return false;
            }

            // This should never happen, but check just in case.
            if (cleanAction.Floor == null)
            {
                AddMessage("Unable to clean current floor tile.");
                StopCleaning();
                return false;
            }

            // Increment how long the floor has been getting cleaned for.
            timeSpentCleaning += DeltaTime;

            // If the tile has not been cleaned long enough, return false as it has not finished getting cleaned.
            if (timeSpentCleaning < timeToClean)
            {
                AddMessage("Cleaning current floor tile.");
                StartCleaning();
                return false;
            }
            
            // The floor has finished being cleaned so reset the time spent cleaning.
            AddMessage("Finished cleaning current floor tile.");
            timeSpentCleaning = 0;
            cleanAction.Floor.Clean();
            cleanAction.Complete = true;
            StopCleaning();
            return true;
        }

        private void StartCleaning()
        {
            if (dirtParticles == null)
            {
                return;
            }

            if (dirtParticles.isPlaying)
            {
                return;
            }
            dirtParticles.Play();
        }

        private void StopCleaning()
        {
            if (dirtParticles == null)
            {
                return;
            }

            if (!dirtParticles.isPlaying)
            {
                return;
            }
                
            dirtParticles.Stop();
        }
    }
}