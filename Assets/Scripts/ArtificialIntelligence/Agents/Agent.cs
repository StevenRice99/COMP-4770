using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArtificialIntelligence.Agents
{
    public abstract class Agent : MonoBehaviour
    {
        [SerializeField]
        private Mind mind;
        
        [SerializeField]
        [Min(0)]
        private float tickRate;
        
        [SerializeField]
        protected float moveSpeed;
        
        [SerializeField]
        protected float lookSpeed;

        [SerializeField]
        private Transform visuals;
        
        public Vector3 MoveTarget { get; protected set; }

        public Vector3 LookTarget { get; private set; }

        public bool MovingToTarget { get; private set; }

        public bool LookingAtTarget { get; private set; }

        private Sensor[] _sensors;

        private Percept[] _percepts;

        private Actuator[] _actuators;

        private Action[] _actions;

        private float _tick;

        public void MoveToTarget()
        {
            MovingToTarget = MoveTarget != transform.position;
        }

        public void MoveToTarget(Vector3 target)
        {
            MoveTarget = target;
            MoveToTarget();
        }

        public void MoveToTarget(Transform target)
        {
            if (target == null)
            {
                StopMoveToTarget();
                return;
            }

            MoveToTarget(target.position);
        }

        public void StopMoveToTarget()
        {
            MovingToTarget = false;
        }

        public void LookAtTarget()
        {
            LookingAtTarget = LookTarget != transform.position;
        }

        public void LookAtTarget(Vector3 target)
        {
            LookTarget = target;
            LookAtTarget();
        }

        public void LookAtTarget(Transform target)
        {
            if (target == null)
            {
                StopLookAtTarget();
                return;
            }
            
            LookAtTarget(target.position);
        }

        public void StopLookAtTarget()
        {
            LookingAtTarget = false;
        }

        public void MoveToLookAtTarget()
        {
            MoveToTarget();
            LookAtTarget();
        }

        public void MoveToLookAtTarget(Vector3 target)
        {
            MoveToTarget(target);
            LookAtTarget(target);
        }

        public void MoveToLookAtTarget(Transform target)
        {
            if (target == null)
            {
                StopMoveToLookAtTarget();
                return;
            }
            
            MoveToLookAtTarget(target.position);
        }

        public void StopMoveToLookAtTarget()
        {
            StopMoveToTarget();
            StopLookAtTarget();
        }

        protected virtual void Start()
        {
            if (mind == null)
            {
                mind = GetComponent<Mind>();
                if (mind == null)
                {
                    mind = GetComponentInChildren<Mind>();
                    if (mind == null)
                    {
                        mind = FindObjectOfType<Mind>();
                    }
                }
            }

            mind.Agent = this;

            List<Actuator> actuators = GetComponents<Actuator>().ToList();
            actuators.AddRange(GetComponentsInChildren<Actuator>());
            _actuators = actuators.ToArray();
            foreach (Actuator actuator in _actuators)
            {
                actuator.Agent = this;
            }
            
            List<Sensor> sensors = GetComponents<Sensor>().ToList();
            sensors.AddRange(GetComponentsInChildren<Sensor>());
            _sensors = sensors.ToArray();
            
            _percepts = new Percept[_sensors.Length];
            for (int i = 0; i < _sensors.Length; i++)
            {
                _sensors[i].Agent = this;
                _percepts[i] = null;
            }
        }

        protected virtual void Update()
        {
            _tick += Time.deltaTime;
            if (_tick >= tickRate)
            {
                _tick = 0;
            
                Sense();
                Action[] decisions = mind.Think(_percepts);

                List<Action> updated = decisions.Where(a => a != null).ToList();

                if (_actions != null)
                {
                    foreach (Action action in _actions)
                    {
                        if (action == null)
                        {
                            continue;
                        }
                    
                        if (!updated.Exists(a => a.GetType() == action.GetType()))
                        {
                            updated.Add(action);
                        }
                    }
                }

                _actions = updated.ToArray();
            
                Act();
            }

            Look();
        }

        private void Sense()
        {
            for (int i = 0; i < _percepts.Length; i++)
            {
                Percept percept = _sensors[i].Read();
                if (percept != null)
                {
                    _percepts[i] = percept;
                }
            }
        }

        private void Act()
        {
            if (_actions == null || _actions.Length == 0)
            {
                return;
            }
            
            foreach (Actuator actuator in _actuators)
            {
                actuator.Act(_actions);
            }

            _actions = _actions.Where(a => !a.Complete).ToArray();
        }
        
        protected abstract void Move();

        private void Look()
        {
            if (!LookingAtTarget)
            {
                return;
            }
            
            Transform t = visuals;
            Vector3 goal = new Vector3(this.LookTarget.x, t.position.y, this.LookTarget.z);
            if (t.position == goal)
            {
                return;
            }
            
            visuals.rotation = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(t.position - goal), lookSpeed * Time.deltaTime);
        }
    }
}