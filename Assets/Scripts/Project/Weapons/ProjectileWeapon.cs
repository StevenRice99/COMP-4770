using UnityEngine;

namespace Project.Weapons
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
            
            GameObject projectile = Instantiate(bulletPrefab, Soldier.shootPosition.position, barrel.rotation);
            projectile.name = $"{name} Projectile";
            ProjectileBullet projectileBullet = projectile.GetComponent<ProjectileBullet>();
            projectileBullet.weaponIndex = Index;
            projectileBullet.shotBy = Soldier;
            projectileBullet.damage = damage;
            projectileBullet.distance = distance;
            projectileBullet.velocity = velocity;
            Destroy(projectile, time);
        }
    }
}