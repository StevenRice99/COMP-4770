using System.Collections.Generic;
using SimpleIntelligence.Agents;
using UnityEngine;

namespace SimpleIntelligence.Base
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
        
        public List<string> Messages { get; } = new List<string>();

        public void AddMessage(string message)
        {
            if (AgentManager.Singleton.CompactMessages && Messages.Count > 0 && Messages[0] == message)
            {
                return;
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