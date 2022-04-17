using UnityEngine;

namespace Project.Actuators.Weapons
{
    public class ProjectileWeapon : Weapon
    {
        [SerializeField]
        [Tooltip("How fast the projectile should travel.")]
        private float velocity = 10;

        [SerializeField]
        [Tooltip("Splash damage distance.")]
        private float distance;
        
        [SerializeField]
        [Tooltip("The bullet prefab.")]
        private GameObject bulletPrefab;
        
        protected override void Shoot(out Vector3[] positions)
        {
            positions = new[] { barrel.position };
            
            GameObject bullet = Instantiate(bulletPrefab, SoldierBrain.shootPosition.position, barrel.rotation);
            bullet.name = $"{name} Bullet";
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            ProjectileBullet projectileBullet = bullet.GetComponent<ProjectileBullet>();
            projectileBullet.weaponIndex = Index;
            projectileBullet.shotBy = SoldierBrain;
            projectileBullet.damage = damage;
            projectileBullet.distance = distance;
            projectileBullet.velocity = velocity;
            rb.useGravity = false;
            rb.AddRelativeForce(Vector3.forward * velocity, ForceMode.VelocityChange);
            Destroy(bullet, time);
        }
    }
}