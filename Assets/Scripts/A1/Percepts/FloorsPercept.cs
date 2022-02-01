﻿using System.Linq;
using EasyAI.Runtime.Percepts;
using UnityEngine;

namespace A1.Percepts
{
    /// <summary>
    /// Hold positions, dirt levels, and if they are likely to get dirty for all floor tiles in the scene.
    /// </summary>
    public class FloorsPercept : Percept
    {
        /// <summary>
        /// Positions of all floor tiles.
        /// </summary>
        public Vector3[] Positions;

        /// <summary>
        /// Dirt levels of all floor tiles.
        /// </summary>
        public Floor.DirtLevel[] States;

        /// <summary>
        /// If each floor tile is likely to get dirty or not.
        /// </summary>
        public bool[] LikelyToGetDirty;

        /// <summary>
        /// Display the details of the percept.
        /// </summary>
        /// <returns>String with the details of the percept.</returns>
        public override string DetailsDisplay()
        {
            int dirtyCount = States.Count(state => state >= Floor.DirtLevel.Dirty);
            int likelyCount = LikelyToGetDirty.Count(likely => likely);
            return $"{Positions.Length} floor tiles, {dirtyCount} dirty, {likelyCount} likely to get dirty.";
        }
    }
}