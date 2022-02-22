using UnityEngine;

public class State : ScriptableObject
{
    public virtual void Enter(Agent agent)
    {
        StandardMessage(agent, "entered");
    }

    public virtual void Execute(Agent agent)
    {
        StandardMessage(agent, "executed");
    }

    public virtual void Exit(Agent agent)
    {
        StandardMessage(agent, "exited");
    }

    public virtual bool HandleEvent(Agent agent, AIEvent aiEvent)
    {
        return false;
    }

    private void StandardMessage(Agent agent, string action)
    {
        agent.AddMessage($"{agent} {action} {GetType().Name}.");
    }

    /// <summary>
    /// Override to easily display the type of the component for easy usage in messages.
    /// </summary>
    /// <returns>Name of this type.</returns>
    public override string ToString()
    {
        return GetType().Name;
    }
}