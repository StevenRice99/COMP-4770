using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        private SoliderRole _role;

        private bool _redTeam;

        private int _health;

        private bool Alive => _role != SoliderRole.Dead;
        
        public override Action[] Think()
        {
            return null;
        }

        public void Damage(int amount)
        {
            if (_role == SoliderRole.Dead)
            {
                return;
            }
            
            _health -= amount;
            if (_health > 0)
            {
                return;
            }

            StopAllCoroutines();
            StartCoroutine(Respawn());
        }

        public void Heal(int amount)
        {
            if (_role == SoliderRole.Dead)
            {
                return;
            }

            _health += amount;
            if (_health > SoldierAgentManager.SoldierAgentManagerSingleton.health)
            {
                _health = SoldierAgentManager.SoldierAgentManagerSingleton.health;
            }
        }

        protected override void Start()
        {
            base.Start();
            
            Agent.Wander = true;
            
            _redTeam = TeamRed.Count <= TeamBlue.Count;
            if (_redTeam)
            {
                TeamRed.Add(this);
            }
            else
            {
                TeamBlue.Add(this);
            }
            
            // MOVE TO SPAWN POSITION.
            _role = GetRole();
            _health = SoldierAgentManager.SoldierAgentManagerSingleton.health;
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
            _health = SoldierAgentManager.SoldierAgentManagerSingleton.health;
            _role = GetRole();
        }

        private IEnumerable<SoldierBrain> GetTeam()
        {
            // ORDER BY DISTANCE TO ENEMY FLAG.
            return (_redTeam ? TeamRed : TeamBlue).Where(s => s.Alive);
        }
    }
}