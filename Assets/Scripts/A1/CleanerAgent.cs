using System;
using System.Linq;
using ArtificialIntelligence;
using UnityEngine;

namespace A1
{
    public class CleanerAgent : TransformAgent
    {
        private Floor[] _dirty;
        
        protected override bool Sense()
        {
            Floor[] dirty = FloorManager.Singleton.Floors.Where(f => f.IsDirty).ToArray();
            bool equal = dirty.Length != _dirty.Length || dirty.Count(d => !_dirty.Contains(d)) == 0;
            if (equal)
            {
                return false;
            }

            _dirty = dirty;
            return true;
        }

        protected override bool Think()
        {
            _dirty = _dirty.OrderBy(d => Vector3.Distance(transform.position, d.transform.position)).ToArray();
            SetDestinationAndTarget(!_dirty.Any() ? Vector3.zero : _dirty[0].transform.position);
            return true;
        }

        protected override void Act()
        {
            throw new System.NotImplementedException();
        }
    }
}