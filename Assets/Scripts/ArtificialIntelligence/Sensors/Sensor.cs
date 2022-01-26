using ArtificialIntelligence.Percepts;
using UnityEngine;

namespace ArtificialIntelligence.Sensors
{
    public abstract class Sensor : AIComponent
    {
        public Percept Read()
        {
            ElapsedTime += Agent.ElapsedTime;

            if (ElapsedTime < timeRequired)
            {
                return null;
            }

            ElapsedTime = 0;
            return Sense();
        }
        
        protected abstract Percept Sense();
    }
}