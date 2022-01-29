using EasyAI.Agents;
using EasyAI.Managers;
using UnityEngine;

namespace EasyAI.Cameras
{
    public class LookAtAgentCamera : MonoBehaviour
    {
        /// <summary>
        /// The singleton agent manager.
        /// </summary>
        public static LookAtAgentCamera Singleton;

        [SerializeField]
        [Tooltip("How much to vertically offset the camera for viewing agents.")]
        private float offset = 0;

        public Transform Target { get; private set; }

        public void SetOffset(float value)
        {
            offset = value;
        }
        
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

        private void LateUpdate()
        {
            if (Target == null)
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

                Target = agent.transform;
            }

            Vector3 position = Target.position;
            transform.LookAt(new Vector3(position.x, position.y + offset, position.z));
        }
    }
}