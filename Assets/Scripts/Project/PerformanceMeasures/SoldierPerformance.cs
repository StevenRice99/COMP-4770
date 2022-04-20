using Project.Agents;
using Project.Managers;

namespace Project.PerformanceMeasures
{
    public class SoldierPerformance : PerformanceMeasure
    {
        private SoldierAgent _soldier;
        
        protected override void Start()
        {
            base.Start();

            _soldier = Agent as SoldierAgent;
        }
        
        protected override float CalculatePerformance()
        {
            return SoldierAgentManager.SoldierAgentManagerSingleton.Sorted.IndexOf(_soldier) + 1;
        }
    }
}