using System.Collections.Generic;
using System.Linq;
using SimpleIntelligence.Actions;
using SimpleIntelligence.Actuators;
using SimpleIntelligence.Minds;
using SimpleIntelligence.Percepts;
using SimpleIntelligence.Sensors;
using UnityEngine;

namespace SimpleIntelligence.Agents
{
    public abstract class Agent : TimedComponent
    {
        [SerializeField]
        protected float moveSpeed;
        
        [SerializeField]
        protected float lookSpeed;

        [SerializeField]
        private Transform visuals;
        
        public Vector3 MoveTarget { get; private set; }

        public Vector3 LookTarget { get; private set; }

        public bool MovingToTarget { get; private set; }

        public bool LookingAtTarget { get; private set; }
        
        public bool DidMove { get; protected set; }
        
        public bool DidLook { get; private set; }

        public float AgentElapsedTime => ElapsedTime;
        
        private Mind _mind;

        private Sensor[] _sensors;

        private Percept[] _percepts;

        private Actuator[] _actuators;

        private Action[] _actions;

        public void AssignMind(Mind mind)
        {
            _mind = mind;
            ConfigureMind();
        }

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
            if (_mind == null)
            {
                _mind = GetComponent<Mind>();
                if (_mind == null)
                {
                    _mind = GetComponentInChildren<Mind>();
                    if (_mind == null)
                    {
                        _mind = FindObjectOfType<Mind>();
                    }
                }
            }

            ConfigureMind();

            List<Actuator> actuators = GetComponents<Actuator>().ToList();
            actuators.AddRange(GetComponentsInChildren<Actuator>());
            _actuators = actuators.Distinct().ToArray();
            foreach (Actuator actuator in _actuators)
            {
                actuator.Agent = this;
            }
            
            List<Sensor> sensors = GetComponents<Sensor>().ToList();
            sensors.AddRange(GetComponentsInChildren<Sensor>());
            _sensors = sensors.Distinct().ToArray();
            
            _percepts = new Percept[_sensors.Length];
            for (int i = 0; i < _sensors.Length; i++)
            {
                _sensors[i].Agent = this;
                _percepts[i] = null;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (_mind != null)
            {
                if (Sense())
                {
                    Action[] decisions = _mind.Think(_percepts);
                    ElapsedTime = 0;
                
                    if (decisions == null)
                    {
                        _actions = null;
                    }
                    else
                    {
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
                    }
                }

                if (_actions != null)
                {
                    Act();
                }
            }

            Look();
        }

        private bool Sense()
        {
            bool sensed = false;
            for (int i = 0; i < _percepts.Length; i++)
            {
                Percept percept = _sensors[i].Read();
                if (percept == null)
                {
                    continue;
                }

                _percepts[i] = percept;
                sensed = true;
            }

            return sensed;
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

            Quaternion rotation = visuals.rotation;
            Quaternion lastRotation = rotation;
            rotation = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(t.position - goal), lookSpeed * Time.deltaTime);
            visuals.rotation = rotation;
            DidLook = rotation != lastRotation;
        }

        private void ConfigureMind()
        {
            if (_mind != null)
            {
                _mind.Agent = this;
            }
        }
    }
}