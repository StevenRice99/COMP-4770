using UnityEngine;

namespace ArtificialIntelligence
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class RigidbodyAgent : MovementAgent
    {
        private Rigidbody _rigidbody;

        protected virtual void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        protected override void Move()
        {
            Vector3 position = transform.position;
            _rigidbody.AddForce(Vector3.MoveTowards(position, Destination, moveSpeed * Time.fixedDeltaTime) - position);
        }

        protected virtual void FixedUpdate()
        {
            Move();
        }
    }
}