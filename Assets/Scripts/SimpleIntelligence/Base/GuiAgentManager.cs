using System.Linq;
using SimpleIntelligence.Actuators;
using SimpleIntelligence.Agents;
using SimpleIntelligence.Minds;
using SimpleIntelligence.Sensors;
using UnityEngine;

namespace SimpleIntelligence.Base
{
    public class GuiAgentManager : AgentManager
    {
        [SerializeField]
        [Min(0)]
        private float width = 300;
        
        private enum GuiState : byte
        {
            Main,
            Agent,
            Components,
            Component
        }

        private GuiState _state;

        private bool _mainOpen = true;

        private bool _camerasOpen = true;

        private Agent _selectedAgent;

        private IntelligenceComponent _selectedComponent;

        private void OnGUI()
        {
            Render(10, 10, width, 20, 5);
        }

        private void Render(float x, float y, float w, float h, float p)
        {
            RenderMain(x, y, w, h, p);
            RenderCameras(x, y, w, h, p);
        }

        private static bool GuiButton(float x, float y, float w, float h, string message)
        {
            return GUI.Button(new Rect(x, y, w, h), message);
        }

        private static void GuiLabel(float x, float y, float w, float h, string message)
        {
            GUI.Label(new Rect(x, y, w, h), message);
        }

        private float NextItem(float y, float h, float p)
        {
            return y + h + p;
        }

        private void RenderMain(float x, float y, float w, float h, float p)
        {
            if (Agents.Length < 1)
            {
                return;
            }
            
            if (GuiButton(x, y, w, h, _mainOpen ? "Close Menu" : "Open Menu"))
            {
                _mainOpen = !_mainOpen;
            }
            
            if (!_mainOpen)
            {
                return;
            }

            if (_selectedAgent == null && _state == GuiState.Agent || _selectedComponent == null && _state == GuiState.Component)
            {
                _state = GuiState.Main;
            }

            if (_state == GuiState.Main && Agents.Length == 1)
            {
                _selectedAgent = Agents[0];
                _state = GuiState.Agent;
            }

            if (_state == GuiState.Agent)
            {

                if (Agents.Length > 1)
                {
                    y = NextItem(y, h, p);
                    if (GuiButton(x, y, w, h, "Back to Overview"))
                    {
                        _state = GuiState.Main;
                    }
                    
                    return;
                }
                
                RenderAgent(x, y, w, h, p);

                return;
            }

            if (_state == GuiState.Components)
            {

                y = NextItem(y, h, p);
                if (GuiButton(x, y, w, h, $"Back to {_selectedAgent.name}"))
                {
                    _state = GuiState.Agent;
                }
                else
                {
                    RenderComponents(x, y, w, h, p);
                }

                return;
            }

            if (_state == GuiState.Component)
            {

                y = NextItem(y, h, p);
                if (GuiButton(x, y, w, h, $"Back to {_selectedAgent.name} Components"))
                {
                    _state = GuiState.Components;
                }
                
                RenderComponent(x, y, w, h, p);
                return;
            }

            y = NextItem(y, h, p);
            GuiLabel(x, y, w, h, $"{Agents.Length} Agents:");

            foreach (Agent agent in Agents)
            {
                y = NextItem(y, h, p);
                if (!GuiButton(x, y, w, h, agent.name))
                {
                    continue;
                }

                _selectedAgent = agent;
                _state = GuiState.Agent;
            }
        }

        private void RenderAgent(float x, float y, float w, float h, float p)
        {
            y = NextItem(y, h, p);
            GuiLabel(x, y, w, h, _selectedAgent.name);
            
            y = NextItem(y, h, p);
            GuiLabel(x, y, w, h, $"Performance: {_selectedAgent.Performance}");

            Mind mind = _selectedAgent.Mind;
            y = NextItem(y, h, p);
            GuiLabel(x, y, w, h, mind != null ? $"Mind: {mind.GetType().ToString().Split('.').Last()}" : "No mind.");
            
            y = NextItem(y, h, p);
            if (GuiButton(x, y, w, h, "View Components"))
            {
                _state = GuiState.Components;
            }
            
            y = NextItem(y, h, p);
            GuiLabel(x, y, w, h, $"Messages:");

            if (!_selectedAgent.HasMessages && (mind == null || !mind.HasMessages))
            {
                y = NextItem(y, h, p);
                GuiLabel(x, y, w, h, "No messages.");
                return;
            }

            foreach (string message in _selectedAgent.GetMessages())
            {
                y = NextItem(y, h, p);
                GuiLabel(x, y, w, h, message);
            }

            if (mind == null)
            {
                return;
            }
            
            foreach (string message in mind.GetMessages())
            {
                y = NextItem(y, h, p);
                GuiLabel(x, y, w, h, message);
            }
        }

        private void RenderComponents(float x, float y, float w, float h, float p)
        {
            foreach (Sensor sensor in _selectedAgent.Sensors)
            {
                y = NextItem(y, h, p);
                if (!GuiButton(x, y, w, h, sensor.GetType().ToString().Split('.').Last()))
                {
                    continue;
                }

                _selectedComponent = sensor;
                _state = GuiState.Component;
            }
            
            foreach (Actuator actuator in _selectedAgent.Actuators)
            {
                y = NextItem(y, h, p);
                if (!GuiButton(x, y, w, h, actuator.GetType().ToString().Split('.').Last()))
                {
                    continue;
                }

                _selectedComponent = actuator;
                _state = GuiState.Component;
            }
        }

        private void RenderComponent(float x, float y, float w, float h, float p)
        {
            y = NextItem(y, h, p);
            GuiLabel(x, y, w, h, $"{_selectedAgent.name} | {_selectedComponent.GetType().ToString().Split('.').Last()}");
            
            y = NextItem(y, h, p);
            GuiLabel(x, y, w, h, $"Messages:");
            
            if (!_selectedComponent.HasMessages)
            {
                y = NextItem(y, h, p);
                GuiLabel(x, y, w, h, "No messages.");
                return;
            }

            foreach (string message in _selectedComponent.GetMessages())
            {
                y = NextItem(y, h, p);
                GuiLabel(x, y, w, h, message);
            }
        }

        private void RenderCameras(float x, float y, float w, float h, float p)
        {
            if (Cameras.Length <= 1)
            {
                return;
            }
            
            x = Screen.width - x - w;
            
            if (GuiButton(x, y, w, h, _camerasOpen ? "Close Cameras" : "Open Cameras"))
            {
                _camerasOpen = !_camerasOpen;
            }
            
            if (!_camerasOpen)
            {
                return;
            }

            foreach (Camera cam in Cameras)
            {
                y = NextItem(y, h, p);
                if (!GUI.Button(new Rect(x, y, w, h), cam.name))
                {
                    continue;
                }

                cam.enabled = true;
                foreach (Camera cam2 in Cameras)
                {
                    if (cam != cam2)
                    {
                        cam2.enabled = false;
                    }
                }
            }
        }
    }
}