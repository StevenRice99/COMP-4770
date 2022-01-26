using SimpleIntelligence.Agents;
using UnityEngine;

namespace SimpleIntelligence
{
    public abstract class AIComponent : TimedComponent
    {
        [HideInInspector]
        public Agent Agent;
        
        [SerializeField]
        [Min(0)]
        protected float time;
    }
}