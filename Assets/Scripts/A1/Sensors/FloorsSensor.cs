using System.Collections.Generic;
using A1.Managers;
using A1.Percepts;
using SimpleIntelligence.Percepts;
using SimpleIntelligence.Sensors;
using UnityEngine;

namespace A1.Sensors
{
    public class FloorsSensor : Sensor
    {
        protected override Percept Sense()
        {
            List<Floor> floors = FloorManager.FloorManagerSingleton.Floors;
            
            FloorsPercept percept = new FloorsPercept
            {
                Positions = new Vector3[floors.Count],
                States = new Floor.DirtLevel[floors.Count],
                LikelyToGetDirty = new bool[floors.Count]
            };
            
            for (int i = 0; i < floors.Count; i++)
            {
                percept.Positions[i] = floors[i].transform.position;
                percept.States[i] = floors[i].State;
                percept.LikelyToGetDirty[i] = floors[i].LikelyToGetDirty;
            }
            
            AddMessage($"Detected {floors.Count} floor tiles.");
            
            return percept;
        }
    }
}