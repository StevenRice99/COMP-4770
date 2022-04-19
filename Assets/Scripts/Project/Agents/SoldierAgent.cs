using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Managers;
using Project.Pickups;
using Project.Positions;
using Project.Weapons;
using UnityEngine;

namespace Project.Agents
{
    public class SoldierAgent : CharacterAgent
    {
        private enum SoliderRole : byte
        {
            Dead,
            Collector,
            Attacker,
            Defender
        }
        private enum WeaponChoices
        {
            MachineGun,
            Shotgun,
            Sniper,
            RocketLauncher,
            Pistol
        }
        
        public override void Perform()
        {
            if (_role == SoliderRole.Dead)
            {
                return;
            }
            
            Target = ChooseTarget();

            Think();

            Shoot();
            
            Cleanup();
            
            base.Perform();
        }
        
        public class EnemyMemory
        {
            public SoldierAgent Enemy;

            public bool HasFlag;

            public bool Visible;

            public Vector3 Position;

            public float DeltaTime;
        }

        public struct TargetData
        {
            public SoldierAgent Enemy;
            
            public Vector3 Position;
        }
        
        private static readonly List<SoldierAgent> TeamRed = new();
        
        private static readonly List<SoldierAgent> TeamBlue = new();

        public Transform headPosition;

        public Transform shootPosition;

        public Transform flagPosition;

        public Transform weaponPosition;

        [SerializeField]
        private MeshRenderer[] colorVisuals;

        [SerializeField]
        private MeshRenderer[] otherVisuals;

        public bool RedTeam { get; private set; }

        public int Health { get; private set; }
        
        public Weapon[] Weapons { get; private set; }
        
        public int WeaponIndex { get; private set; }
        
        public TargetData? Target { get; set; }

        public bool Alive => _role != SoliderRole.Dead;
        
        private SoliderRole _role;

        private Collider[] _colliders;

        public readonly List<EnemyMemory> EnemiesDetected = new();

        private bool CarryingFlag => RedTeam ? FlagPickup.BlueFlag != null && FlagPickup.BlueFlag.carryingPlayer == this : FlagPickup.RedFlag != null && FlagPickup.RedFlag.carryingPlayer == this;

        private bool TeamHasFlag => RedTeam ? FlagPickup.BlueFlag != null && FlagPickup.BlueFlag.carryingPlayer != null : FlagPickup.RedFlag != null && FlagPickup.RedFlag.carryingPlayer != null;

        private bool EnemyHasFlag => RedTeam ? FlagPickup.RedFlag != null && FlagPickup.RedFlag.carryingPlayer != null : FlagPickup.BlueFlag != null && FlagPickup.BlueFlag.carryingPlayer != null;
        
        private bool FLagAtBase => RedTeam ? FlagPickup.RedFlag != null && FlagPickup.RedFlag.transform.position == FlagPickup.RedFlag.SpawnPosition : FlagPickup.BlueFlag != null && FlagPickup.BlueFlag.transform.position == FlagPickup.BlueFlag.SpawnPosition;

        private Vector3 EnemyFlag => RedTeam ? FlagPickup.BlueFlag != null ? FlagPickup.BlueFlag.transform.position : Vector3.zero : FlagPickup.RedFlag != null ? FlagPickup.RedFlag.transform.position : Vector3.zero;
        
        private Vector3 Base => RedTeam ? FlagPickup.RedFlag != null ? FlagPickup.RedFlag.SpawnPosition : Vector3.zero : FlagPickup.BlueFlag != null ? FlagPickup.BlueFlag.SpawnPosition : Vector3.zero;
        
        public override void Move()
        {
            if (CharacterController == null || !CharacterController.enabled)
            {
                EnemiesDetected.Clear();
                return;
            }
            
            base.Move();
        }
        
        public void Damage(int amount, SoldierAgent shotBy)
        {
            if (_role == SoliderRole.Dead)
            {
                return;
            }
            
            Health -= amount;
            if (Health > 0)
            {
                return;
            }
            
            // ADD KILL POINTS.

            StopAllCoroutines();
            StartCoroutine(Respawn());
        }

        public void Hear(SoldierAgent enemy, float distance)
        {
            if (Vector3.Distance(headPosition.position, enemy.headPosition.position) > distance)
            {
                return;
            }
            
            EnemyMemory memory = EnemiesDetected.FirstOrDefault(e => e.Enemy == enemy);
            if (memory != null)
            {
                memory.DeltaTime = 0;
                memory.Position = enemy.headPosition.position;
                memory.Visible = false;
                memory.HasFlag = false;
                return;
            }
            
            EnemiesDetected.Add(new EnemyMemory
            {
                DeltaTime = 0,
                Position = enemy.headPosition.position,
                Visible = false,
                HasFlag = false
            });
        }

        public void Heal()
        {
            if (_role == SoliderRole.Dead)
            {
                return;
            }

            Health = SoldierAgentManager.SoldierAgentManagerSingleton.health;
        }

        public IEnumerable<SoldierAgent> GetEnemies()
        {
            return (RedTeam ? TeamBlue : TeamRed).Where(s => s.Alive);
        }

        protected override void Start()
        {
            base.Start();

            Weapons = GetComponentsInChildren<Weapon>();
            for (int i = 0; i < Weapons.Length; i++)
            {
                Weapons[i].Soldier = this;
                Weapons[i].Index = i;
            }

            RedTeam = TeamRed.Count <= TeamBlue.Count;
            if (RedTeam)
            {
                TeamRed.Add(this);
            }
            else
            {
                TeamBlue.Add(this);
            }

            name = (RedTeam ? "Solider Red " : "Solider Blue ") + (RedTeam ? TeamRed.Count : TeamBlue.Count);

            List<Collider> colliders = GetComponents<Collider>().ToList();
            colliders.AddRange(GetComponentsInChildren<Collider>());
            _colliders = colliders.Distinct().ToArray();

            foreach (MeshRenderer meshRenderer in colorVisuals)
            {
                meshRenderer.material = RedTeam ? SoldierAgentManager.SoldierAgentManagerSingleton.red : SoldierAgentManager.SoldierAgentManagerSingleton.blue;
            }
            
            Spawn();
        }

