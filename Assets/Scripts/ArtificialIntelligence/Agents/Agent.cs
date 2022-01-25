using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArtificialIntelligence.Agents
{
    public abstract class Agent : MonoBehaviour
    {
        [SerializeField]
        [Min(0)]
        private float tickRate;
        
        [SerializeField]
        protected float moveSpeed;
        
        [SerializeField]
        protected float lookSpeed;

        [SerializeField]
        private Transform visuals;

        public Vector3 destination;

        public Vector3 target;

        private Sensor[] _sensors;

        private Percept[] _percepts;

        private Actuator[] _actuators;

        private Action[] _actions;

        private float _tick;

        protected virtual void Start()
        {
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
                Action[] decisions = Think(_percepts);

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

        protected abstract Action[] Think(Percept[] percepts);

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
            Transform t = visuals;
            Vector3 target = new Vector3(this.target.x, t.position.y, this.target.z);
            if (t.position == target)
            {
                return;
            }
            
            visuals.rotation = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(t.position - target), lookSpeed * Time.deltaTime);
        }
    }
}