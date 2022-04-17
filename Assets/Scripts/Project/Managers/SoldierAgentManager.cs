﻿using UnityEngine;

namespace Project.Managers
{
    public class SoldierAgentManager : AgentManager
    {
        /// <summary>
        /// Getter to cast the AgentManager singleton into a SoldierManager.
        /// </summary>
        public static SoldierAgentManager SoldierAgentManagerSingleton => Singleton as SoldierAgentManager;

        [SerializeField]
        [Min(1)]
        private int soldiersPerTeam = 1;

        [Min(1)]
        public int health = 100;

        [Min(0)]
        public float respawn = 10;

        [Range(0, 1)]
        public float sound;

        [SerializeField]
        private GameObject soldierPrefab;

        public Material red;

        public Material blue;
        
        protected override void Start()
        {
            base.Start();

            for (int i = 0; i < soldiersPerTeam * 2; i++)
            {
                Instantiate(soldierPrefab);
            }
        }
    }
}