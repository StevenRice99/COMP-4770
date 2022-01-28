using EasyAI.Actions;
using EasyAI.Components;
using EasyAI.Percepts;
using EasyAI.PerformanceMeasures;
using UnityEngine;

namespace EasyAI.Minds
{
    public abstract class Mind : IntelligenceComponent
    {
        public abstract Action[] Think(Percept[] percepts);

        public override void DisplayGizmos()
        {
            if (MovingToTarget)
            {
                GL.Color(Color.green);
                GL.Vertex(Position);
                GL.Vertex(MoveTarget);
            }

            if (LookingAtTarget && (!MovingToTarget || MoveTarget != LookTarget))
            {
                GL.Color(Color.blue);
                GL.Vertex(Position);
                GL.Vertex(LookTarget);
            }
        }
        
        public void AssignPerformanceMeasure(PerformanceMeasure performanceMeasure)
        {
            Agent.AssignPerformanceMeasure(performanceMeasure);
        }

        public void MoveToTarget()
        {
            Agent.MoveToTarget();
        }

        public void MoveToTarget(Vector3 target)
        {
            Agent.MoveToTarget(target);
        }

        public void MoveToTarget(Transform target)
        {
            Agent.MoveToTarget(target);
        }

        public void StopMoveToTarget()
        {
            Agent.StopMoveToTarget();
        }

        public void LookAtTarget()
        {
            Agent.LookAtTarget();
        }

        public void LookAtTarget(Vector3 target)
        {
            Agent.LookAtTarget(target);
        }

        public void LookAtTarget(Transform target)
        {
            Agent.LookAtTarget(target);
        }

        public void StopLookAtTarget()
        {
            Agent.StopLookAtTarget();
        }

        public void MoveToLookAtTarget()
        {
            Agent.MoveToLookAtTarget();
        }

        public void MoveToLookAtTarget(Vector3 target)
        {
            Agent.MoveToLookAtTarget(target);
        }

        public void MoveToLookAtTarget(Transform target)
        {
            Agent.MoveToLookAtTarget(target);
        }

        public void StopMoveToLookAtTarget()
        {
            Agent.StopMoveToLookAtTarget();
        }
    }
}