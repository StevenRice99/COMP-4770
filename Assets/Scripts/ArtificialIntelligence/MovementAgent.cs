using UnityEngine;

namespace ArtificialIntelligence
{
    public abstract class MovementAgent : Agent
    {
        [SerializeField]
        protected float moveSpeed;
        
        [SerializeField]
        protected float lookSpeed;

        [SerializeField]
        private Transform visuals;

        protected Vector3 Destination;
        
        private Vector3 _target;

        protected void SetDestinationAndTarget(Vector3 position)
        {
            SetDestination(position);
            SetTarget(position);
        }

        protected void SetDestination(Vector3 destination)
        {
            Destination = destination;
        }

        protected void SetTarget(Vector3 target)
        {
            _target = target;
        }
        
        protected override void Update()
        {
            base.Update();
            Look();
        }

        protected abstract void Move();

        private void Look()
        {
            Transform t = visuals;
            Vector3 target = new Vector3(_target.x, t.position.y, _target.z);
            if (t.position == target)
            {
                return;
            }
            
            visuals.rotation = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(t.position - target), lookSpeed * Time.deltaTime);
        }
    }
}