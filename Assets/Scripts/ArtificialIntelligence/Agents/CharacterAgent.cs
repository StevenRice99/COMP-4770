using UnityEngine;

namespace ArtificialIntelligence.Agents
{
    public abstract class CharacterAgent : TransformAgent
    {
        private CharacterController _characterController;

        protected override void Start()
        {
            base.Start();
            _characterController = GetComponent<CharacterController>();
        }
        
        protected override void Move()
        {
            Vector3 position = transform.position;
            _characterController.Move(Vector3.MoveTowards(position, destination, moveSpeed * Time.deltaTime) - position);
        }
    }
}