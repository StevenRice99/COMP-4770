using SimpleIntelligence.Actions;
using SimpleIntelligence.Actuators;
using SimpleIntelligence.Components;
using SimpleIntelligence.Percepts;
using SimpleIntelligence.PerformanceMeasures;
using SimpleIntelligence.Sensors;
using UnityEngine;

namespace SimpleIntelligence.Minds
{
    public abstract class Mind : IntelligenceComponent
    {
        public Vector3 MoveTarget => agent.MoveTarget;

        public Vector3 LookTarget => agent.LookTarget;

        public bool MovingToTarget => agent.MovingToTarget;

        public bool LookingAtTarget => agent.LookingAtTarget;

        public bool DidMove => agent.DidMove;

        public bool DidLook => agent.DidLook;

        public float Performance => agent.Performance;

        public Sensor[] Sensors => agent.Sensors;

        public Percept[] Percepts => agent.Percepts;

        public Actuator[] Actuators => agent.Actuators;

        public Action[] Actions => agent.Actions;

        public float AgentDeltaTime => agent.AgentDeltaTime;

        public Vector3 Position => agent.Position;

        public Quaternion Rotation => agent.Rotation;

        public Vector3 LocalPosition => agent.LocalPosition;

        public Quaternion LocalRotation => agent.LocalRotation;
        
        public void AssignPerformanceMeasure(PerformanceMeasure performanceMeasure)
        {
            agent.AssignPerformanceMeasure(performanceMeasure);
        }

        public void MoveToTarget()
        {
            agent.MoveToTarget();
        }

        public void MoveToTarget(Vector3 target)
        {
            agent.MoveToTarget(target);
        }

        public void MoveToTarget(Transform target)
        {
            agent.MoveToTarget(target);
        }

        public void StopMoveToTarget()
        {
            agent.StopMoveToTarget();
        }

        public void LookAtTarget()
        {
            agent.LookAtTarget();
        }

        public void LookAtTarget(Vector3 target)
        {
            agent.LookAtTarget(target);
        }

        public void LookAtTarget(Transform target)
        {
            agent.LookAtTarget(target);
        }

        public void StopLookAtTarget()
        {
            agent.StopLookAtTarget();
        }

        public void MoveToLookAtTarget()
        {
            agent.MoveToLookAtTarget();
        }

        public void MoveToLookAtTarget(Vector3 target)
        {
            agent.MoveToLookAtTarget(target);
        }

        public void MoveToLookAtTarget(Transform target)
        {
            agent.MoveToLookAtTarget(target);
        }

        public void StopMoveToLookAtTarget()
        {
            agent.StopMoveToLookAtTarget();
        }
        
        public abstract Action[] Think(Percept[] percepts);
    }
}