using System.Collections.Generic;
using System.Linq;
using A1.Managers;
using A1.Percepts;
using EasyAI.Percepts;
using EasyAI.Sensors;
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

            bool isDirty = floors.OrderBy(f => Vector3.Distance(Position, f.transform.position)).First().State != Floor.DirtLevel.Clean;
            AddMessage(isDirty ? "Current floor tile is dirty." : "Current floor tile is not dirty.");
            return new CurrentFloorDirtyPercept
            {
                IsDirty = isDirty
            };
        }
    }
}