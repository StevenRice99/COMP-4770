using UnityEngine;

namespace EasyAI.Agents
{
    /// <summary>
    /// Agent which moves directly through its transform.
    /// </summary>
    public class TransformAgent : Agent
    {
        protected override void Update()
        {
            base.Update();
            
            // Add in movement every frame.
            Move();
            if (DidMove && Mind != null)
            {
                Mind.AddMessage($"Moved towards {MoveTarget}.");
            }
        }
        
        /// <summary>
        /// Transform movement.
        /// </summary>
        protected override void Move()
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
        }
    }
}