using UnityEngine;

namespace A3.Minds
{
    public class SteeringDemoMind : Mind
    {
        [SerializeField]
        private Transform[] targets;
        
        public override float DisplayDetails(float x, float y, float w, float h, float p)
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
            
            y = AgentManager.NextItem(y, h, p);
            if (AgentManager.GuiButton(x, y, w, h, "Origin"))
            {
                Agent.Wander = false;
                Agent.SetMoveData(Agent.MoveType.Seek, new Vector2(0, 0));
            }

            foreach (Transform target in targets)
            {
                Agent other = target.GetComponent<Agent>();
                if (other != null)
                {
                    y = AgentManager.NextItem(y, h, p);
                    if (AgentManager.GuiButton(x, y, w, h, $"Seek {target.name} and have {target.name} Flee"))
                    {
                        Agent.Wander = false;
                        Agent.SetMoveData(Agent.MoveType.Seek, target);

                        other.Wander = false;
                        other.SetMoveData(Agent.MoveType.Flee, Agent.transform);
                    }
                    
                    y = AgentManager.NextItem(y, h, p);
                    if (AgentManager.GuiButton(x, y, w, h, $"Pursue {target.name} and have {target.name} Evade"))
                    {
                        Agent.Wander = false;
                        Agent.SetMoveData(Agent.MoveType.Pursuit, target);

                        other.Wander = false;
                        other.SetMoveData(Agent.MoveType.Evade, Agent.transform);
                    }
                    
                    continue;
                }

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

            return y;
        }
    }
}
