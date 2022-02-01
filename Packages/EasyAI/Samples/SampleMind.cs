namespace Samples
{
    public class SampleMind : Mind
    {
        /// <summary>
        /// Implement to decide what actions the agent's actuators will perform based on the percepts it sensed.
        /// </summary>
        /// <param name="percepts">The percepts which the agent's sensors sensed.</param>
        /// <returns>The actions the agent's actuators will perform.</returns>
        public override Action[] Think(Percept[] percepts)
        {
            foreach (Percept percept in percepts)
            {
                if (!(percept is SamplePercept samplePercept))
                {
                    continue;
                }

                MoveToLookAtTarget(samplePercept.Position);
                return null;
            }

            return null;
        }
    }
}