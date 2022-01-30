using System.Collections.Generic;
using System.Linq;
using A1.Managers;
using A1.Percepts;
using EasyAI.Percepts;
using EasyAI.Sensors;
using UnityEngine;

namespace A1.Sensors
{
    public class DirtySensor : Sensor
    {
        protected override Percept Sense()
        {
            List<Floor> floors = FloorManager.FloorManagerSingleton.Floors;
            if (floors.Count == 0)
            {
                AddMessage("No floors.");
                return null;
            }

            DirtyPercept percept = new DirtyPercept
            {
                Floor = floors.OrderBy(f => Vector3.Distance(Position, f.transform.position)).First()
            };
            
            AddMessage(percept.IsDirty ? "Current floor tile is dirty." : "Current floor tile is not dirty.");
            return percept;
        }
    }
}