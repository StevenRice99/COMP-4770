using SimpleIntelligence.Percepts;

namespace SimpleIntelligence.Sensors
{
    public abstract class Sensor : AIComponent
    {
        public Percept Read()
        {
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