using UnityEngine;

namespace ArtificialIntelligence.Agents
{
    public abstract class RigidbodyAgent : Agent
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
            Vector3 position = transform.position;
            _rigidbody.AddForce(Vector3.MoveTowards(position, destination, moveSpeed * Time.fixedDeltaTime) - position);
        }
    }
}