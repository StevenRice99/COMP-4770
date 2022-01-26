using ArtificialIntelligence.Agents;
using UnityEngine;

namespace ArtificialIntelligence
{
    public abstract class AIComponent : MonoBehaviour
    {
        [HideInInspector]
        public Agent Agent;
        
        [SerializeField]
        [Min(0)]
        protected float time;

        protected float ElapsedTime;
    }
}