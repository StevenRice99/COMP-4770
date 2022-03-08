using UnityEngine;

namespace A3
{
    public class Spinner : MonoBehaviour
    {
        [SerializeField]
        [Min(0)]
        private float speed;

        private void Update()
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y + speed * Time.deltaTime, 0));
        }
    }
}