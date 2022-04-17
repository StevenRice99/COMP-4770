using UnityEngine;

namespace Project.Managers
{
    public class SoldierAgentManager : AgentManager
    {
        /// <summary>
        /// Getter to cast the AgentManager singleton into a SoldierManager.
        /// </summary>
        public static SoldierAgentManager SoldierAgentManagerSingleton => Singleton as SoldierAgentManager;

        [Min(1)]
        public int health = 100;

        [Min(0)]
        public float respawn = 10;

        [Range(0, 1)]
        public float sound = 0;
    }
}