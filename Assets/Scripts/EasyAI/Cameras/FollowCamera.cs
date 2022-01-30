using EasyAI.Agents;
using EasyAI.Managers;
using UnityEngine;

namespace EasyAI.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class FollowCamera : MonoBehaviour
    {
        /// <summary>
        /// The singleton look at agent camera.
        /// </summary>
        public static FollowCamera Singleton;
        
        [SerializeField]
        [Tooltip("How much to vertically offset the camera for viewing agents.")]
        private float offset;
        
        [SerializeField]
        [Min(0)]
        [Tooltip("How fast the camera should look to the agent for smooth looking. Set to zero for instant camera looking.")]
        private float lookSpeed;
        
        [SerializeField]
        [Min(0)]
        [Tooltip("How fast the camera should move to the agent for smooth movement. Set to zero for instant camera movement.")]
        private float moveSpeed;

        [SerializeField]
        [Min(0)]
        [Tooltip("How far away from the agent should the camera be.")]
        private float distance;

        [SerializeField]
        [Min(0)]
        [Tooltip("How high from the agent should the camera be.")]
        private float height;
        
        private void Awake()
        {
            if (Singleton == this)
            {
                return;
            }

            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }

        private void Start()
        {
            float look = lookSpeed;
            float move = moveSpeed;
            lookSpeed = 0;
            moveSpeed = 0;
            LateUpdate();
            lookSpeed = look;
            moveSpeed = move;
        }
        
        private void LateUpdate()
        {
            Agent agent = AgentManager.Singleton.SelectedAgent;
            if (agent == null)
            {
                agent = FindObjectOfType<Agent>();
            }
            
            if (agent == null)
            {
                return;
            }

            Transform agentTransform = agent.Visuals == null ? agent.transform : agent.Visuals;
            Vector3 target = agentTransform.position;
            target = new Vector3(target.x, target.y + offset, target.z);
            Vector3 move = target + agentTransform.rotation * new Vector3(0, height, -distance);;

            Transform t = transform;
            Vector3 position = t.position;

            transform.position = moveSpeed <= 0 ? move : Vector3.Slerp(position, move, moveSpeed * Time.deltaTime);

            if (lookSpeed <= 0)
            {
                transform.LookAt(target);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(t.rotation, Quaternion.LookRotation(target - position), lookSpeed * Time.deltaTime);
            }
        }
    }
}