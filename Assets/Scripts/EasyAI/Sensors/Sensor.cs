using EasyAI.Components;
using EasyAI.Percepts;

namespace EasyAI.Sensors
{
    public abstract class Sensor : IntelligenceComponent
    {
        public Percept Read()
        {
            Percept percept = Sense();
            AddMessage(percept == null ? "Did not perceive anything." : $"Perceived {percept}.");
            return percept;
        }
        
        protected abstract Percept Sense();
    }
}