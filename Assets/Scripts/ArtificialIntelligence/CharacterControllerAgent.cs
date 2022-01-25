using UnityEngine;

namespace ArtificialIntelligence
{
    [RequireComponent(typeof(CharacterController))]
    public abstract class CharacterControllerAgent : MovementAgent
    {
        private CharacterController _characterController;

        protected virtual void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }
        
        protected override void Move()
        {
            Vector3 position = transform.position;
            _characterController.Move(Vector3.MoveTowards(position, Destination, moveSpeed * Time.deltaTime) - position);
        }
    }
}