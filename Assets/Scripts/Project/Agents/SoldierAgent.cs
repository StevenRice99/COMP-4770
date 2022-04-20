﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Managers;
using Project.Pickups;
using Project.Positions;
using Project.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Agents
{
    public class SoldierAgent : CharacterAgent
    {
        private enum SoliderRole : byte
        {
            Dead = 0,
            Collector = 1,
            Attacker = 2,
            Defender = 3
        }
        private enum WeaponChoices
        {
            MachineGun = 0,
            Shotgun = 1,
            Sniper = 2,
            RocketLauncher = 3,
            Pistol = 4
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

            public bool Visible;
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

        public int Health { get; set; }
        
        public int WeaponIndex { get; set; }
        
        public TargetData? Target { get; set; }
        
        public int Kills { get; set; }
        
        public int Deaths { get; set; }
        
        public int Captures { get; set; }
        
        public int Returns { get; set; }

        public bool RedTeam { get; private set; }
        
        public Weapon[] Weapons { get; private set; }

        public bool Alive => _role != SoliderRole.Dead;
        
        private SoliderRole _role;

        public Collider[] Colliders { get; private set; }

        private bool _findNewPoint = true;

        private Coroutine _pointDelay;

        public readonly List<EnemyMemory> EnemiesDetected = new();

        private int[] _weaponPriority = new int[(int) WeaponChoices.Pistol];

        private bool CarryingFlag => RedTeam ? FlagPickup.BlueFlag != null && FlagPickup.BlueFlag.carryingPlayer == this : FlagPickup.RedFlag != null && FlagPickup.RedFlag.carryingPlayer == this;

        private bool FlagAtBase => RedTeam ? FlagPickup.RedFlag != null && FlagPickup.RedFlag.transform.position == FlagPickup.RedFlag.SpawnPosition : FlagPickup.BlueFlag != null && FlagPickup.BlueFlag.transform.position == FlagPickup.BlueFlag.SpawnPosition;

        private Vector3 EnemyFlag => RedTeam ? FlagPickup.BlueFlag != null ? FlagPickup.BlueFlag.transform.position : Vector3.zero : FlagPickup.RedFlag != null ? FlagPickup.RedFlag.transform.position : Vector3.zero;

        private Vector3 TeamFlag => RedTeam ? FlagPickup.RedFlag != null ? FlagPickup.RedFlag.transform.position : Vector3.zero : FlagPickup.BlueFlag != null ? FlagPickup.BlueFlag.transform.position : Vector3.zero;
        
        private Vector3 Base => RedTeam ? FlagPickup.RedFlag != null ? FlagPickup.RedFlag.SpawnPosition : Vector3.zero : FlagPickup.BlueFlag != null ? FlagPickup.BlueFlag.SpawnPosition : Vector3.zero;
        
        /// <summary>
        /// Override for custom detail rendering on the automatic GUI.
        /// </summary>
        /// <param name="x">X rendering position. In most cases this should remain unchanged.</param>
        /// <param name="y">Y rendering position. Update this with every component added and return it.</param>
        /// <param name="w">Width of components. In most cases this should remain unchanged.</param>
        /// <param name="h">Height of components. In most cases this should remain unchanged.</param>
        /// <param name="p">Padding of components. In most cases this should remain unchanged.</param>
        /// <returns>The updated Y position after all custom rendering has been done.</returns>
        public override float DisplayDetails(float x, float y, float w, float h, float p)
        {
            y = AgentManager.NextItem(y, h, p);
            AgentManager.GuiBox(x, y, w, h, p, 13);
            
            AgentManager.GuiLabel(x, y, w, h, p, $"Team Captures - Red: {SoldierAgentManager.SoldierAgentManagerSingleton.ScoreRed} | Blue: {SoldierAgentManager.SoldierAgentManagerSingleton.ScoreBlue}");
            y = AgentManager.NextItem(y, h, p);
            
            AgentManager.GuiLabel(x, y, w, h, p, $"Team Kills - Red: {SoldierAgentManager.SoldierAgentManagerSingleton.KillsRed} | Blue: {SoldierAgentManager.SoldierAgentManagerSingleton.KillsBlue}");
            y = AgentManager.NextItem(y, h, p);
            
            AgentManager.GuiLabel(x, y, w, h, p, "--------------------------------------------------------------------------------------------------------------------------");
            y = AgentManager.NextItem(y, h, p);

            AgentManager.GuiLabel(x, y, w, h, p, $"Soldier Performance: {SoldierAgentManager.SoldierAgentManagerSingleton.Sorted.IndexOf(this) + 1} / {SoldierAgentManager.SoldierAgentManagerSingleton.Sorted.Count}");
            y = AgentManager.NextItem(y, h, p);

            AgentManager.GuiLabel(x, y, w, h, p, _role == SoliderRole.Dead ? "Respawning" : $"Role: {_role}");
            y = AgentManager.NextItem(y, h, p);

            AgentManager.GuiLabel(x, y, w, h, p, $"Health: {Health} / {SoldierAgentManager.SoldierAgentManagerSingleton.health}");
            y = AgentManager.NextItem(y, h, p);

            AgentManager.GuiLabel(x, y, w, h, p, _role == SoliderRole.Dead ? "Weapon: None" : WeaponIndex switch
            {
                (int) WeaponChoices.MachineGun => $"Weapon: Machine Gun | Ammo: {Weapons[WeaponIndex].Ammo} / {Weapons[WeaponIndex].maxAmmo}",
                (int) WeaponChoices.Shotgun => $"Weapon: Shotgun | Ammo: {Weapons[WeaponIndex].Ammo} / {Weapons[WeaponIndex].maxAmmo}",
                (int) WeaponChoices.Sniper => $"Weapon: Sniper | Ammo: {Weapons[WeaponIndex].Ammo} / {Weapons[WeaponIndex].maxAmmo}",
                (int) WeaponChoices.RocketLauncher => $"Weapon: Rocket Launcher | Ammo: {Weapons[WeaponIndex].Ammo} / {Weapons[WeaponIndex].maxAmmo}",
                _ => "Weapon: Pistol"
            });
            y = AgentManager.NextItem(y, h, p);
            
            AgentManager.GuiLabel(x, y, w, h, p, Target == null || Target.Value.Enemy == null ? "Fighting: Nobody" : $"Fighting: {Target.Value.Enemy.name}");
            y = AgentManager.NextItem(y, h, p);

            int visible = EnemiesDetected.Count(e => e.Visible);
            AgentManager.GuiLabel(x, y, w, h, p, $"See: {visible} | Hear: {EnemiesDetected.Count - visible}");
            y = AgentManager.NextItem(y, h, p);

            AgentManager.GuiLabel(x, y, w, h, p, $"Captures: {Captures} | Most: {SoldierAgentManager.SoldierAgentManagerSingleton.MostCaptures}");
            y = AgentManager.NextItem(y, h, p);

            AgentManager.GuiLabel(x, y, w, h, p, $"Returns: {Returns} | Most: {SoldierAgentManager.SoldierAgentManagerSingleton.MostReturns}");
            y = AgentManager.NextItem(y, h, p);

            AgentManager.GuiLabel(x, y, w, h, p, $"Kills: {Kills} | Most: {SoldierAgentManager.SoldierAgentManagerSingleton.MostKills}");
            y = AgentManager.NextItem(y, h, p);
            
            AgentManager.GuiLabel(x, y, w, h, p, $"Deaths: {Deaths} | Least: {SoldierAgentManager.SoldierAgentManagerSingleton.LeastDeaths}");
            
            return y;
        }
        
        public override void Perform()
        {
            if (_role == SoliderRole.Dead)
            {
                return;
            }
            
            Target = ChooseTarget();

            PrioritizeWeapons();

            ChooseWeapon();

            ChooseDestination();
            
            Cleanup();
            
            base.Perform();
        }
        
        public override void Move()
        {
            if (CharacterController != null && CharacterController.enabled)
            {
                base.Move();
            }
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

            Health = 0;
            Deaths++;
            shotBy.Kills++;

            if (RedTeam)
            {
                SoldierAgentManager.SoldierAgentManagerSingleton.KillsRed++;
            }
            else
            {
                SoldierAgentManager.SoldierAgentManagerSingleton.KillsBlue++;
            }

            SoldierAgentManager.SoldierAgentManagerSingleton.UpdateSorted();

            StopAllCoroutines();
            StartCoroutine(Respawn());
        }

        public void Hear(SoldierAgent enemy, float distance)
        {
            if (Vector3.Distance(headPosition.position, enemy.headPosition.position) > distance)
            {
                return;
            }
            
            EnemyMemory memory = EnemiesDetected.FirstOrDefault(e => e.Enemy == enemy && !e.Visible);
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

        public void AssignRoles()
        {
            SoldierAgent[] team = GetTeam();
            for (int i = 0; i < team.Length; i++)
            {
                ClearPath();
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

        public void Spawn()
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
            _findNewPoint = true;

            foreach (Weapon weapon in Weapons)
            {
                weapon.Replenish();
            }
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

            name = (RedTeam ? "Red " : "Blue ") + (RedTeam ? TeamRed.Count : TeamBlue.Count);

            List<Collider> colliders = GetComponents<Collider>().ToList();
            colliders.AddRange(GetComponentsInChildren<Collider>());
            Colliders = colliders.Distinct().ToArray();

            foreach (MeshRenderer meshRenderer in colorVisuals)
            {
                meshRenderer.material = RedTeam ? SoldierAgentManager.SoldierAgentManagerSingleton.red : SoldierAgentManager.SoldierAgentManagerSingleton.blue;
            }
            
            Spawn();
        }

        private void ChooseDestination()
        {
            if (CarryingFlag)
            {
                Navigate(Base);
                return;
            }

            switch (_role)
            {
                case SoliderRole.Collector:
                    Navigate(EnemyFlag);
                    return;
                
                case SoliderRole.Defender when !FlagAtBase:
                    Navigate(TeamFlag);
                    _findNewPoint = true;
                    return;
                
                default:
                    if (Health <= SoldierAgentManager.SoldierAgentManagerSingleton.lowHealth)
                    {
                        Vector3? destination = SoldierAgentManager.SoldierAgentManagerSingleton.GetHealth(transform.position);
                        if (destination != null)
                        {
                            Navigate(destination.Value);
                            _findNewPoint = true;
                            return;
                        }
                    }

                    if (Target is not { Visible: true })
                    {
                        if (Health < SoldierAgentManager.SoldierAgentManagerSingleton.health)
                        {
                            Vector3? destination = SoldierAgentManager.SoldierAgentManagerSingleton.GetHealth(transform.position);
                            if (destination != null)
                            {
                                Navigate(destination.Value);
                                _findNewPoint = true;
                                return;
                            }
                        }
                        
                        foreach (int w in _weaponPriority)
                        {
                            if (Weapons[w].maxAmmo < 0 || Weapons[w].Ammo >= Weapons[w].maxAmmo)
                            {
                                continue;
                            }
                            
                            Vector3? destination = SoldierAgentManager.SoldierAgentManagerSingleton.GetWeapon(transform.position, w);
                            if (destination == null)
                            {
                                continue;
                            }

                            Navigate(destination.Value);
                            _findNewPoint = true;
                            return;
                        }
                    }

                    if (Destination != null)
                    {
                        return;
                    }

                    if (_findNewPoint || (_role == SoliderRole.Attacker && Target is { Visible: true }))
                    {
                        _findNewPoint = false;
                        Navigate(SoldierAgentManager.SoldierAgentManagerSingleton.GetPoint(RedTeam, _role == SoliderRole.Defender));
                        return;
                    }

                    _pointDelay ??= StartCoroutine(PointDelay());
                    return;
            }
        }

        private void PrioritizeWeapons()
        {
            if (Target == null)
            {
                if (_role == SoliderRole.Defender)
                {
                    _weaponPriority = new[]
                    {
                        (int) WeaponChoices.Sniper,
                        (int) WeaponChoices.RocketLauncher,
                        (int) WeaponChoices.MachineGun,
                        (int) WeaponChoices.Shotgun,
                        (int) WeaponChoices.Pistol,
                    };
                }
                else
                {
                    _weaponPriority = new[]
                    {
                        (int) WeaponChoices.Shotgun,
                        (int) WeaponChoices.MachineGun,
                        (int) WeaponChoices.RocketLauncher,
                        (int) WeaponChoices.Sniper,
                        (int) WeaponChoices.Pistol,
                    };
                }
                return;
            }

            float distance = Vector3.Distance(shootPosition.position, Target.Value.Position);
            if (distance >= SoldierAgentManager.SoldierAgentManagerSingleton.distanceFar)
            {
                if (_role == SoliderRole.Defender)
                {
                    _weaponPriority = new[]
                    {
                        (int) WeaponChoices.Sniper,
                        (int) WeaponChoices.RocketLauncher,
                        (int) WeaponChoices.MachineGun,
                        (int) WeaponChoices.Pistol,
                        (int) WeaponChoices.Shotgun
                    };
                }
                else
                {
                    _weaponPriority = new[]
                    {
                        (int) WeaponChoices.RocketLauncher,
                        (int) WeaponChoices.MachineGun,
                        (int) WeaponChoices.Sniper,
                        (int) WeaponChoices.Pistol,
                        (int) WeaponChoices.Shotgun
                    };
                }
                
                return;
            }

            if (distance <= SoldierAgentManager.SoldierAgentManagerSingleton.distanceClose)
            {
                _weaponPriority = new[]
                {
                    (int) WeaponChoices.Shotgun,
                    (int) WeaponChoices.MachineGun,
                    (int) WeaponChoices.Pistol,
                    (int) WeaponChoices.RocketLauncher,
                    (int) WeaponChoices.Sniper
                };
                
                return;
            }
            
            if (_role == SoliderRole.Defender)
            {
                _weaponPriority = new[]
                {
                    (int) WeaponChoices.MachineGun,
                    (int) WeaponChoices.RocketLauncher,
                    (int) WeaponChoices.Shotgun,
                    (int) WeaponChoices.Sniper,
                    (int) WeaponChoices.Pistol
                };
            }
            else
            {
                _weaponPriority = new[]
                {
                    (int) WeaponChoices.MachineGun,
                    (int) WeaponChoices.RocketLauncher,
                    (int) WeaponChoices.Sniper,
                    (int) WeaponChoices.Shotgun,
                    (int) WeaponChoices.Pistol
                };
            }
        }

        private void ChooseWeapon()
        {
            foreach (int w in _weaponPriority)
            {
                if (Weapons[w].Ammo <= 0 && Weapons[w].maxAmmo >= 0)
                {
                    continue;
                }

                SelectWeapon(w);
                return;
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

        private SoldierAgent[] GetTeam()
        {
            IEnumerable<SoldierAgent> team = (RedTeam ? TeamRed : TeamBlue).Where(s => s.Alive);
            if (RedTeam)
            {
                if (FlagPickup.BlueFlag != null)
                {
                    team = team.OrderBy(s => Vector3.Distance(s.transform.position, FlagPickup.BlueFlag.transform.position));
                }
            }
            else
            {
                if (FlagPickup.RedFlag != null)
                {
                    team = team.OrderBy(s => Vector3.Distance(s.transform.position, FlagPickup.RedFlag.transform.position));
                }
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

            foreach (Collider col in Colliders)
            {
                col.enabled = Alive;
            }

            WeaponVisible();
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
                Position = target.Position,
                Visible = target.Visible
            };
        }

        private IEnumerator PointDelay()
        {
            yield return new WaitForSeconds(Random.Range(0, SoldierAgentManager.SoldierAgentManagerSingleton.maxWaitTime));
            _findNewPoint = true;
            _pointDelay = null;
        }
    }
}