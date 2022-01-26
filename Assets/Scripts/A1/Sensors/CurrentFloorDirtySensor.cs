using System.Collections.Generic;
using System.Linq;
using A1.Percepts;
using SimpleIntelligence.Percepts;
using SimpleIntelligence.Sensors;
using UnityEngine;

namespace A1.Sensors
{
    public class CurrentFloorDirtySensor : Sensor
    {
        protected override Percept Sense()
        {
            List<Floor> floors = FloorManager.Singleton.Floors;
            if (floors.Count == 0)
            {
                return null;
            }

            return new CurrentFloorDirtyPercept
            {
                IsDirty = floors.OrderBy(f => Vector3.Distance(agent.transform.position, f.transform.position)).First().State != Floor.DirtLevel.Clean
            };
        }
    }
}