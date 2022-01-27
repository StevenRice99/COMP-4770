using UnityEngine;

namespace SimpleIntelligence.Agents
{
    public class RigidbodyAgent : Agent
    {
        private Rigidbody _rigidbody;

        protected override void Start()
        {
            base.Update();
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true;
        }
        
        protected virtual void FixedUpdate()
        {
            Move();
            if (DidMove && Mind != null)
            {
                Mind.AddMessage($"Moved towards {MoveTarget}.");
            }
        }
        
        protected override void Move()
        {
            Vector3 lastPosition = transform.position;
            
            if (!MovingToTarget)
            {
                _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            }
            else
            {
                Vector3 position = transform.position;
                _rigidbody.AddForce(Vector3.MoveTowards(position, MoveTarget, moveSpeed * Time.fixedDeltaTime) - position);
            }
            
            DidMove = transform.position != lastPosition;
        }
    }
}