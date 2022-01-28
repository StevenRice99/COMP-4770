using System.Linq;
using EasyAI.Percepts;
using UnityEngine;

namespace A1.Percepts
{
    public class FloorsPercept : Percept
    {
        public Vector3[] Positions;

        public Floor.DirtLevel[] States;

        public bool[] LikelyToGetDirty;

        public override string DetailsDisplay()
        {
            int dirtyCount = States.Count(state => state >= Floor.DirtLevel.Dirty);
            int likelyCount = LikelyToGetDirty.Count(likely => likely);
            return $"{Positions.Length} floor tiles, {dirtyCount} dirty, {likelyCount} likely to get dirty.";
        }
    }
}