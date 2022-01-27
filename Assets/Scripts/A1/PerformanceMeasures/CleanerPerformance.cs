using System.Collections.Generic;
using System.Linq;
using A1.Managers;
using EasyAI.PerformanceMeasures;

namespace A1.PerformanceMeasures
{
    public class CleanerPerformance : PerformanceMeasure
    {
        protected override float CalculatePerformance()
        {
            List<Floor> floors = FloorManager.FloorManagerSingleton.Floors;
            if (floors == null || floors.Count == 0)
            {
                return 100;
            }

            int maxPerformance = floors.Count * 3;
            int performance = maxPerformance;
            performance = floors.Aggregate(performance, (current, floor) => current - (int) floor.State);

            return (float) performance / maxPerformance * 100f;
        }
    }
}