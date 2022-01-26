using UnityEngine;

namespace ArtificialIntelligence.Agents
{
    public class RigidbodyAgent : Agent
    {
        private Rigidbody _rigidbody;

        protected override void Start()
        {
            base.Update();
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        protected virtual void FixedUpdate()
        {
            Move();
        }
        
        protected override void Move()
        {
            if (!MovingToTarget)
            {
                _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
                return;
            }
            
            Vector3 position = transform.position;
            _rigidbody.AddForce(Vector3.MoveTowards(position, MoveTarget, moveSpeed * Time.fixedDeltaTime) - position);
        }
    }
}