using ArtificialIntelligence.Percepts;
using UnityEngine;

namespace ArtificialIntelligence.Sensors
{
    public abstract class Sensor : AIComponent
    {
        [SerializeField]
        [Min(0)]
        private float timeBetweenReads;

        private float elapsedTime;
        
        public Percept Read()
        {
            elapsedTime += Agent.ElapsedTime;

            if (elapsedTime < timeBetweenReads)
            {
                return null;
            }

            elapsedTime = 0;
            return Sense();
        }
        
        protected abstract Percept Sense();
    }
}