using Project.Agents;
using Project.Managers;
using UnityEngine;

namespace Project.Pickups
{
    /// <summary>
    /// Pickups for the flags for soldiers to carry.
    /// </summary>
    public class FlagPickup : PickupBase
    {
        /// <summary>
        /// How close to a base to be considered captured.
        /// </summary>
        private const float CaptureDistance = 1f;
        
        /// <summary>
        /// The blue flag.
        /// </summary>
        public static FlagPickup BlueFlag;

        /// <summary>
        /// The red flag.
        /// </summary>
        public static FlagPickup RedFlag;

        /// <summary>
        /// The player carrying this flag.
        /// </summary>
        public SoldierAgent carryingPlayer;

        [SerializeField]
        [Tooltip("If this flag is for the red team or not.")]
        private bool redFlag;

        [SerializeField]
        [Tooltip("The raycast for a dropped flag to hit the ground.")]
        private LayerMask raycastMask;

        /// <summary>
        /// The position the flag starts at.
        /// </summary>
        public Vector3 SpawnPosition { get; private set; }

        /// <summary>
        /// The rotation the flag starts at.
        /// </summary>
        private Quaternion _spawnRotation;

        /// <summary>
        /// Return the flag back to the base.
        /// </summary>
        /// <param name="soldier">The soldier who returned the flag.</param>
        public void ReturnFlag(SoldierAgent soldier)
        {
            // No need to return if already at the base.
            Transform tr = transform;
            if (tr.position == SpawnPosition)
            {
                return;
            }

            // If returned by a player, increase its score.
            if (soldier != null)
            {
                soldier.Returns++;
                soldier.AddMessage("Returned the flag.");
                SoldierAgentManager.SoldierAgentManagerSingleton.UpdateSorted();
            }

            // Reset the flag.
            UnlinkFlag();
            tr.position = SpawnPosition;
            tr.rotation = _spawnRotation;
        }

        /// <summary>
        /// Implement to pickup the flag.
        /// </summary>
        /// <param name="soldier">The soldier.</param>
        /// <param name="ammo">Not used.</param>
        protected override void OnPickedUp(SoldierAgent soldier, int[] ammo)
        {
            // If already being carried, do nothing.
            if (carryingPlayer != null)
            {
                return;
            }

            // If collected by a player of its team, return it to the base.
            if (SameTeam(soldier))
            {
                ReturnFlag(soldier);
                return;
            }

            // Otherwise, pickup the flag.
            PickupFlag(soldier);
        }

        /// <summary>
        /// Pickup the flag.
        /// </summary>
        /// <param name="soldier">The soldier who picked it up.</param>
        private void PickupFlag(SoldierAgent soldier)
        {
            // Set carrying.
            carryingPlayer = soldier;
            carryingPlayer.AddMessage("Picked up the flag.");
            
            // Attach to the soldier.
            Transform tr = transform;
            tr.parent = soldier.flagPosition;
            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.identity;
        }
        
        /// <summary>
        /// Drop the flag.
        /// </summary>
        private void DropFlag()
        {
            // Detach from the carrying soldier.
            UnlinkFlag();
            
            Transform tr = transform;

            // Snap to the ground below.
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hit, Mathf.Infinity, raycastMask))
            {
                Vector3 position = tr.position;
                tr.position = new Vector3(position.x, hit.point.y, position.z);
                return;
            }

            // If no ground can be detected, stay in place.
            tr.position = SpawnPosition;
            tr.rotation = _spawnRotation;
        }

        /// <summary>
        /// Detach from the carrying player.
        /// </summary>
        private void UnlinkFlag()
        {
            carryingPlayer = null;
            transform.parent = null;
        }

        /// <summary>
        /// Capture the flag.
        /// </summary>
        private void CaptureFlag()
        {
            // Add the point to the given team.
            if (redFlag)
            {
                SoldierAgentManager.SoldierAgentManagerSingleton.ScoreBlue++;
            }
            else
            {
                SoldierAgentManager.SoldierAgentManagerSingleton.ScoreRed++;
            }

            // Add the capture to the player.
            carryingPlayer.AddMessage("Captured the flag.");
            carryingPlayer.Captures++;

            // Finally return the flag and reassign roles.
            SoldierAgent soldier = carryingPlayer;
            ReturnFlag(null);
            soldier.AssignRoles();
            
            SoldierAgentManager.SoldierAgentManagerSingleton.UpdateSorted();
        }

        /// <summary>
        /// Check if a player is on the same team as the flag is for.
        /// </summary>
        /// <param name="soldier">The soldier to check.</param>
        /// <returns>True if the soldier is on the flag's team, false otherwise.</returns>
        private bool SameTeam(SoldierAgent soldier)
        {
            return soldier.RedTeam && this == RedFlag || !soldier.RedTeam && this == BlueFlag;
        }

        private void Awake()
        {
            // Ensure only one flag for each team exists.
            if (redFlag && RedFlag != null || !redFlag && BlueFlag != null)
            {
                Destroy(gameObject);
                return;
            }

            // Store the spawn position to return the flag to.
            Transform tr = transform;
            SpawnPosition = tr.position;
            _spawnRotation = tr.rotation;

            // Assign what flag this is.
            if (redFlag)
            {
                RedFlag = this;
            }
            else
            {
                BlueFlag = this;
            }
        }
        
        private void Update()
        {
            // Nothing to do if not being carried.
            if (carryingPlayer == null)
            {
                return;
            }

            // If the carrying player is dead, drop the flag.
            if (!carryingPlayer.Alive)
            {
                DropFlag();
                return;
            }

            // If in range to capture the flag, capture it.
            if (Vector3.Distance(carryingPlayer.transform.position, (redFlag ? BlueFlag : RedFlag).SpawnPosition) <= CaptureDistance)
            {
                CaptureFlag();
            }
        }
    }
}