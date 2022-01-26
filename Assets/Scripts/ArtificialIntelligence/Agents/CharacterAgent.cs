using UnityEngine;

namespace ArtificialIntelligence.Agents
{
    public class CharacterAgent : TransformAgent
    {
        private CharacterController _characterController;

        protected override void Start()
        {
            base.Start();
            _characterController = GetComponent<CharacterController>();
        }
        
        protected override void Move()
        {
            if (!MovingToTarget)
            {
                _characterController.SimpleMove(Vector3.zero);
                return;
            }
            
            Vector3 position = transform.position;
            _characterController.SimpleMove(Vector3.MoveTowards(position, MoveTarget, moveSpeed * Time.deltaTime) - position);
        }
    }
}