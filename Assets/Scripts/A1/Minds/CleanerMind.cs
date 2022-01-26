﻿using System.Collections.Generic;
using System.Linq;
using A1.Actions;
using A1.Percepts;
using ArtificialIntelligence.Actions;
using ArtificialIntelligence.Minds;
using ArtificialIntelligence.Percepts;
using UnityEngine;

namespace A1.Minds
{
    public class CleanerMind : Mind
    {
        public override Action[] Think(Percept[] percepts)
        {
            if (CanClean(percepts))
            {
                Agent.StopMoveToLookAtTarget();
                return new Action[] { new CleanAction() };
            }

            Agent.MoveToLookAtTarget(DetermineNextToClean(percepts));
            return null;
        }

        private static bool CanClean(IEnumerable<Percept> percepts)
        {
            return percepts.OfType<CurrentFloorDirtyPercept>().ToArray().Any(isDirtyPercept => isDirtyPercept.IsDirty);
        }

        private Vector3 DetermineNextToClean(IEnumerable<Percept> percepts)
        {
            FloorsPercept[] dirtPercepts = percepts.OfType<FloorsPercept>().ToArray();
            if (dirtPercepts.Length == 0)
            {
                return Vector3.zero;
            }

            List<Vector3> dirty = new List<Vector3>();
            List<Vector3> veryDirty = new List<Vector3>();
            List<Vector3> extremelyDirty = new List<Vector3>();
            List<Vector3> likelyToGetDirty = new List<Vector3>();

            foreach (FloorsPercept dirtPercept in dirtPercepts)
            {
                for (int i = 0; i < dirtPercept.Positions.Length; i++)
                {
                    if (dirtPercept.LikelyToGetDirty[i])
                    {
                        likelyToGetDirty.Add(dirtPercept.Positions[i]);
                    }
                    
                    switch (dirtPercept.States[i])
                    {
                        case Floor.DirtLevel.Dirty:
                            dirty.Add(dirtPercept.Positions[i]);
                            break;
                        case Floor.DirtLevel.VeryDirty:
                            veryDirty.Add(dirtPercept.Positions[i]);
                            break;
                        case Floor.DirtLevel.ExtremelyDirty:
                            extremelyDirty.Add(dirtPercept.Positions[i]);
                            break;
                    }
                }
            }

            return extremelyDirty.Count > 0
                ? NearestPosition(extremelyDirty)
                : veryDirty.Count > 0
                    ? NearestPosition(veryDirty)
                    : dirty.Count > 0
                        ? NearestPosition(dirty)
                        : likelyToGetDirty.Count > 0
                            ? likelyToGetDirty.Aggregate(Vector3.zero, (current, position) => current + position) / likelyToGetDirty.Count
                            : Vector3.zero;
        }

        private Vector3 NearestPosition(IReadOnlyCollection<Vector3> positions)
        {
            return positions.Count == 0 ? Vector3.zero : positions.OrderBy(p => Vector3.Distance(Agent.transform.position, p)).First();
        }
    }
}