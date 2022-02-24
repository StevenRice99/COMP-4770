using System.Collections;
using UnityEngine;

namespace A2
{
    public class ParticleDisplay : MonoBehaviour
    {
        [SerializeField]
        [Min(float.Epsilon)]
        private float duration = 1f;

        private void Start()
        {
            StartCoroutine(DestroyAfterSeconds());
        }

        private IEnumerator DestroyAfterSeconds()
        {
            yield return new WaitForSeconds(duration);
            Destroy(gameObject);
        }
    }
}