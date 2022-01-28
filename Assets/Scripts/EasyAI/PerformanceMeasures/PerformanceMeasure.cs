using EasyAI.Components;

namespace EasyAI.PerformanceMeasures
{
    public abstract class PerformanceMeasure : IntelligenceComponent
    {
        private float performance;
        
        public float GetPerformance()
        {
            if (Agent != null)
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