using EasyAI.Percepts;
using UnityEngine;

namespace A1.Percepts
{
    public class FloorsPercept : Percept
    {
        public Vector3[] Positions;

        public Floor.DirtLevel[] States;

        public bool[] LikelyToGetDirty;
    }
}