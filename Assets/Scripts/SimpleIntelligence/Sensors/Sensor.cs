using SimpleIntelligence.Base;
using SimpleIntelligence.Percepts;

namespace SimpleIntelligence.Sensors
{
    public abstract class Sensor : IntelligenceComponent
    {
        public Percept Read()
        {
            if (agent == null || ElapsedTime < time)
            {
                return null;
            }

            Percept percept = Sense();
            ElapsedTime = 0;
            return percept;
        }
        
        protected abstract Percept Sense();
    }
}