using System.Collections.Generic;
using System.Linq;
using A1.Managers;
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
            List<Floor> floors = FloorManager.FloorManagerSingleton.Floors;
            if (floors.Count == 0)
            {
                AddMessage("No floors.");
                return null;
            }

            bool isDirty = floors.OrderBy(f => Vector3.Distance(agent.transform.position, f.transform.position)).First().State != Floor.DirtLevel.Clean;
            AddMessage(isDirty ? "Current floor tile is dirty." : "Current floor tile is not dirty.");
            return new CurrentFloorDirtyPercept
            {
                IsDirty = isDirty
            };
        }
    }
}