using System.Linq;
using UnityEngine;

public abstract class NodeBase : MonoBehaviour
{
    public void Finish()
    {
        if (transform.childCount == 0 && !GetComponents<MonoBehaviour>().Any(m => m != this && m.enabled))
        {
            Destroy(gameObject);
            return;
        }

        enabled = false;
        Destroy(this);
    }
}