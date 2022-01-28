using System.Collections.Generic;
using System.Linq;
using EasyAI.Actuators;
using EasyAI.Managers;
using EasyAI.Minds;
using EasyAI.Percepts;
using EasyAI.PerformanceMeasures;
using EasyAI.Sensors;
using UnityEngine;
using Action = EasyAI.Actions.Action;

namespace EasyAI.Agents
{
    public abstract class Agent : MonoBehaviour
    {
        [SerializeField]
        [Min(0)]
        [Tooltip("How fast this agent can move in units per second.")]
        protected float moveSpeed;
        
        [SerializeField]
        [Tooltip("How fast this agent can look in degrees per second.")]
        protected float lookSpeed;
        
        public Vector3 MoveTarget { get; private set; }

        public Vector3 LookTarget { get; private set; }

        public bool MovingToTarget { get; private set; }

        public bool LookingToTarget { get; private set; }
        
        public bool DidMove { get; protected set; }
        
        public bool DidLook { get; private set; }

        public float Performance { get; private set; }
        
        public Mind Mind { get; private set; }

        public Sensor[] Sensors { get; private set; }

        public Percept[] Percepts { get; private set; }

        public Actuator[] Actuators { get; private set; }

        public Action[] Actions { get; private set; }

        public float DeltaTime { get; private set; }

        public Vector3 Position => transform.position;

        public Quaternion Rotation => _visuals.rotation;

        public Vector3 LocalPosition => transform.localPosition;

        public Quaternion LocalRotation => _visuals.localRotation;

        private PerformanceMeasure _performanceMeasure;

        private Transform _visuals;

        public void AssignMind(Mind mind)
        {
            Mind = mind;
            ConfigureMind();
        }

        public void AssignPerformanceMeasure(PerformanceMeasure performanceMeasure)
        {
            _performanceMeasure = performanceMeasure;
            ConfigurePerformanceMeasure();
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
            LookingToTarget = LookTarget != transform.position;
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
            LookingToTarget = false;
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

        public void Perform()
        {
            if (Mind != null)
            {
                if (Sense())
                {
                    Action[] decisions = Mind.Think(Percepts);
                
                    if (decisions == null)
                    {
                        Actions = null;
                    }
                    else
                    {
                        List<Action> updated = decisions.Where(a => a != null).ToList();

                        if (Actions != null)
                        {
                            foreach (Action action in Actions)
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
            
                        Actions = updated.ToArray();
                    }
                }

                Act();
            }

            if (_performanceMeasure != null)
            {
                Performance = _performanceMeasure.GetPerformance();
            }
            
            DeltaTime = 0;
        }

        public void AddMessage(string message)
        {
            if (Mind == null)
            {
                return;
            }
            
            Mind.AddMessage(message);
        }

        protected virtual void Start()
        {
            if (Mind == null)
            {
                Mind = GetComponent<Mind>();
                if (Mind == null)
                {
                    Mind = GetComponentInChildren<Mind>();
                    if (Mind == null)
                    {
                        Mind = FindObjectsOfType<Mind>().FirstOrDefault(m => m.Agent == null);
                    }
                }
            }

            ConfigureMind();
            
            if (_performanceMeasure == null)
            {
                _performanceMeasure = GetComponent<PerformanceMeasure>();
                if (_performanceMeasure == null)
                {
                    _performanceMeasure = GetComponentInChildren<PerformanceMeasure>();
                    if (_performanceMeasure == null)
                    {
                        _performanceMeasure = FindObjectOfType<PerformanceMeasure>();
                    }
                }
            }

            ConfigurePerformanceMeasure();

            List<Actuator> actuators = GetComponents<Actuator>().ToList();
            actuators.AddRange(GetComponentsInChildren<Actuator>());
            Actuators = actuators.Distinct().ToArray();
            foreach (Actuator actuator in Actuators)
            {
                actuator.Agent = this;
            }
            
            List<Sensor> sensors = GetComponents<Sensor>().ToList();
            sensors.AddRange(GetComponentsInChildren<Sensor>());
            Sensors = sensors.Distinct().ToArray();
            
            Percepts = new Percept[Sensors.Length];
            for (int i = 0; i < Sensors.Length; i++)
            {
                Sensors[i].Agent = this;
                Percepts[i] = null;
            }
            
            AgentManager.Singleton.FindAgents();

            Transform[] children = GetComponentsInChildren<Transform>();
            if (children.Length == 0)
            {
                GameObject go = new GameObject("Visuals");
                _visuals = go.transform;
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                return;
            }

            _visuals = children.FirstOrDefault(t => t.name == "Visuals");
            if (_visuals == null)
            {
                _visuals = children[0];
            }
        }
        
        protected abstract void Move();

        protected virtual void Update()
        {
            DeltaTime += Time.deltaTime;
            Look();
        }

        protected virtual void OnEnable()
        {
            try
            {
                AgentManager.Singleton.FindAgents();
            }
            catch { }
        }

        protected virtual void OnDisable()
        {
            AgentManager.Singleton.FindAgents();
        }

        protected virtual void OnDestroy()
        {
            AgentManager.Singleton.FindAgents();
        }

        private bool Sense()
        {
            bool sensed = false;
            for (int i = 0; i < Percepts.Length; i++)
            {
                Percept percept = Sensors[i].Read();
                if (percept == null)
                {
                    continue;
                }

                AddMessage($"Perceived {percept.GetType().ToString().Split('.').Last()} from sensor {Sensors[i].ToString().Split('.').Last().Replace(")", string.Empty)}.");
                Percepts[i] = percept;
                sensed = true;
            }

            if (!sensed)
            {
                AddMessage("Did not perceive anything.");
            }

            return sensed;
        }

        private void Act()
        {
            if (Actions == null || Actions.Length == 0)
            {
                AddMessage("Did not perform any actions.");
                return;
            }
            
            foreach (Actuator actuator in Actuators)
            {
                actuator.Act(Actions);
            }

            foreach (Action action in Actions)
            {
                if (action.Complete)
                {
                    AddMessage($"Completed action {action.GetType().ToString().Split('.').Last()}.");
                }
            }

            Actions = Actions.Where(a => !a.Complete).ToArray();
        }

        private void Look()
        {
            if (!LookingToTarget)
            {
                return;
            }
            
            Transform visuals = _visuals;
            Vector3 target = new Vector3(LookTarget.x, visuals.position.y, LookTarget.z);
            if (visuals.position == target)
            {
                return;
            }

            Quaternion rotation = _visuals.rotation;
            Quaternion lastRotation = rotation;
            rotation = Quaternion.LookRotation(Vector3.RotateTowards(visuals.forward, target - visuals.position, lookSpeed * Time.deltaTime, 0.0f));
            _visuals.rotation = rotation;
            DidLook = rotation != lastRotation;

            if (DidLook && Mind != null)
            {
                AddMessage($"Looked towards {LookTarget}.");
            }
        }

        private void ConfigureMind()
        {
            if (Mind != null)
            {
                Mind.Agent = this;
            }
        }
        
        private void ConfigurePerformanceMeasure()
        {
            if (_performanceMeasure != null)
            {
                _performanceMeasure.Agent = this;
            }
        }
    }
}