using System.Collections.Generic;
using A1.Managers;
using A1.Percepts;
using UnityEngine;

namespace A1.Sensors
{
    /// <summary>
    /// Sense positions, dirt levels, and if they are likely to get dirty for all floor tiles in the scene.
    /// </summary>
    public class FloorsSensor : Sensor
    {
        /// <summary>
        /// Sense positions, dirt levels, and if they are likely to get dirty for all floor tiles in the scene.
        /// </summary>
        /// <returns>A FloorsPercept with positions, dirt levels, and if they are likely to get dirty for all floor tiles in the scene.</returns>
        protected override Percept Sense()
        {
            // Get all floors.
            List<Floor> floors = FloorManager.FloorManagerSingleton.Floors;
            
            // Build the percept.
            FloorsPercept percept = new FloorsPercept
            {
                Positions = new Vector3[floors.Count],
                States = new Floor.DirtLevel[floors.Count],
                LikelyToGetDirty = new bool[floors.Count]
            };
            
            // Fill the percept with data.
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