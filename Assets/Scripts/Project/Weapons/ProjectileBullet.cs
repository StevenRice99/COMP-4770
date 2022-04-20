using System.Collections.Generic;
using System.Linq;
using Project.Agents;
using UnityEngine;

namespace Project.Weapons
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileBullet : MonoBehaviour
    {
        [HideInInspector]
        public SoldierAgent shotBy;

        [HideInInspector]
        public int weaponIndex;
        
        [HideInInspector]
        public float velocity;
        
        [HideInInspector]
        public int damage;

        [HideInInspector]
        public float distance;

        private Rigidbody _rb;
        
        private void Start()
        {
            Collider col = GetComponent<Collider>();
            foreach (Collider hitBox in shotBy.Colliders)
            {
                if (hitBox != null && hitBox.enabled)
                {
                    Physics.IgnoreCollision(col, hitBox, true);
                }
            }
            
            _rb = GetComponent<Rigidbody>();
            _rb.AddRelativeForce(Vector3.forward * velocity, ForceMode.VelocityChange);
            _rb.useGravity = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision.transform);
        }
        
        private void HandleCollision(Transform tr)
        {
            SoldierAgent attacked;
            do
            {
                attacked = tr.GetComponent<SoldierAgent>();
                tr = tr.parent;
            } while (attacked == null && tr != null);

            if (attacked != null && attacked.RedTeam != shotBy.RedTeam)
            {
                attacked.Damage(damage, shotBy);
            }
            
            if (distance > 0)
            {
                int layerMask = LayerMask.GetMask("Default", "Obstacle", "Ground", "Projectile", "HitBox");

                foreach (SoldierAgent soldier in FindObjectsOfType<SoldierAgent>().Where(p => p != shotBy && p.RedTeam != shotBy.RedTeam && p != attacked).ToArray())
                {
                    Collider[] hitBoxes = soldier.GetComponentsInChildren<Collider>().Where(c => c.gameObject.layer == LayerMask.NameToLayer("HitBox")).ToArray();
                    
                    Vector3 position = soldier.transform.position;
                    List<Vector3> points = new() { position, new Vector3(position.x, position.y + 0.1f, position.z), soldier.shootPosition.position };
                    points.AddRange(hitBoxes.Select(h => h.bounds).Select(b => b.ClosestPoint(transform.position)));
                
                    foreach (Vector3 point in points.Where(p => Vector3.Distance(p, transform.position) <= distance).OrderBy(p => Vector3.Distance(p, transform.position)))
                    {
                        if (!Physics.Linecast(transform.position, point, out RaycastHit hit, layerMask) || !hitBoxes.Contains(hit.collider))
                        {
                            continue;
                        }
                        
                        soldier.Damage(Mathf.Max((int) (damage * (1 - Vector3.Distance(point, transform.position) / distance)), 1), shotBy);
                        break;
                    }
                }
            }

            Vector3 p = transform.position;
            shotBy.Weapons[weaponIndex].ImpactAudio(p, 1);
            shotBy.Weapons[weaponIndex].ImpactVisual(p, Vector3.zero);
            Destroy(gameObject);
        }
    }
}