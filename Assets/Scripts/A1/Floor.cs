﻿using UnityEngine;

namespace A1
{
    /// <summary>
    /// Hold data for individual floor components.
    /// </summary>
    public class Floor : MonoBehaviour
    {
        /// <summary>
        /// The current dirt level of this floor.
        /// </summary>
        public enum DirtLevel : byte
        {
            Clean,
            Dirty,
            VeryDirty,
            ExtremelyDirty
        }

        /// <summary>
        /// If this floor is likely to get dirty. Floors where this is true are twice as likely to get more dirty than other floor tiles.
        /// </summary>
        public bool LikelyToGetDirty { get; private set; }

        /// <summary>
        /// The material to display when this floor is clean.
        /// </summary>
        private Material _cleanMaterial;

        /// <summary>
        /// The material to display when this floor is dirty.
        /// </summary>
        private Material _dirtyMaterial;

        /// <summary>
        /// The material to display when this floor is very dirty.
        /// </summary>
        private Material _veryDirtyMaterial;

        /// <summary>
        /// The material to display when this floor is extremely dirty.
        /// </summary>
        private Material _extremelyDirtyMaterial;

        /// <summary>
        /// The mesh renderer to apply the materials to.
        /// </summary>
        private MeshRenderer _meshRenderer;

        /// <summary>
        /// How dirty this floor tile is.
        /// </summary>
        public DirtLevel State { get; private set; }

        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            UpdateMaterial();
        }

        /// <summary>
        /// Clean this floor tile reducing its dirty level by one.
        /// </summary>
        public void Clean()
        {
            if (State == DirtLevel.Clean)
            {
                return;
            }
            
            State--;
            UpdateMaterial();
        }

        /// <summary>
        /// Add dirt to this floor tile increasing its dirty level by one.
        /// </summary>
        public void Dirty()
        {
            if (State == DirtLevel.ExtremelyDirty)
            {
                return;
            }

            State++;
            UpdateMaterial();
        }

        /// <summary>
        /// Configure this floor tile.
        /// </summary>
        /// <param name="likelyToGetDirty">If this floor is likely to get dirty.</param>
        /// <param name="cleanMaterial">The material to display when this floor is clean.</param>
        /// <param name="dirtyMaterial">The material to display when this floor is dirty.</param>
        /// <param name="veryDirtyMaterial">The material to display when this floor is very dirty.</param>
        /// <param name="extremelyDirtyMaterial">The material to display when this floor is extremely dirty.</param>
        public void Setup(bool likelyToGetDirty, Material cleanMaterial, Material dirtyMaterial, Material veryDirtyMaterial, Material extremelyDirtyMaterial)
        {
            LikelyToGetDirty = likelyToGetDirty;
            _cleanMaterial = cleanMaterial;
            _dirtyMaterial = dirtyMaterial;
            _veryDirtyMaterial = veryDirtyMaterial;
            _extremelyDirtyMaterial = extremelyDirtyMaterial;
        }

        /// <summary>
        /// Update the material based on the current state of the floor.
        /// </summary>
        private void UpdateMaterial()
        {
            switch (State)
            {
                case DirtLevel.Clean:
                    _meshRenderer.material = _cleanMaterial;
                    break;
                case DirtLevel.Dirty:
                    _meshRenderer.material = _dirtyMaterial;
                    break;
                case DirtLevel.VeryDirty:
                    _meshRenderer.material = _veryDirtyMaterial;
                    break;
                default:
                    _meshRenderer.material = _extremelyDirtyMaterial;
                    break;
            }
        }
    }
}