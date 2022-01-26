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
    }
}