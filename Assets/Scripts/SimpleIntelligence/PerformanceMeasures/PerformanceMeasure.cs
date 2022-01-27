using SimpleIntelligence.Components;

namespace SimpleIntelligence.PerformanceMeasures
{
    public abstract class PerformanceMeasure : IntelligenceComponent
    {
        protected float TimeSinceLastCalculation => agent == null ? 0 : agent.AgentDeltaTime;
        
        private float performance;
        
        public float GetPerformance()
        {
            if (agent != null)
            {
                performance = CalculatePerformance();
            }

            return performance;
        }
        
        protected abstract float CalculatePerformance();

        protected virtual void Start()
        {
            CalculatePerformance();
        }
    }
}