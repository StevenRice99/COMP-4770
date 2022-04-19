using System.Collections;
using UnityEngine;

namespace Project.Positions
{
    public class SpawnPoint : Position
    {
        private bool _used;

        public bool Open => !_used && Count == 0;

        public void Use()
        {
            StopAllCoroutines();
            StartCoroutine(UseDelay());
        }

        private IEnumerator UseDelay()
        {
            _used = true;
            yield return new WaitForSeconds(1);
            _used = false;
        }
    }
}