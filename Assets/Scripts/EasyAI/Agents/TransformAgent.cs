using UnityEngine;

namespace EasyAI.Agents
{
    /// <summary>
    /// Agent which moves directly through its transform.
    /// </summary>
    public class TransformAgent : Agent
    {
        /// <summary>
        /// Transform movement.
        /// </summary>
        public override void Move()
        {
            // If the agent should not be moving simply return.
            if (!MovingToTarget)
            {
                DidMove = false;
                return;
            }

            // Move towards the target position.
            Vector3 position = transform.position;
            Vector3 lastPosition = position;
            position = Vector3.MoveTowards(position, MoveTarget, moveSpeed * Time.deltaTime);
            DidMove = position != lastPosition;
            transform.position = position;
            
            if (DidMove && Mind != null)
            {
                Mind.AddMessage($"Moved towards {MoveTarget}.");
            }
        }
    }
}