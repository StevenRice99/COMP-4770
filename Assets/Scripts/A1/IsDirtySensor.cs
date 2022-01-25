using System.Collections.Generic;
using System.Linq;
using ArtificialIntelligence;
using UnityEngine;

namespace A1
{
    public class IsDirtySensor : Sensor
    {
        protected override Percept Sense()
        {
            IsDirtyPercept percept = new IsDirtyPercept();
            
            List<Floor> floors = FloorManager.Singleton.Floors;
            if (floors.Count == 0)
            {
                return percept;
            }

            percept.IsDirty = floors.OrderBy(f => Vector3.Distance(Agent.transform.position, f.transform.position)).First().IsDirty;
            return percept;
        }
    }
}