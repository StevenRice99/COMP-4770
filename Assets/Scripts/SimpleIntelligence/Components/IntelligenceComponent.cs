using System.Collections.Generic;
using System.Linq;
using SimpleIntelligence.Agents;
using SimpleIntelligence.Managers;
using UnityEngine;

namespace SimpleIntelligence.Components
{
    public abstract class IntelligenceComponent : TimedComponent
    {
        [HideInInspector]
        public Agent agent;
        
        [SerializeField]
        [Min(0)]
        [Tooltip(
            "How much time is required for this component to perform its actions again, " +
            "whether that is reading percepts as a sensor, performing actions as an actuator, " +
            "or calculating performance as a performance measure."
        )]
        protected float time;
        
        public bool HasMessages => Messages.Count > 0;

        public int MessageCount => Messages.Count;
        
        public List<string> Messages { get; private set; } = new List<string>();

        public void AddMessage(string message)
        {
            if (AgentManager.Singleton.MessageMode == AgentManager.MessagingMode.Compact && Messages.Count > 0 && Messages[0] == message)
            {
                return;
            }

            if (AgentManager.Singleton.MessageMode == AgentManager.MessagingMode.Unique)
            {
                Messages = Messages.Where(m => m != message).ToList();
            }
            
            Messages.Insert(0, message);
            if (Messages.Count > AgentManager.Singleton.MaxMessages)
            {
                Messages.RemoveAt(Messages.Count - 1);
            }
        }

        public void ClearMessages()
        {
            Messages.Clear();
        }
    }
}