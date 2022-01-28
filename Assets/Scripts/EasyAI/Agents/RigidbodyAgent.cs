using UnityEngine;

namespace EasyAI.Agents
{
    /// <summary>
    /// Agent which moves through a rigidbody.
    /// </summary>
    public class RigidbodyAgent : Agent
    {
        /// <summary>
        /// This agent's rigidbody.
        /// </summary>
        private Rigidbody _rigidbody;

        protected override void Start()
        {
            base.Update();
            
            // Get the rigidbody.
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            // Since rotation is all done with the root visuals transform, freeze rigidbody rotation.
            if (_rigidbody != null)
            {
                _rigidbody.freezeRotation = true;
            }
        }
        
        protected virtual void FixedUpdate()
        {
            // Move in FixedUpdate as the rigidbody uses physics.
            Move();
            if (DidMove && Mind != null)
            {
                Mind.AddMessage($"Moved towards {MoveTarget}.");
            }
        }
        
        protected override void Move()
        {
            Vector3 lastPosition = transform.position;
            
            if (MovingToTarget)
            {
                Vector3 position = transform.position;
                _rigidbody.AddForce(Vector3.MoveTowards(position, MoveTarget, moveSpeed * Time.fixedDeltaTime) - position);
            }
            
            DidMove = transform.position != lastPosition;
        }
    }
}