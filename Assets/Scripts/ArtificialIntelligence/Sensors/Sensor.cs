﻿using ArtificialIntelligence.Percepts;

namespace ArtificialIntelligence.Sensors
{
    public abstract class Sensor : AIComponent
    {
        public Percept Read()
        {
            ElapsedTime += Agent.AgentElapsedTime;

            if (ElapsedTime < time)
            {
                return null;
            }

            ElapsedTime = 0;
            return Sense();
        }
        
        protected abstract Percept Sense();
    }
}