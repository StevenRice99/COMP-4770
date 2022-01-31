using UnityEngine;

namespace EasyAI.Agents
{
    /// <summary>
    /// Agent which moves through a character controller.
    /// </summary>
    public class CharacterAgent : TransformAgent
    {
        /// <summary>
        /// This agent's character controller.
        /// </summary>
        private CharacterController _characterController;

        protected override void Start()
        {
            base.Start();
            
            // Get the character controller.
            _characterController = GetComponent<CharacterController>();
            if (_characterController == null)
            {
                _characterController = gameObject.AddComponent<CharacterController>();
            }
        }
        
        /// <summary>
        /// Character controller movement.
        /// </summary>
        public override void Move()
        {
            // Get the agent's position prior to any movement.
            Vector3 lastPosition = transform.position;
            
            // If the agent should not be moving, still call to move so gravity is applied.
            if (!MovingToTarget)
            {
                MoveVelocity = 0;
                _characterController.SimpleMove(Vector3.zero);
            }
            else
            {
                // Calculate how fast we can move this frame.
                CalculateMoveVelocity();
                
                Vector3 position = transform.position;
                _characterController.SimpleMove(Vector3.MoveTowards(position, MoveTarget, MoveVelocity * Time.deltaTime) - position);
            }

            DidMove = transform.position != lastPosition;
            
            if (DidMove)
            {
                AddMessage($"Moved towards {MoveTarget}.");
            }
        }
    }
}