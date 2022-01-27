using System.Collections.Generic;
using UnityEngine;

namespace SimpleIntelligence.Base
{
    public abstract class TimedComponent : MonoBehaviour
    {
        public bool HasMessages => messages.Count > 0;
        
        protected float ElapsedTime;

        private readonly List<string> messages = new List<string>();

        public List<string> GetMessages()
        {
            return messages;
        }

        public void AddMessage(string message)
        {
            if (messages.Count > 0 && messages[0] == message)
            {
                return;
            }
            
            messages.Insert(0, message);
            if (messages.Count > AgentManager.Singleton.MaxMessages)
            {
                messages.RemoveAt(messages.Count - 1);
            }
        }

        protected virtual void Update()
        {
            ElapsedTime += Time.deltaTime;
        }
    }
}