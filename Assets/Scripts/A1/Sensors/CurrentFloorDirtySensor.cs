using System.Collections.Generic;
using System.Linq;
using A1.Percepts;
using ArtificialIntelligence.Percepts;
using ArtificialIntelligence.Sensors;
using UnityEngine;

namespace A1.Sensors
{
    public class CurrentFloorDirtySensor : Sensor
    {
        protected override Percept Sense()
        {
            CurrentFloorDirtyPercept percept = new CurrentFloorDirtyPercept();
            
            List<Floor> floors = FloorManager.Singleton.Floors;
            if (floors.Count == 0)
            {
                return percept;
            }

            percept.IsDirty = floors.OrderBy(f => Vector3.Distance(Agent.transform.position, f.transform.position)).First().State != Floor.DirtLevel.Clean;
            return percept;
        }
    }
}