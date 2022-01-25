using UnityEngine;

namespace ArtificialIntelligence
{
    public abstract class TransformAgent : MovementAgent
    {
        protected override void Move()
        {
            Vector3 movement = Vector3.MoveTowards(transform.position, Destination, moveSpeed * Time.deltaTime);
            Debug.Log(movement);
            transform.position = movement;
        }

        protected override void Update()
        {
            base.Update();
            Move();
        }
    }
}