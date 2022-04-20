using Project.Agents;
using Project.Managers;
using UnityEngine;

namespace Project.Pickups
{
    public class FlagPickup : PickupBase
    {
        private const float CaptureDistance = 1f;
        
        public static FlagPickup BlueFlag;

        public static FlagPickup RedFlag;

        public SoldierAgent carryingPlayer;

        [SerializeField]
        [Tooltip("If this flag is for the red team or not.")]
        private bool redFlag;

        [SerializeField]
        [Tooltip("The raycast for a dropped flag to hit the ground.")]
        private LayerMask raycastMask;

        public Vector3 SpawnPosition { get; private set; }

        private Quaternion _spawnRotation;

        private Coroutine _captureDelay;

        public void ReturnFlag(SoldierAgent soldier)
        {
            Transform tr = transform;
            if (tr.position == SpawnPosition)
            {
                return;
            }

            if (soldier != null)
            {
                soldier.Returns++;
                SoldierAgentManager.SoldierAgentManagerSingleton.UpdateSorted();
            }

            UnlinkFlag();
            tr.position = SpawnPosition;
            tr.rotation = _spawnRotation;
        }

        protected override void OnPickedUp(SoldierAgent soldier, int[] ammo)
        {
            if (carryingPlayer != null)
            {
                return;
            }

            if (SameTeam(soldier))
            {
                ReturnFlag(soldier);
                return;
            }

            PickupFlag(soldier);
        }

        private void PickupFlag(SoldierAgent soldier)
        {
            carryingPlayer = soldier;
            Transform tr = transform;
            tr.parent = soldier.flagPosition;
            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.identity;;
        }
        
        private void DropFlag()
        {
            UnlinkFlag();
            
            Transform tr = transform;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hit, Mathf.Infinity, raycastMask))
            {
                Vector3 position = tr.position;
                tr.position = new Vector3(position.x, hit.point.y, position.z);
                return;
            }

            tr.position = SpawnPosition;
            tr.rotation = _spawnRotation;
        }

        private void UnlinkFlag()
        {
            carryingPlayer = null;
            transform.parent = null;
        }

        private void CaptureFlag()
        {
            if (redFlag)
            {
                SoldierAgentManager.SoldierAgentManagerSingleton.ScoreBlue++;
            }
            else
            {
                SoldierAgentManager.SoldierAgentManagerSingleton.ScoreRed++;
            }

            carryingPlayer.Captures++;

            SoldierAgent soldier = carryingPlayer;
            ReturnFlag(null);
            soldier.AssignRoles();
            
            SoldierAgentManager.SoldierAgentManagerSingleton.UpdateSorted();
        }

        private bool SameTeam(SoldierAgent soldier)
        {
            return soldier.RedTeam && this == RedFlag || !soldier.RedTeam && this == BlueFlag;
        }

        private void Awake()
        {
            if (redFlag && RedFlag != null || !redFlag && BlueFlag != null)
            {
                Destroy(gameObject);
                return;
            }

            Transform tr = transform;
            SpawnPosition = tr.position;
            _spawnRotation = tr.rotation;

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
            if (carryingPlayer == null)
            {
                return;
            }

            if (!carryingPlayer.Alive)
            {
                DropFlag();
                return;
            }

            FlagPickup otherFlag = redFlag ? BlueFlag : RedFlag;
            if (Vector3.Distance(carryingPlayer.transform.position, otherFlag.SpawnPosition) > CaptureDistance)
            {
                return;
            }

            CaptureFlag();
        }
    }
}