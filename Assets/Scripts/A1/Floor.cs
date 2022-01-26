using UnityEngine;

namespace A1
{
    public class Floor : MonoBehaviour
    {
        public enum DirtLevel : byte
        {
            Clean,
            Dirty,
            VeryDirty
        }

        public bool LikelyToGetDirty { get; private set; }

        private Material _cleanMaterial;

        private Material _dirtyMaterial;

        private Material _veryDirtyMaterial;

        private MeshRenderer _meshRenderer;

        public DirtLevel State { get; private set; }

        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            UpdateMaterial();
        }

        public void Clean()
        {
            if (State == DirtLevel.Clean)
            {
                return;
            }
            
            State--;
            UpdateMaterial();
        }

        public void Dirty()
        {
            if (State == DirtLevel.VeryDirty)
            {
                return;
            }

            State++;
            UpdateMaterial();
        }

        public void Setup(bool likelyToGetDirty, Material cleanMaterial, Material dirtyMaterial, Material veryDirtyMaterial)
        {
            LikelyToGetDirty = likelyToGetDirty;
            _cleanMaterial = cleanMaterial;
            _dirtyMaterial = dirtyMaterial;
            _veryDirtyMaterial = veryDirtyMaterial;
        }

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
                default:
                    _meshRenderer.material = _veryDirtyMaterial;
                    break;
            }
        }
    }
}