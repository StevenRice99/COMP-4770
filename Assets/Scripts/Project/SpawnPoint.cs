using System.Collections;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(BoxCollider))]
    public class SpawnPoint : MonoBehaviour
    {
        public bool redTeam = true;

        private int _count;

        private bool _used;

        public bool Open => !_used && _count == 0;

        public void Use()
        {
            StopAllCoroutines();
            StartCoroutine(UseDelay());
        }

        private void Start()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            ++_count;
        }

        private void OnTriggerExit(Collider other)
        {
            --_count;
        }

        private IEnumerator UseDelay()
        {
            _used = true;
            yield return new WaitForSeconds(1);
            _used = false;
        }
    }
}