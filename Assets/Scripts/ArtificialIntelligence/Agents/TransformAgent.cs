using UnityEngine;

namespace ArtificialIntelligence.Agents
{
    public class TransformAgent : Agent
    {
        protected override void Update()
        {
            base.Update();
            Move();
        }
        
        protected override void Move()
        {
            if (!MovingToTarget)
            {
                return;
            }
            
            Vector3 movement = Vector3.MoveTowards(transform.position, MoveTarget, moveSpeed * Time.deltaTime);
            transform.position = movement;
        }
    }
}