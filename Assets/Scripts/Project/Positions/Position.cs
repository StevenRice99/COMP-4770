using UnityEngine;

namespace Project.Positions
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class Position : MonoBehaviour
    {
        public bool redTeam = true;
        
        protected int Count;
        
        private void Start()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            ++Count;
        }

        private void OnTriggerExit(Collider other)
        {
            --Count;
        }
    }
}