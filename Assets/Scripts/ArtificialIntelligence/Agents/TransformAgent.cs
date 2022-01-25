using UnityEngine;

namespace ArtificialIntelligence.Agents
{
    public abstract class TransformAgent : Agent
    {
        protected override void Update()
        {
            base.Update();
            Move();
        }
        
        protected override void Move()
        {
            Vector3 movement = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            transform.position = movement;
        }
    }
}