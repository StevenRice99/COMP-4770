using UnityEngine;

namespace ArtificialIntelligence
{
    public abstract class Sensor : AIComponent
    {
        [SerializeField]
        [Min(1)]
        private int tickSteps = 1;

        private int _step;

        public Percept Read()
        {
            _step++;
            if (_step < tickSteps)
            {
                return null;
            }

            _step = 0;
            return Sense();
        }
        
        protected abstract Percept Sense();
    }
}