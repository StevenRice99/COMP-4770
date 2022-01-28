using System.Collections.Generic;
using System.Linq;
using EasyAI.Agents;
using EasyAI.Managers;
using UnityEngine;

namespace EasyAI.Components
{
    public abstract class IntelligenceComponent : MonoBehaviour
    {
        [HideInInspector]
        public Agent agent;
        
        public bool HasMessages => Messages.Count > 0;

        public int MessageCount => Messages.Count;
        
        public List<string> Messages { get; private set; } = new List<string>();

        public virtual float DisplayDetails(float x, float y, float w, float h, float p)
        {
            return y;
        }
        
        public virtual void DisplayGizmos() { }

        public void AddMessage(string message)
        {
            AgentManager.Singleton.AddGlobalMessage($"{name} - {message}");
            
            switch (AgentManager.Singleton.MessageMode)
            {
                case AgentManager.MessagingMode.Compact when Messages.Count > 0 && Messages[0] == message:
                    return;
                case AgentManager.MessagingMode.Unique:
                    Messages = Messages.Where(m => m != message).ToList();
                    break;
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