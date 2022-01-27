using System;
using SimpleIntelligence.Agents;
using UnityEngine;

namespace SimpleIntelligence.Base
{
    public class AgentManager : MonoBehaviour
    {
        public static AgentManager Singleton;

        [SerializeField]
        [Min(0)]
        [Tooltip("The maximum number of agents which can be updated in a single frame. Set to zero to be unlimited.")]
        private int maxAgentsPerUpdate;

        [SerializeField]
        [Min(1)]
        [Tooltip("How many messages each component can store.")]
        private int maxMessages = 10;

        public int MaxMessages => maxMessages;

        private int _currentAgent;

        protected Agent[] Agents = Array.Empty<Agent>();

        protected Camera[] Cameras = Array.Empty<Camera>();
    
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
            DontDestroyOnLoad(gameObject);
        }

        public void FindAgents()
        {
            Agents = FindObjectsOfType<Agent>();
            _currentAgent = 0;
        }

        public void FindCameras()
        {
            Cameras = FindObjectsOfType<Camera>();
        }

        protected virtual void Start()
        {
            FindAgents();
            FindCameras();
        }

        protected virtual void Update()
        {
            if (maxAgentsPerUpdate <= 0)
            {
                foreach (Agent agent in Agents)
                {
                    agent.Perform();
                }
            
                return;
            }
        
            for (int i = 0; i < maxAgentsPerUpdate && i < Agents.Length; i++)
            {
                Agents[_currentAgent].Perform();
                NextAgent();
            }
        }

        private void NextAgent()
        {
            _currentAgent++;
            if (_currentAgent >= Agents.Length)
            {
                _currentAgent = 0;
            }
        }
    }
}
