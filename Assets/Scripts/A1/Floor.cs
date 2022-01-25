using UnityEngine;

namespace A1
{
    public class Floor : MonoBehaviour
    {
        public bool IsDirty { get; private set; }

        public Material cleanMaterial;

        public Material dirtyMaterial;

        private MeshRenderer _meshRenderer;

        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            Clean();
        }

        public void Clean()
        {
            IsDirty = false;
            _meshRenderer.material = cleanMaterial;
        }

        public void Dirty()
        {
            IsDirty = true;
            _meshRenderer.material = dirtyMaterial;
        }
    }
}