using UnityEngine;

namespace EasyAI.Agents
{
    public class TransformAgent : Agent
    {
        protected override void Update()
        {
            base.Update();
            Move();
            if (DidMove && Mind != null)
            {
                Mind.AddMessage($"Moved towards {MoveTarget}.");
            }
        }
        
        protected override void Move()
        {
            if (!MovingToTarget)
            {
                DidMove = false;
                return;
            }

            Vector3 position = transform.position;
            Vector3 lastPosition = position;
            position = Vector3.MoveTowards(position, MoveTarget, moveSpeed * Time.deltaTime);
            DidMove = position != lastPosition;
            transform.position = position;
        }
    }
}