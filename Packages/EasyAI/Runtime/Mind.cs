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

    /// <summary>
    /// Add a message to this component.
    /// </summary>
    /// <param name="message">The message to add.</param>
    public override void AddMessage(string message)
    {
        Agent.AddMessage(message);
    }
}