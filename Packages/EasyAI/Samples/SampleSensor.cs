using System.Collections;
using UnityEngine;

namespace Samples
{
    public class SampleSensor : Sensor
    {
        public Vector3 target;

        public Vector3 origin;

        public float size;

        private bool change;
        
        /// <summary>
        /// Implement what the sensor will send back to the agent.
        /// </summary>
        /// <returns>The percept sent back to the agent.</returns>
        protected override Percept Sense()
        {
            if (!change && Vector2.Distance(new Vector2(Position.x, Position.z), new Vector2(target.x, target.z)) > 1)
            {
                return new SamplePercept {Position = target};
            }

            target = new Vector3(Random.Range(origin.x - size, origin.x + size), 0, Random.Range(origin.z - size, origin.z + size));
            change = false;
            StopAllCoroutines();
            StartCoroutine(WaitForSeconds());

            return new SamplePercept { Position = target };
        }

        private IEnumerator WaitForSeconds()
        {
            yield return new WaitForSeconds(5);
            change = true;
        }
    }
}