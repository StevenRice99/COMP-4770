using SimpleIntelligence.Actions;
using SimpleIntelligence.Components;
using SimpleIntelligence.Percepts;

namespace SimpleIntelligence.Minds
{
    public abstract class Mind : IntelligenceComponent
    {
        public abstract Action[] Think(Percept[] percepts);
    }
}