using System.Collections.Generic;
using System.Linq;
using ArtificialIntelligence;
using ArtificialIntelligence.Agents;
using UnityEngine;

namespace A1
{
    public class CleanerMind : Mind
    {
        public override Action[] Think(Percept[] percepts)
        {
            List<Action> actions = new List<Action>();

            if (CanClean(percepts))
            {
                actions.Add(new CleanAction());
                Agent.StopMoveToLookAtTarget();
            }
            else
            {
                Agent.MoveToLookAtTarget(DetermineNextToClean(percepts));
            }
            
            return actions.ToArray();
        }

        private static bool CanClean(IEnumerable<Percept> percepts)
        {
            return percepts.OfType<IsDirtyPercept>().ToArray().Any(isDirtyPercept => isDirtyPercept.IsDirty);
        }

        private Vector3 DetermineNextToClean(IEnumerable<Percept> percepts)
        {
            DirtPercept[] dirtPercepts = percepts.OfType<DirtPercept>().ToArray();
            if (dirtPercepts.Length == 0)
            {
                return Vector3.zero;
            }

            List<Vector3> positions = new List<Vector3>();
            foreach (DirtPercept dirtPercept in dirtPercepts)
            {
                positions.AddRange(dirtPercept.Positions);
            }

            if (positions.Count == 0)
            {
                return Vector3.zero;
            }

            return positions.OrderBy(p => Vector3.Distance(transform.position, p)).First();
        }
    }
}