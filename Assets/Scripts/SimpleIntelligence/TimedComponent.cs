using System;
using UnityEngine;

namespace SimpleIntelligence
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