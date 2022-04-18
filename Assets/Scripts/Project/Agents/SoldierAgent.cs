﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Managers;
using Project.Pickups;
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
            Detect();

            TargetData? target = ChooseTarget();
            if (target != null)
            {
                LookAtTarget(target.Value.Position);
                headPosition.LookAt(target.Value.Position);
                headPosition.localRotation = Quaternion.Euler(headPosition.localRotation.eulerAngles.x, 0, 0);
            }
            else
            {
                StopLookAtTarget();
            }

            Think(target);
            
            Cleanup();
            
            base.Perform();
        }
        
        private class EnemyMemory
        {
            public SoldierAgent Enemy;

            public bool HasFlag;

            public bool Visible;

            public Vector3 Position;

            public float DeltaTime;
        }

        private struct TargetData
        {
            public SoldierAgent Enemy;
            
            public Vector3 Position;
        }
        
        private static readonly List<SoldierAgent> TeamRed = new();
        
        private static readonly List<SoldierAgent> TeamBlue = new();

        public Transform headPosition;

        public Transform flagPosition;

        [SerializeField]
        private MeshRenderer[] colorVisuals;

        [SerializeField]
        private MeshRenderer[] otherVisuals;

        public bool RedTeam { get; private set; }

        public int Health { get; private set; }
        
        public Weapon[] Weapons { get; private set; }
        
        public int WeaponIndex { get; private set; }

        public bool Alive => _role != SoliderRole.Dead;
        
        private SoliderRole _role;

        private Collider[] _colliders;

        private readonly List<EnemyMemory> _enemiesDetected = new();

        private bool CarryingFlag => RedTeam ? FlagPickup.RedFlag != null && FlagPickup.RedFlag.carryingPlayer == this : FlagPickup.BlueFlag != null && FlagPickup.BlueFlag.carryingPlayer == this;

        private bool EnemyHasFlag => RedTeam ? FlagPickup.RedFlag != null && FlagPickup.RedFlag.carryingPlayer != null : FlagPickup.BlueFlag != null && FlagPickup.BlueFlag.carryingPlayer != null;

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
            
            EnemyMemory memory = _enemiesDetected.FirstOrDefault(e => e.Enemy == enemy);
            if (memory != null)
            {
                memory.DeltaTime = 0;
                memory.Position = enemy.headPosition.position;
                memory.Visible = false;
                memory.HasFlag = false;
                return;
            }
            
            _enemiesDetected.Add(new EnemyMemory
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

        private void Think(TargetData? targetData)
        {
            // ADD THINKING LOGIC.
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
            Transform tr = transform;
            tr.position = spawnTr.position;
            tr.rotation = spawnTr.rotation;
            
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
            Weapons[WeaponIndex].Shoot();
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

        private IEnumerable<SoldierAgent> SeeEnemies()
        {
            return GetEnemies().Where(enemy => !Physics.Linecast(headPosition.position, enemy.headPosition.position, AgentManager.Singleton.obstacleLayers)).ToArray();
        }

        private void Detect()
        {
            foreach (SoldierAgent enemy in SeeEnemies())
            {
                EnemyMemory memory = _enemiesDetected.FirstOrDefault(e => e.Enemy == enemy);
                if (memory != null)
                {
                    memory.DeltaTime = 0;
                    memory.Position = enemy.headPosition.position;
                    memory.Visible = true;
                    memory.HasFlag = FlagPickup.RedFlag != null && FlagPickup.RedFlag.carryingPlayer == enemy || FlagPickup.BlueFlag != null && FlagPickup.BlueFlag.carryingPlayer == enemy;
                    continue;
                }
                
                _enemiesDetected.Add(new EnemyMemory
                {
                    DeltaTime = 0,
                    Position = enemy.headPosition.position,
                    Visible = true,
                    HasFlag = FlagPickup.RedFlag != null && FlagPickup.RedFlag.carryingPlayer == enemy || FlagPickup.BlueFlag != null && FlagPickup.BlueFlag.carryingPlayer == enemy
                });
            }
        }

        private void Cleanup()
        {
            for (int i = 0; i < _enemiesDetected.Count; i++)
            {
                _enemiesDetected[i].DeltaTime += DeltaTime;
                if (_enemiesDetected[i].DeltaTime > SoldierAgentManager.SoldierAgentManagerSingleton.memoryTime)
                {
                    _enemiesDetected.RemoveAt(i--);
                }
            }
        }

        private TargetData? ChooseTarget()
        {
            if (_enemiesDetected.Count == 0)
            {
                return null;
            }
            
            EnemyMemory target = _enemiesDetected.OrderBy(e => e.HasFlag).ThenBy(e => e.Visible).ThenBy(e => e.DeltaTime).ThenBy(e => Vector3.Distance(transform.position, e.Position)).First();
            
            return new TargetData
            {
                Enemy = target.Enemy,
                Position = target.Position
            };
        }
    }
}