using UnityEngine;

namespace A1
{
    public class Floor : MonoBehaviour
    {
        public enum DirtLevel : byte
        {
            Clean,
            Dirty,
            VeryDirty,
            ExtremelyDirty
        }

        public bool LikelyToGetDirty { get; private set; }

        private Material _cleanMaterial;

        private Material _dirtyMaterial;

        private Material _veryDirtyMaterial;

        private Material _extremelyDirtyMaterial;

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
            if (State == DirtLevel.ExtremelyDirty)
            {
                return;
            }

            State++;
            UpdateMaterial();
        }

        public void Setup(bool likelyToGetDirty, Material cleanMaterial, Material dirtyMaterial, Material veryDirtyMaterial, Material extremelyDirtyMaterial)
        {
            LikelyToGetDirty = likelyToGetDirty;
            _cleanMaterial = cleanMaterial;
            _dirtyMaterial = dirtyMaterial;
            _veryDirtyMaterial = veryDirtyMaterial;
            _extremelyDirtyMaterial = extremelyDirtyMaterial;
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