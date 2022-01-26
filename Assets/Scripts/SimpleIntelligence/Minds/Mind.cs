using SimpleIntelligence.Actions;
using SimpleIntelligence.Percepts;

namespace SimpleIntelligence.Minds
{
    public abstract class Mind : AIComponent
    {
        public abstract Action[] Think(Percept[] percepts);
    }
}