        private void Think()
        {
            // ADD THINKING LOGIC.

            if (CarryingFlag)
            {
                Navigate(Base);
            }
            else if (!TeamHasFlag)
            {
                Navigate(EnemyFlag);
            }
        }

        private void AssignRoles()
        {
            SoldierAgent[] team = GetTeam();
            for (int i = 0; i < team.Length; i++)
            {
                if (i == 0)
                {
                    team[i]._role = SoliderRole.Collector;
                }
                else if (i <= team.Length / 2)
                {
                    team[i]._role = SoliderRole.Attacker;
                }
                else
                {
                    team[i]._role = SoliderRole.Defender;
                }
            }
        }

        private IEnumerator Respawn()
        {
            _role = SoliderRole.Dead;
            ToggleAlive();
            AssignRoles();
            EnemiesDetected.Clear();
            Target = null;
            ClearPath();
            StopLookAtTarget();
            MoveVelocity = Vector2.zero;
            
            yield return new WaitForSeconds(SoldierAgentManager.SoldierAgentManagerSingleton.respawn);
            
            Spawn();
        }

        private void Spawn()
        {
            SpawnPoint[] points = SoldierAgentManager.SoldierAgentManagerSingleton.SpawnPoints.Where(p => p.redTeam == RedTeam).ToArray();
            SpawnPoint[] open = points.Where(p => p.Open).ToArray();
            SpawnPoint spawn = open.Length > 0 ? open[Random.Range(0, open.Length)] : points[Random.Range(0, points.Length)];

            CharacterController controller = GetComponent<CharacterController>();
            controller.enabled = false;

            Transform spawnTr = spawn.transform;
            transform.position = spawnTr.position;
            Visuals.rotation = spawnTr.rotation;
            
            spawn.Use();
            
            // ReSharper disable once Unity.InefficientPropertyAccess
            controller.enabled = true;
            
            _role = SoliderRole.Collector;
            AssignRoles();
            Heal();
            SelectWeapon(0);
            ToggleAlive();
        }

        private SoldierAgent[] GetTeam()
        {
            IEnumerable<SoldierAgent> team = (RedTeam ? TeamRed : TeamBlue).Where(s => s.Alive);
            if (FlagPickup.RedFlag != null && FlagPickup.BlueFlag != null)
            {
                team = RedTeam ? team.OrderBy(s => Vector3.Distance(s.transform.position, FlagPickup.BlueFlag.transform.position)) : team.OrderBy(s => Vector3.Distance(s.transform.position, FlagPickup.RedFlag.transform.position));
            }
            
            return team.ToArray();
        }

        private void ToggleAlive()
        {
            foreach (MeshRenderer meshRenderer in colorVisuals)
            {
                meshRenderer.enabled = Alive;
            }
            
            foreach (MeshRenderer meshRenderer in otherVisuals)
            {
                meshRenderer.enabled = Alive;
            }

            foreach (Collider col in _colliders)
            {
                col.enabled = Alive;
            }

            WeaponVisible();
        }

        private void Shoot()
        {
            if (!Weapons[WeaponIndex].CanShoot || !Physics.Raycast(shootPosition.position, shootPosition.forward, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Default", "Obstacle", "Ground", "Projectile", "HitBox")))
            {
                return;
            }

            SoldierAgent attacked;
            Transform tr = hit.collider.transform;
            do
            {
                attacked = tr.GetComponent<SoldierAgent>();
                tr = tr.parent;
            } while (attacked == null && tr != null);
                
            if (attacked != null)
            {
                Weapons[WeaponIndex].Shoot();
            }
        }

        private void SelectWeapon(int i)
        {
            WeaponIndex = Mathf.Clamp(i, 0, Weapons.Length - 1);
            lookSpeed = Weapons[WeaponIndex].rotationSpeed;
            WeaponVisible();
        }

        private void WeaponVisible()
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                Weapons[i].Visible(Alive && i == WeaponIndex);
            }
        }

        public IEnumerable<SoldierAgent> SeeEnemies()
        {
            return GetEnemies().Where(enemy => !Physics.Linecast(headPosition.position, enemy.headPosition.position, AgentManager.Singleton.obstacleLayers)).ToArray();
        }

        private void Cleanup()
        {
            for (int i = 0; i < EnemiesDetected.Count; i++)
            {
                EnemiesDetected[i].DeltaTime += DeltaTime;
                if (EnemiesDetected[i].DeltaTime > SoldierAgentManager.SoldierAgentManagerSingleton.memoryTime || EnemiesDetected[i].Enemy._role == SoliderRole.Dead)
                {
                    EnemiesDetected.RemoveAt(i--);
                }
            }
        }

        private TargetData? ChooseTarget()
        {
            if (EnemiesDetected.Count == 0)
            {
                return null;
            }
            
            EnemyMemory target = EnemiesDetected.OrderBy(e => e.HasFlag).ThenBy(e => e.Visible).ThenBy(e => e.DeltaTime).ThenBy(e => Vector3.Distance(transform.position, e.Position)).First();
            
            return new TargetData
            {
                Enemy = target.Enemy,
                Position = target.Position
            };
        }
    }
}