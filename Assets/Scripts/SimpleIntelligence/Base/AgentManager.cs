using System;
using System.Collections;
using System.Linq;
using SimpleIntelligence.Agents;
using UnityEngine;

namespace SimpleIntelligence.Base
{
    public class AgentManager : MonoBehaviour
    {
        public enum MessagingMode : byte
        {
            All,
            Compact,
            Unique
        }
        
        public static AgentManager Singleton;

        [SerializeField]
        [Min(0)]
        [Tooltip("The maximum number of agents which can be updated in a single frame. Set to zero to be unlimited.")]
        private int maxAgentsPerUpdate;

        [SerializeField]
        [Tooltip("The maximum number of messages any component can hold.")]
        private int maxMessages = 100;

        public int MaxMessages => maxMessages;

        public bool Playing => !_stepping && Time.timeScale > 0;

        public MessagingMode MessageMode { get; private set; }

        private int _currentAgent;

        protected Agent[] Agents = Array.Empty<Agent>();

        protected Camera[] Cameras = Array.Empty<Camera>();

        private bool _stepping;

        private bool _doneStepping;

        public void FindAgents()
        {
            Agents = FindObjectsOfType<Agent>().OrderBy(a => a.name).ToArray();
            _currentAgent = 0;
        }

        public void FindCameras()
        {
            Cameras = FindObjectsOfType<Camera>().OrderBy(c => c.name).ToArray();
        }

        public void ChangeMessageMode()
        {
            if (MessageMode == MessagingMode.Unique)
            {
                MessageMode = MessagingMode.All;
            }
            else
            {
                MessageMode++;
            }

            if (MessageMode == MessagingMode.Unique)
            {
                ClearMessages();
            }
        }

        public static void ClearMessages()
        {
            foreach (IntelligenceComponent component in FindObjectsOfType<IntelligenceComponent>())
            {
                component.ClearMessages();
            }
        }

        public void Resume()
        {
            Time.timeScale = 1;
        }

        public void Pause()
        {
            Time.timeScale = 0;
        }

        public void Step()
        {
            StartCoroutine(StepOneFrame());
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
        
        private IEnumerator StepOneFrame()
        {
            _stepping = true;
            Resume();
            yield return 0;
            Pause();
            _stepping = false;
        }
    }
}
