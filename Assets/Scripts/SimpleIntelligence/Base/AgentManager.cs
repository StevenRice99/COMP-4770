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

        private int _currentAgent;

        private Agent[] _agents;
    
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
            _agents = FindObjectsOfType<Agent>();
            _currentAgent = 0;
        }

        protected virtual void Start()
        {
            FindAgents();
        }

        protected virtual void Update()
        {
            if (maxAgentsPerUpdate <= 0)
            {
                foreach (Agent agent in _agents)
                {
                    agent.Perform();
                }
            
                return;
            }
        
            for (int i = 0; i < maxAgentsPerUpdate && i < _agents.Length; i++)
            {
                _agents[_currentAgent].Perform();
                NextAgent();
            }
        }

        private void NextAgent()
        {
            _currentAgent++;
            if (_currentAgent >= _agents.Length)
            {
                _currentAgent = 0;
            }
        }
    }
}
