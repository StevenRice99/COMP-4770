using UnityEngine;

namespace ArtificialIntelligence
{
    public abstract class Agent : MonoBehaviour
    {
        [SerializeField]
        private float tick;

        protected float Tick { get; private set; }

        protected virtual void Update()
        {
            Tick += Time.deltaTime;
            if (Tick < tick)
            {
                return;
            }

            if (Sense())
            {
                if (Think())
                {
                    Act();
                }
            }

            Tick = 0;
        }

        protected abstract bool Sense();

        protected abstract bool Think();

        protected abstract void Act();
    }
}