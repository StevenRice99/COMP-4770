using ArtificialIntelligence.Agents;
using UnityEngine;

namespace ArtificialIntelligence
{
    public abstract class AIComponent : MonoBehaviour
    {
        [HideInInspector]
        public Agent Agent;
    }
}