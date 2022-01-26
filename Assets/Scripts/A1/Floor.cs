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

        public Material cleanMaterial;

        public Material dirtyMaterial;

        public Material veryDirtyMaterial;

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

        private void UpdateMaterial()
        {
            switch (State)
            {
                case DirtLevel.Clean:
                    _meshRenderer.material = cleanMaterial;
                    break;
                case DirtLevel.Dirty:
                    _meshRenderer.material = dirtyMaterial;
                    break;
                default:
                    _meshRenderer.material = veryDirtyMaterial;
                    break;
            }
        }
    }
}