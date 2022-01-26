using UnityEngine;

namespace ArtificialIntelligence
{
    public abstract class Mind : AIComponent
    {
        public abstract Action[] Think(Percept[] percepts);
    }
}