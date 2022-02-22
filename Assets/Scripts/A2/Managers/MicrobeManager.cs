using UnityEngine;

namespace A2.Managers
{
    public class MicrobeManager : AgentManager
    {
        [SerializeField]
        private GameObject microbePrefab;

        [SerializeField]
        private float radius;
        
        public enum MicrobeEvents
        {
            Eaten = 0,
            AttractMate,
            RejectMate
        }

        protected override void Start()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(floor.GetComponent<Collider>());
            floor.transform.position = new Vector3(0, -1, 0);
            floor.transform.localScale = new Vector3(radius, 1, radius);
            floor.name = "Floor";
            
            base.Start();
        }
    }
}