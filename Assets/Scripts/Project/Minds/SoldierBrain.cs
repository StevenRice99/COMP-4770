using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Actuators.Weapons;
using Project.Managers;
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
            Deciding,
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
            
            Agent.Wander = true;

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
            
            // MOVE TO SPAWN POSITION.
            _role = GetRole();
            Health = SoldierAgentManager.SoldierAgentManagerSingleton.health;
        }

        private SoliderRole GetRole()
        {
            bool collector = false;
            int attackDefend = 0;
            foreach (SoldierBrain brain in GetTeam())
            {
                switch (brain._role)
                {
                    case SoliderRole.Collector:
                        collector = true;
                        continue;
                    case SoliderRole.Attacker:
                        attackDefend++;
                        continue;
                    default:
                        attackDefend--;
                        break;
                }
            }

            return !collector ? SoliderRole.Collector : attackDefend >= 0 ? SoliderRole.Defender : SoliderRole.Attacker;
        }

        private IEnumerator Respawn()
        {
            _role = SoliderRole.Dead;
            
            foreach (SoldierBrain brain in GetTeam())
            {
                brain._role = SoliderRole.Deciding;
            }
            
            foreach (SoldierBrain brain in GetTeam())
            {
                brain.GetRole();
            }
            
            yield return new WaitForSeconds(SoldierAgentManager.SoldierAgentManagerSingleton.respawn);
            
            // MOVE TO SPAWN POSITION.
            Health = SoldierAgentManager.SoldierAgentManagerSingleton.health;
            _role = GetRole();
        }

        private IEnumerable<SoldierBrain> GetTeam()
        {
            // ORDER BY DISTANCE TO ENEMY FLAG.
            return (RedTeam ? TeamRed : TeamBlue).Where(s => s.Alive);
        }
    }
}