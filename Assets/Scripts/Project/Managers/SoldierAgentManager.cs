using System.Linq;
using Project.Agents;
using Project.Pickups;
using Project.Positions;
using UnityEngine;

namespace Project.Managers
{
    public class SoldierAgentManager : AgentManager
    {
        /// <summary>
        /// Getter to cast the AgentManager singleton into a SoldierAgentManager.
        /// </summary>
        public static SoldierAgentManager SoldierAgentManagerSingleton => Singleton as SoldierAgentManager;

        [SerializeField]
        [Range(1, 15)]
        private int soldiersPerTeam = 1;

        [Min(1)]
        public int health = 100;

        [Min(0)]
        public float respawn = 10;

        [Min(0)]
        public float pickupTimer = 10;

        [Min(0)]
        public float memoryTime = 5;

        [Min(0)]
        public float maxWaitTime = 5;

        [Range(0, 1)]
        public float volume;

        [SerializeField]
        private GameObject soldierPrefab;

        public Material red;

        public Material blue;
        
        public SpawnPoint[] SpawnPoints { get; private set; }

        private StrategicPoint[] _strategicPoints;

        public Vector3 GetPoint(bool redTeam, bool defensive)
        {
            StrategicPoint[] points = _strategicPoints.Where(s => s.redTeam == redTeam && s.defensive == defensive).ToArray();
            StrategicPoint[] open = points.Where(s => s.Open).ToArray();
            return open.Length > 0 ? open[Random.Range(0, open.Length)].transform.position : points[Random.Range(0, points.Length)].transform.position;
        }
        
        protected override void Start()
        {
            base.Start();

            SpawnPoints = FindObjectsOfType<SpawnPoint>();

            _strategicPoints = FindObjectsOfType<StrategicPoint>();

            for (int i = 0; i < soldiersPerTeam * 2; i++)
            {
                Instantiate(soldierPrefab);
            }
        }

        protected override void Update()
        {
            base.Update();

            Detect();

            int layerMask = LayerMask.GetMask("Default", "Obstacle", "Ground", "Projectile", "HitBox");

            foreach (Agent agent in Agents)
            {
                if (agent is not SoldierAgent { Alive: true } soldier)
                {
                    agent.StopLookAtTarget();
                    continue;
                }

                if (soldier.Target != null)
                {
                    Vector3 position = soldier.Target.Value.Position;
                    soldier.LookAtTarget(position);
                    soldier.headPosition.LookAt(position);
                    soldier.headPosition.localRotation = Quaternion.Euler(soldier.headPosition.localRotation.eulerAngles.x, 0, 0);
                    soldier.weaponPosition.LookAt(position);
                    soldier.weaponPosition.localRotation = Quaternion.Euler(soldier.weaponPosition.localRotation.eulerAngles.x, 0, 0);
                    
                    if (!soldier.Weapons[soldier.WeaponIndex].CanShoot || !Physics.Raycast(soldier.shootPosition.position, soldier.shootPosition.forward, out RaycastHit hit, float.MaxValue, layerMask))
                    {
                        continue;
                    }

                    SoldierAgent attacked;
                    Transform tr = hit.collider.transform;
                    do
                    {
                        attacked = tr.GetComponent<SoldierAgent>();
                        if (attacked != null)
                        {
                            soldier.Weapons[soldier.WeaponIndex].Shoot();
                            break;
                        }
                        tr = tr.parent;
                    } while (tr != null);
                    
                    continue;
                }

                soldier.StopLookAtTarget();
                soldier.headPosition.localRotation = Quaternion.identity;
                soldier.weaponPosition.localRotation = Quaternion.identity;
            }
        }
        
        private void Detect()
        {
            foreach (Agent agent in Agents)
            {
                if (agent is not SoldierAgent { Alive: true } soldier)
                {
                    continue;
                }
                
                foreach (SoldierAgent enemy in soldier.SeeEnemies())
                {
                    SoldierAgent.EnemyMemory memory = soldier.EnemiesDetected.FirstOrDefault(e => e.Enemy == enemy);
                    if (memory != null)
                    {
                        memory.DeltaTime = 0;
                        memory.Enemy = enemy;
                        memory.Position = enemy.headPosition.position;
                        memory.Visible = true;
                        memory.HasFlag = FlagPickup.RedFlag != null && FlagPickup.RedFlag.carryingPlayer == enemy || FlagPickup.BlueFlag != null && FlagPickup.BlueFlag.carryingPlayer == enemy;
                    }
                    else
                    {
                        soldier.EnemiesDetected.Add(new SoldierAgent.EnemyMemory
                        {
                            DeltaTime = 0,
                            Enemy = enemy,
                            Position = enemy.headPosition.position,
                            Visible = true,
                            HasFlag = FlagPickup.RedFlag != null && FlagPickup.RedFlag.carryingPlayer == enemy || FlagPickup.BlueFlag != null && FlagPickup.BlueFlag.carryingPlayer == enemy
                        });
                    }

                    if (soldier.Target == null || soldier.Target.Value.Enemy != enemy)
                    {
                        continue;
                    }

                    soldier.Target = new SoldierAgent.TargetData
                    {
                        Enemy = enemy,
                        Position = enemy.headPosition.position
                    };
                }
            }
        }
    }
}