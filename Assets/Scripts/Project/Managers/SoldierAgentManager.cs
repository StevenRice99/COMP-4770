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
    }
}