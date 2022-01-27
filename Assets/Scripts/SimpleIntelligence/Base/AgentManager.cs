using System;
using System.Linq;
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
        [Tooltip("The maximum number of messages any component can hold.")]
        private int maxMessages = 100;

        [SerializeField]
        [Tooltip("Setting true will cause duplicate messages to be merged, false will display duplicate messages.")]
        private bool compactMessages = true;

        public int MaxMessages => maxMessages;

        public bool CompactMessages => compactMessages;

        private int _currentAgent;

        protected Agent[] Agents = Array.Empty<Agent>();

        protected Camera[] Cameras = Array.Empty<Camera>();

        public void FindAgents()
        {
            Agents = FindObjectsOfType<Agent>().OrderBy(a => a.name).ToArray();
            _currentAgent = 0;
        }

        public void FindCameras()
        {
            Cameras = FindObjectsOfType<Camera>().OrderBy(c => c.name).ToArray();
        }

        protected static void ClearMessages()
        {
            foreach (IntelligenceComponent component in FindObjectsOfType<IntelligenceComponent>())
            {
                component.ClearMessages();
            }
        }

        protected virtual void Start()
        {
            FindAgents();
            FindCameras();
        }

        protected void MessageMode(bool compact)
        {
            compactMessages = compact;
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
