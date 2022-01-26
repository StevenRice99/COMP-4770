using SimpleIntelligence.Base;

namespace SimpleIntelligence.PerformanceMeasures
{
    public abstract class PerformanceMeasure : IntelligenceComponent
    {
        private float performance;
        
        public float GetPerformance()
        {
            if (agent != null && ElapsedTime >= time)
            {
                performance = CalculatePerformance();
                ElapsedTime = 0;
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