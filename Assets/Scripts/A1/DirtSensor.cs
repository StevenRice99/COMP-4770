using System.Collections.Generic;
using System.Linq;
using ArtificialIntelligence;

namespace A1
{
    public class DirtSensor : Sensor
    {
        protected override Percept Sense()
        {
            DirtPercept percept = new DirtPercept();
            
            List<Floor> floors = FloorManager.Singleton.Floors;
            if (floors.Count == 0)
            {
                return percept;
            }
            
            percept.Positions = floors.Where(f => f.IsDirty).Select(f => f.transform.position).ToArray();
            
            return percept;
        }
    }
}