using UnityEngine;

/// <summary>
/// The mind which will decide on what actions an agent's actuators will perform based on the percepts it sensed.
/// </summary>
public abstract class Mind : IntelligenceComponent
{
    /// <summary>
    /// Implement to decide what actions the agent's actuators will perform based on the percepts it sensed.
    /// </summary>
    /// <param name="percepts">The percepts which the agent's sensors sensed.</param>
    /// <returns>The actions the agent's actuators will perform.</returns>
    public abstract Action[] Think(Percept[] percepts);
    
    public void AssignMoveSpeed(float speed)
    {
       Agent.AssignMoveSpeed(speed);
    }

    public void AssignLookSpeed(float speed)
    {
        Agent.AssignLookSpeed(speed);
    }
        
    /// <summary>
    /// Assign a performance measure to this agent.
    /// </summary>
    /// <param name="performanceMeasure">The performance measure to assign.</param>
    public void AssignPerformanceMeasure(PerformanceMeasure performanceMeasure)
    {
        Agent.AssignPerformanceMeasure(performanceMeasure);
    }

    /// <summary>
    /// Resume movement towards the move target currently assigned to the agent.
    /// </summary>
    public void MoveToTarget()
    {
        Agent.MoveToTarget();
    }

    /// <summary>
    /// Set a target position for the agent to move towards.
    /// </summary>
    /// <param name="target">The target position to move to.</param>
    public void MoveToTarget(Vector3 target)
    {
        Agent.MoveToTarget(target);
    }

    /// <summary>
    /// Set a target transform for the agent to move towards.
    /// </summary>
    /// <param name="target">The target transform to move to.</param>
    public void MoveToTarget(Transform target)
    {
        Agent.MoveToTarget(target);
    }

    /// <summary>
    /// Have the agent stop moving towards its move target.
    /// </summary>
    public void StopMoveToTarget()
    {
        Agent.StopMoveToTarget();
    }

    /// <summary>
    /// Resume looking towards the look target currently assigned to the agent.
    /// </summary>
    public void LookAtTarget()
    {
        Agent.LookAtTarget();
    }

    /// <summary>
    /// Set a target position for the agent to look towards.
    /// </summary>
    /// <param name="target">The target position to look to.</param>
    public void LookAtTarget(Vector3 target)
    {
        Agent.LookAtTarget(target);
    }

    /// <summary>
    /// Set a target transform for the agent to look towards.
    /// </summary>
    /// <param name="target">The target transform to look to.</param>
    public void LookAtTarget(Transform target)
    {
        Agent.LookAtTarget(target);
    }

    /// <summary>
    /// Have the agent stop looking towards its look target.
    /// </summary>
    public void StopLookAtTarget()
    {
        Agent.StopLookAtTarget();
    }

    /// <summary>
    /// Resume moving towards the move target currently assigned and looking towards the look target currently assigned to the agent.
    /// </summary>
    public void MoveToLookAtTarget()
    {
        Agent.MoveToLookAtTarget();
    }

    /// <summary>
    /// Set a target position for the agent to move and look towards.
    /// </summary>
    /// <param name="target">The target position to move and look to.</param>
    public void MoveToLookAtTarget(Vector3 target)
    {
        Agent.MoveToLookAtTarget(target);
    }

    /// <summary>
    /// Set a target transform for the agent to move and look towards.
    /// </summary>
    /// <param name="target">The target transform to move and look to.</param>
    public void MoveToLookAtTarget(Transform target)
    {
        Agent.MoveToLookAtTarget(target);
    }

    /// <summary>
    /// Have the agent stop moving towards its move target and looking towards its look target.
    /// </summary>
    public void StopMoveToLookAtTarget()
    {
        Agent.StopMoveToLookAtTarget();
    }
        
    /// <summary>
    /// Instantly stop all actions this agent is performing.
    /// </summary>
    public void StopAllActions()
    {
        Agent.StopAllActions();
    }

    /// <summary>
    /// Add a message to this component.
    /// </summary>
    /// <param name="message">The message to add.</param>
    public override void AddMessage(string message)
    {
        Agent.AddMessage(message);
    }
}