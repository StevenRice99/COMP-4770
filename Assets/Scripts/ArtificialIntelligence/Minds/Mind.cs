using ArtificialIntelligence.Actions;
using ArtificialIntelligence.Percepts;

namespace ArtificialIntelligence.Minds
{
    public abstract class Mind : AIComponent
    {
        public abstract Action[] Think(Percept[] percepts);
    }
}