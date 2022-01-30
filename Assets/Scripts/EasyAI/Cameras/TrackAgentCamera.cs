using EasyAI.Agents;
using EasyAI.Managers;
using UnityEngine;

namespace EasyAI.Cameras
{
    /// <summary>
    /// Camera for tracking above an agent.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class TrackAgentCamera : MonoBehaviour
    {
        [Min(0)]
        [Tooltip("How fast the camera should move to the agent for smooth movement. Set to zero for instant camera movement.")]
        public float moveSpeed = 5;

        [Min(0)]
        [Tooltip("How high from the agent should the camera be.")]
        public float height = 10;
        
        private void Start()
        {
            // Snap look right away.
            float move = moveSpeed;
            moveSpeed = 0;
            LateUpdate();
            moveSpeed = move;
        }
        
        private void LateUpdate()
        {
            // Get the agent to look towards.
            Agent agent = AgentManager.Singleton.SelectedAgent;
            if (agent == null)
            {
                if (AgentManager.Singleton.Agents.Count > 0)
                {
                    agent = AgentManager.Singleton.Agents[0];
                }
                else
                {
                    return;
                }
            }

            // Move over the agent.
            Vector3 target = (agent.Visuals == null ? agent.transform : agent.Visuals).position;
            target = new Vector3(target.x, target.y + height, target.z);
            transform.position = moveSpeed <= 0 ? target : Vector3.Slerp(transform.position, target, moveSpeed * Time.deltaTime);
        }
    }
}