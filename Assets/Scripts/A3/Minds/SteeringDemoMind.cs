using System.Linq;
using UnityEngine;

namespace A3.Minds
{
    public class SteeringDemoMind : Mind
    {
        [SerializeField]
        private Transform[] targets;

        [SerializeField]
        [Min(0)]
        private float cornerRange = 450;
        
        public override float DisplayDetails(float x, float y, float w, float h, float p)
        {
            if (AgentManager.Singleton.Agents.Count > 1)
            {
                y = AgentManager.NextItem(y, h, p);
                if (AgentManager.GuiButton(x, y, w, h, "Stop Agents"))
                {
                    foreach (Agent agent in AgentManager.Singleton.Agents)
                    {
                        agent.Wander = false;
                        agent.ClearMoveData();
                    }
                }
            }

            y = AgentManager.NextItem(y, h, p);
            if (AgentManager.GuiButton(x, y, w, h, $"Stop {Agent.name}"))
            {
                Agent.Wander = false;
                Agent.ClearMoveData();
            }
            
            y = AgentManager.NextItem(y, h, p);
            if (AgentManager.GuiButton(x, y, w, h, "Wander"))
            {
                Agent.Wander = true;
                Agent.ClearMoveData();
            }

            foreach (Agent other in AgentManager.Singleton.Agents.Where(other => other != Agent))
            {
                y = AgentManager.NextItem(y, h, p);
                if (AgentManager.GuiButton(x, y, w, h, $"Seek {other.name} and have {other.name} Flee"))
                {
                    Agent.Wander = false;
                    Agent.SetMoveData(Agent.MoveType.Seek, other.transform);

                    other.Wander = false;
                    other.SetMoveData(Agent.MoveType.Flee, Agent.transform);
                }
                    
                y = AgentManager.NextItem(y, h, p);
                if (AgentManager.GuiButton(x, y, w, h, $"Pursue {other.name} and have {other.name} Evade"))
                {
                    Agent.Wander = false;
                    Agent.SetMoveData(Agent.MoveType.Pursuit, other.transform);

                    other.Wander = false;
                    other.SetMoveData(Agent.MoveType.Evade, Agent.transform);
                }
            }

            foreach (Transform target in targets)
            {
                y = AgentManager.NextItem(y, h, p);
                if (AgentManager.GuiButton(x, y, w, h, $"Seek {target.name}"))
                {
                    Agent.Wander = false;
                    Agent.SetMoveData(Agent.MoveType.Seek, target);
                }
                    
                y = AgentManager.NextItem(y, h, p);
                if (AgentManager.GuiButton(x, y, w, h, $"Pursue {target.name}"))
                {
                    Agent.Wander = false;
                    Agent.SetMoveData(Agent.MoveType.Pursuit, target);
                }
                
                y = AgentManager.NextItem(y, h, p);
                if (AgentManager.GuiButton(x, y, w, h, $"Flee {target.name}"))
                {
                    Agent.Wander = false;
                    Agent.SetMoveData(Agent.MoveType.Flee, target);
                }
                
                y = AgentManager.NextItem(y, h, p);
                if (AgentManager.GuiButton(x, y, w, h, $"Evade {target.name}"))
                {
                    Agent.Wander = false;
                    Agent.SetMoveData(Agent.MoveType.Evade, target);
                }
            }
            
            y = AgentManager.NextItem(y, h, p);
            if (AgentManager.GuiButton(x, y, w, h, "Seek (0, 0)"))
            {
                Agent.Wander = false;
                Agent.SetMoveData(Agent.MoveType.Seek, new Vector2(0, 0));
            }
            
            y = AgentManager.NextItem(y, h, p);
            if (AgentManager.GuiButton(x, y, w, h, $"Seek to ({cornerRange}, {cornerRange})"))
            {
                Agent.Wander = false;
                Agent.SetMoveData(Agent.MoveType.Seek, new Vector2(cornerRange, cornerRange));
            }
            
            y = AgentManager.NextItem(y, h, p);
            if (AgentManager.GuiButton(x, y, w, h, $"Seek to ({cornerRange}, -{cornerRange})"))
            {
                Agent.Wander = false;
                Agent.SetMoveData(Agent.MoveType.Seek, new Vector2(cornerRange, -cornerRange));
            }
            
            y = AgentManager.NextItem(y, h, p);
            if (AgentManager.GuiButton(x, y, w, h, $"Seek to (-{cornerRange}, -{cornerRange})"))
            {
                Agent.Wander = false;
                Agent.SetMoveData(Agent.MoveType.Seek, new Vector2(-cornerRange, -cornerRange));
            }
            
            y = AgentManager.NextItem(y, h, p);
            if (AgentManager.GuiButton(x, y, w, h, $"Seek to (-{cornerRange}, {cornerRange})"))
            {
                Agent.Wander = false;
                Agent.SetMoveData(Agent.MoveType.Seek, new Vector2(-cornerRange, cornerRange));
            }

            return y;
        }
    }
}
