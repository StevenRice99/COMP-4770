using UnityEngine;

namespace SimpleIntelligence.Components
{
    public abstract class TimedComponent : MonoBehaviour
    {
        protected float ElapsedTime;

        protected virtual void Update()
        {
            ElapsedTime += Time.deltaTime;
        }
    }
}