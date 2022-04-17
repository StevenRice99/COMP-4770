﻿using Project.Minds;
using UnityEngine;

namespace Project.Pickups
{
    public class FlagPickup : PickupBase
    {
        private const float CaptureDistance = 1f;
        
        public static FlagPickup BlueFlag;

        public static FlagPickup RedFlag;

        public SoldierBrain carryingPlayer;

        [SerializeField]
        [Tooltip("If this flag is for the red team or not.")]
        private bool redFlag;

        [SerializeField]
        [Tooltip("The raycast for a dropped flag to hit the ground.")]
        private LayerMask raycastMask;

        private Vector3 _spawnPosition;

        private Quaternion _spawnRotation;

        public int Captures { get; private set; }

        private Coroutine _captureDelay;

        protected override void OnPickedUp(SoldierBrain soldierBrain, int[] ammo)
        {
            if (carryingPlayer != null)
            {
                return;
            }

            if (SameTeam(soldierBrain))
            {
                ReturnFlag(soldierBrain);
                return;
            }

            PickupFlag(soldierBrain);
        }

        private void PickupFlag(SoldierBrain soldierBrain)
        {
            carryingPlayer = soldierBrain;
            Transform tr = transform;
            tr.parent = soldierBrain.flagPosition;
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

            tr.position = _spawnPosition;
            tr.rotation = _spawnRotation;
        }

        private void UnlinkFlag()
        {
            carryingPlayer = null;
            transform.parent = null;
        }

        private void ReturnFlag(SoldierBrain soldierBrain)
        {
            Transform tr = transform;
            if (tr.position == _spawnPosition)
            {
                return;
            }
            
            // ADD POINTS FOR RETURNING FLAG.
            
            tr.position = _spawnPosition;
            tr.rotation = _spawnRotation;
        }

        private void CaptureFlag()
        {
            Captures++;
            
            // ADD POINTS FOR CAPTURING FLAG.
            
            ReturnFlag(null);
        }

        private bool SameTeam(SoldierBrain soldierBrain)
        {
            return soldierBrain.RedTeam && this == RedFlag || !soldierBrain.RedTeam && this == BlueFlag;
        }

        private void Awake()
        {
            if (redFlag && RedFlag != null || !redFlag && BlueFlag != null)
            {
                Destroy(gameObject);
                return;
            }

            Transform tr = transform;
            _spawnPosition = tr.position;
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
            if (Vector3.Distance(carryingPlayer.transform.position, otherFlag._spawnPosition) > CaptureDistance)
            {
                return;
            }

            CaptureFlag();
        }
    }
}