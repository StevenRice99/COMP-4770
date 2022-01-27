using System.Linq;
using SimpleIntelligence.Components;
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
            AddMessage(percept == null ? "Did not perceive anything." : $"Perceived {percept.GetType().ToString().Split('.').Last()}.");
            return percept;
        }
        
        protected abstract Percept Sense();
    }
}