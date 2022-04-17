using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Actuators.Weapons;
using Project.Managers;
using Project.Pickups;
using UnityEngine;

namespace Project.Minds
{
    public class SoldierBrain : Mind
    {
        private static readonly List<SoldierBrain> TeamRed = new();
        
        private static readonly List<SoldierBrain> TeamBlue = new();

        private enum SoliderRole : byte
        {
            Dead,
            Collector,
            Attacker,
            Defender
        }

        public Transform shootPosition;

        public Transform flagPosition;

        public bool RedTeam { get; private set; }

        public int Health { get; private set; }
        
        public Weapon[] Weapons { get; private set; }
        
        public int WeaponIndex { get; private set; }
        
        private SoliderRole _role;

        public bool Alive => _role != SoliderRole.Dead;

        [SerializeField]
        private MeshRenderer[] colorVisuals;

        [SerializeField]
        private MeshRenderer[] otherVisuals;
        
        public override Action[] Think()
        {
            return null;
        }

        public void Damage(int amount, SoldierBrain shotBy)
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

        public void Heal()
        {
            if (_role == SoliderRole.Dead)
            {
                return;
            }

            Health = SoldierAgentManager.SoldierAgentManagerSingleton.health;
        }

        protected override void Start()
        {
            base.Start();

            Weapons = GetComponentsInChildren<Weapon>();
            for (int i = 0; i < Weapons.Length; i++)
            {
                Weapons[i].SoldierBrain = this;
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

            foreach (MeshRenderer meshRenderer in colorVisuals)
            {
                meshRenderer.material = RedTeam ? SoldierAgentManager.SoldierAgentManagerSingleton.red : SoldierAgentManager.SoldierAgentManagerSingleton.blue;
            }
            
            // MOVE TO SPAWN POSITION.
            
            AssignRoles();
            Health = SoldierAgentManager.SoldierAgentManagerSingleton.health;
        }

        private void AssignRoles()
        {
            SoldierBrain[] team = GetTeam();
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
            ToggleMeshes();
            AssignRoles();
            
            yield return new WaitForSeconds(SoldierAgentManager.SoldierAgentManagerSingleton.respawn);
            
            // MOVE TO SPAWN POSITION.
            
            ToggleMeshes();
            Health = SoldierAgentManager.SoldierAgentManagerSingleton.health;
            _role = SoliderRole.Collector;
            AssignRoles();
        }

        private SoldierBrain[] GetTeam()
        {
            IEnumerable<SoldierBrain> team = (RedTeam ? TeamRed : TeamBlue).Where(s => s.Alive);
            if (FlagPickup.RedFlag != null && FlagPickup.BlueFlag != null)
            {
                team = RedTeam ? team.OrderBy(s => Vector3.Distance(s.transform.position, FlagPickup.BlueFlag.transform.position)) : team.OrderBy(s => Vector3.Distance(s.transform.position, FlagPickup.RedFlag.transform.position));
            }
            
            return team.ToArray();
        }

        private void ToggleMeshes()
        {
            foreach (MeshRenderer meshRenderer in colorVisuals)
            {
                meshRenderer.enabled = Alive;
            }
            
            foreach (MeshRenderer meshRenderer in otherVisuals)
            {
                meshRenderer.enabled = Alive;
            }
        }
    }
}