using System.Collections.Generic;
using System.Linq;
using Project.Minds;
using UnityEngine;

namespace Project.Weapons
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileBullet : MonoBehaviour
    {
        [HideInInspector]
        public SoldierBrain shotBy;

        [HideInInspector]
        public int weaponIndex;
        
        [HideInInspector]
        public float velocity;
        
        [HideInInspector]
        public int damage;

        [HideInInspector]
        public float distance;
        
        private void Start()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddRelativeForce(Vector3.forward * velocity, ForceMode.VelocityChange);
            rb.useGravity = false;
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            HandleCollision(collision);
        }
        
        private void HandleCollision(Collision collision)
        {
            SoldierBrain attacked;
            Transform tr = collision.transform;
            do
            {
                attacked = tr.GetComponent<SoldierBrain>();
                tr = tr.parent;
            } while (attacked == null && tr != null);
            
            if (attacked == shotBy)
            {
                return;
            }
            
            foreach (Collider c in GetComponentsInChildren<Collider>())
            {
                c.enabled = false;
            }
            
            if (attacked != null && attacked.RedTeam != shotBy.RedTeam)
            {
                attacked.Damage(damage, shotBy);
            }
            
            if (distance > 0)
            {
                int layerMask = LayerMask.GetMask("Default", "HitBox");
                
                SoldierBrain[] soldierBrains = FindObjectsOfType<SoldierBrain>().Where(p => p != shotBy && p.RedTeam != shotBy.RedTeam && p != attacked).ToArray();

                foreach (SoldierBrain soldierBrain in soldierBrains)
                {
                    Collider[] hitBoxes = soldierBrain.GetComponentsInChildren<Collider>().Where(c => c.gameObject.layer == LayerMask.NameToLayer("HitBox")).ToArray();
                    
                    Vector3 position = soldierBrain.transform.position;
                    List<Vector3> points = new() { position, new Vector3(position.x, position.y + 0.1f, position.z), soldierBrain.shootPosition.position };
                    points.AddRange(hitBoxes.Select(h => h.bounds).Select(b => b.ClosestPoint(transform.position)));
                
                    foreach (Vector3 point in points.Where(p => Vector3.Distance(p, transform.position) <= distance).OrderBy(p => Vector3.Distance(p, transform.position)))
                    {
                        if (!Physics.Linecast(transform.position, point, out RaycastHit hit, layerMask) || !hitBoxes.Contains(hit.collider))
                        {
                            continue;
                        }
                        
                        attacked.Damage(Mathf.Max((int) (damage * (1 - Vector3.Distance(point, transform.position) / distance)), 1), shotBy);
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