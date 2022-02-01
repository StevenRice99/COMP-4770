﻿using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Base actuator class for performing actions requested by the agent.
/// </summary>
public abstract class Actuator : IntelligenceComponent
{
    /// <summary>
    /// Attempt to act on each type of action from the agent.
    /// </summary>
    /// <param name="actions">The actions the agent wants to perform.</param>
    public void Act(IEnumerable<Action> actions)
    {
        int actionsComplete = 0;
            
        // Ensure only actions which are not yet complete (by another actuator) are attempted.
        foreach (Action action in actions.Where(a => !a.Complete))
        {
            action.Complete = Act(action);
            if (!action.Complete)
            {
                continue;
            }

            actionsComplete++;
            AddMessage($"Finished action {action}.");
        }

        if (actionsComplete == 0)
        {
            AddMessage("Finished no actions.");
        }
        else if (actionsComplete > 1)
        {
            AddMessage($"Finished {actionsComplete} actions.");
        }
    }
        
    /// <summary>
    /// Implement how the actuator will act to any given action, if at all.
    /// </summary>
    /// <param name="action">The action the agent wants to perform.</param>
    /// <returns>True if the action has been completed, false if it has not been acted upon or it is not yet complete.</returns>
    protected abstract bool Act(Action action);
}