using System.Collections.Generic;
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

        [Min(1)]
        public int lowHealth = 50;

        [Min(0)]
        public float maxWaitTime = 5;

        [Min(0)]
        public float distanceClose = 10;

        [Min(0)]
        public float distanceFar = 20;

        [Range(0, 1)]
        public float volume;

        [SerializeField]
        private GameObject soldierPrefab;

        public Material red;

        public Material blue;
        
        public SpawnPoint[] SpawnPoints { get; private set; }

        private StrategicPoint[] _strategicPoints;

        private HealthWeaponPickup[] _healthWeaponPickups;
        
        public List<SoldierAgent> Sorted { get; private set; }
        
        public int MostCaptures { get; private set; }
        
        public int MostReturns { get; private set; }
        
        public int MostKills { get; private set; }
        
        public int LeastDeaths { get; private set; }

        private bool _best = true;

        public Vector3 GetPoint(bool redTeam, bool defensive)
        {
            StrategicPoint[] points = _strategicPoints.Where(s => s.redTeam == redTeam && s.defensive == defensive).ToArray();
            StrategicPoint[] open = points.Where(s => s.Open).ToArray();
            return open.Length > 0 ? open[Random.Range(0, open.Length)].transform.position : points[Random.Range(0, points.Length)].transform.position;
        }

        public Vector3? GetHealth(Vector3 soldierPosition)
        {
            return GetWeapon(soldierPosition, -1);
        }

        public Vector3? GetWeapon(Vector3 soldierPosition, int weaponIndex)
        {
            HealthWeaponPickup[] ready = _healthWeaponPickups.Where(p => p.weaponIndex == weaponIndex && p.Ready).ToArray();
            return ready.Length > 0 ? ready.OrderBy(p => Vector3.Distance(soldierPosition, p.transform.position)).First().transform.position : null;
        }

        public void UpdateSorted()
        {
            Sorted = Sorted.OrderByDescending(s => s.Captures).ThenByDescending(s => s.Kills).ThenBy(s => s.Deaths).ThenByDescending(s => s.Returns).ToList();
            MostCaptures = Sorted.OrderByDescending(s => s.Captures).First().Captures;
            MostReturns = Sorted.OrderByDescending(s => s.Returns).First().Returns;
            MostKills = Sorted.OrderByDescending(s => s.Kills).First().Kills;
            LeastDeaths = Sorted.OrderBy(s => s.Deaths).First().Deaths;
        }
        
        protected override void Start()
        {
            base.Start();

            SpawnPoints = FindObjectsOfType<SpawnPoint>();

            _strategicPoints = FindObjectsOfType<StrategicPoint>();

            _healthWeaponPickups = FindObjectsOfType<HealthWeaponPickup>();

            for (int i = 0; i < soldiersPerTeam * 2; i++)
            {
                Instantiate(soldierPrefab);
            }

            Sorted = FindObjectsOfType<SoldierAgent>().ToList();
        }

        protected override void Update()
        {
            base.Update();

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
                        Position = enemy.headPosition.position,
                        Visible = true
                    };
                }
            }

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

                    Transform tr = hit.collider.transform;
                    do
                    {
                        SoldierAgent attacked = tr.GetComponent<SoldierAgent>();
                        if (attacked != null)
                        {
                            if (attacked.RedTeam != soldier.RedTeam)
                            {
                                soldier.Weapons[soldier.WeaponIndex].Shoot();
                            }
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

            if (_best)
            {
                SoldierAgent bestAlive = Sorted.FirstOrDefault(s => s.Alive);
                SetSelectedAgent(bestAlive != null ? bestAlive : Sorted[0]);
            }
            
            if (selectedCamera != null)
            {
                transform.position = selectedCamera.transform.position;
            }
        }
        
        /// <summary>
        /// Render buttons to reset the level or follow the best agent.
        /// </summary>
        /// <param name="x">X rendering position. In most cases this should remain unchanged.</param>
        /// <param name="y">Y rendering position. Update this with every component added and return it.</param>
        /// <param name="w">Width of components. In most cases this should remain unchanged.</param>
        /// <param name="h">Height of components. In most cases this should remain unchanged.</param>
        /// <param name="p">Padding of components. In most cases this should remain unchanged.</param>
        /// <returns>The updated Y position after all custom rendering has been done.</returns>
        protected override float CustomRendering(float x, float y, float w, float h, float p)
        {
            // Regenerate the floor button.
            if (GuiButton(x, y, w, h, "Reset"))
            {
                Reset();
            }
            
            y = NextItem(y, h, p);
            if (GuiButton(x, y, w, h, _best ? "Manual" : "Best"))
            {
                _best = !_best;
            }
            
            return NextItem(y, h, p);
        }

        private void Reset()
        {
            if (FlagPickup.RedFlag != null)
            {
                FlagPickup.RedFlag.ReturnFlag(null);
            }
            
            if (FlagPickup.BlueFlag != null)
            {
                FlagPickup.BlueFlag.ReturnFlag(null);
            }

            foreach (SpawnPoint spawnPoint in SpawnPoints)
            {
                spawnPoint.Used = false;
            }
            
            foreach (SoldierAgent soldier in Sorted)
            {
                soldier.Spawn();
            }
            
            foreach (HealthWeaponPickup pickup in _healthWeaponPickups)
            {
                pickup.StopAllCoroutines();
                pickup.Ready = true;
            }
        }
    }
}