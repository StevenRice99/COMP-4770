﻿using SimpleIntelligence.Actions;
using SimpleIntelligence.Base;
using SimpleIntelligence.Percepts;

namespace SimpleIntelligence.Minds
{
    public abstract class Mind : IntelligenceComponent
    {
        public abstract Action[] Think(Percept[] percepts);
    }
}