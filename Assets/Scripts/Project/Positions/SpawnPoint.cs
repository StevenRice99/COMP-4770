using System.Collections;
using UnityEngine;

namespace Project.Positions
{
    public class SpawnPoint : Position
    {
        public bool Used { get; set; }

        public bool Open => !Used && Count == 0;

        public void Use()
        {
            StopAllCoroutines();
            StartCoroutine(UseDelay());
        }

        private IEnumerator UseDelay()
        {
            Used = true;
            yield return new WaitForSeconds(1);
            Used = false;
        }
    }
}