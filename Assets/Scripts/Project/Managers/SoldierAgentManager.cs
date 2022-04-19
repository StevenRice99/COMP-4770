using System.Linq;
using Project.Agents;
using Project.Pickups;
using UnityEngine;

namespace Project.Managers
{
    public class SoldierAgentManager : AgentManager
    {
        /// <summary>
        /// Getter to cast the AgentManager singleton into a SoldierManager.
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
        public float memoryTime = 5;

        [Range(0, 1)]
        public float volume;

        [SerializeField]
        private GameObject soldierPrefab;

        public Material red;

        public Material blue;
        
        public SpawnPoint[] SpawnPoints { get; private set; }
        
        protected override void Start()
        {
            base.Start();

            SpawnPoints = FindObjectsOfType<SpawnPoint>();

            for (int i = 0; i < soldiersPerTeam * 2; i++)
            {
                Instantiate(soldierPrefab);
            }
        }

        protected override void Update()
        {
            base.Update();

            Detect();

            foreach (Agent agent in Agents)
            {
                if (agent is not SoldierAgent soldier)
                {
                    continue;
                }

                if (soldier.Target != null)
                {
                    soldier.LookAtTarget(soldier.Target.Value.Position);
                    soldier.headPosition.LookAt(soldier.Target.Value.Position);
                    soldier.headPosition.localRotation = Quaternion.Euler(soldier.headPosition.localRotation.eulerAngles.x, 0, 0);
                    soldier.weaponPosition.LookAt(soldier.Target.Value.Position);
                    soldier.weaponPosition.localRotation = Quaternion.Euler(soldier.weaponPosition.localRotation.eulerAngles.x, 0, 0);
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
                    agent.StopLookAtTarget();
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

                    Vector3 position = enemy.headPosition.position;
                    soldier.Target = new SoldierAgent.TargetData
                    {
                        Enemy = enemy,
                        Position = position
                    };
                }
            }
        }
    }
}