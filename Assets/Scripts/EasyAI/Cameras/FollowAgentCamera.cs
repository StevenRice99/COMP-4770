using EasyAI.Agents;
using EasyAI.Managers;
using UnityEngine;

namespace EasyAI.Cameras
{
    /// <summary>
    /// Camera for following behind an agent.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class FollowAgentCamera : MonoBehaviour
    {
        [Tooltip("How much to vertically offset the camera for viewing agents.")]
        public float offset = 1;
        
        [SerializeField]
        [Min(0)]
        [Tooltip("How fast the camera should look to the agent for smooth looking. Set to zero for instant camera looking.")]
        private float lookSpeed;
        
        [Min(0)]
        [Tooltip("How fast the camera should move to the agent for smooth movement. Set to zero for instant camera movement.")]
        public float moveSpeed = 5;

        [Min(0)]
        [Tooltip("How far away from the agent should the camera be.")]
        public float distance = 5;

        [Min(0)]
        [Tooltip("How high from the agent should the camera be.")]
        public float height = 5;

        private void Start()
        {
            // Snap look right away.
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
            // Get the agent to look towards.
            Agent agent = AgentManager.Singleton.SelectedAgent;
            if (agent == null && AgentManager.Singleton.Agents.Count > 0)
            {
                agent = AgentManager.Singleton.Agents[0];
            }
            else
            {
                return;
            }

            // Determine where to move and look to.
            Transform agentTransform = agent.Visuals == null ? agent.transform : agent.Visuals;
            Vector3 target = agentTransform.position;
            target = new Vector3(target.x, target.y + offset, target.z);
            Vector3 move = target + agentTransform.rotation * new Vector3(0, height, -distance);

            Transform t = transform;
            Vector3 position = t.position;

            // Move to the location.
            transform.position = moveSpeed <= 0 ? move : Vector3.Slerp(position, move, moveSpeed * Time.deltaTime);

            // Look at the agent.
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