using System.Linq;
using SimpleIntelligence.Actuators;
using SimpleIntelligence.Agents;
using SimpleIntelligence.Components;
using SimpleIntelligence.Minds;
using SimpleIntelligence.Sensors;
using UnityEngine;

namespace SimpleIntelligence.Managers
{
    public class GuiAgentManager : AgentManager
    {
        private const float ClosedSize = 70;
        
        private enum GuiState : byte
        {
            Main,
            Agent,
            Components,
            Component
        }
        
        [SerializeField]
        [Min(0)]
        [Tooltip("How wide the details list is.")]
        private float detailsWidth = 450;
        
        [SerializeField]
        [Min(0)]
        [Tooltip("How wide the controls list is.")]
        private float controlsWidth = 120;

        private GuiState _state;

        private bool _menuOpen;

        private bool _controls;

        private Agent _selectedAgent;

        private IntelligenceComponent _selectedComponent;

        protected virtual float CustomRendering(float x, float y, float w, float h, float p)
        {
            return 0;
        }

        protected static bool GuiButton(float x, float y, float w, float h, string message)
        {
            return !(y + h > Screen.height) && GUI.Button(new Rect(x, y, w, h), message);
        }

        protected static void GuiLabel(float x, float y, float w, float h, float p, string message)
        {
            if (y + h > Screen.height)
            {
                return;
            }
            
            GUI.Label(new Rect(x + p, y, w - p, h), message);
        }

        protected static void GuiBox(float x, float y, float w, float h, float p, int number)
        {
            while (y + (h + p) * number - p > Screen.height)
            {
                number--;
                if (number <= 0)
                {
                    return;
                }
            }
            
            GUI.Box(new Rect(x,y,w,(h + p) * number - p), string.Empty);
        }

        protected static float NextItem(float y, float h, float p)
        {
            return y + h + p;
        }

        private void OnGUI()
        {
            Render(10, 10, 20, 5);
        }

        private void Render(float x, float y, float h, float p)
        {
            RenderDetails(x, y, detailsWidth, h, p);
            RenderControls(x, y, controlsWidth, h, p);
        }

        private float RenderMessageOptions(float x, float y, float w, float h, float p)
        {
            y = NextItem(y, h, p);
            if (GuiButton(x, y, w, h, MessageMode switch
                {
                    MessagingMode.Compact => "Compact Messages",
                    MessagingMode.All => "All Messages",
                    _ => "Unique Messages"
                }))
            {
                ChangeMessageMode();
            }

            y = NextItem(y, h, p);
            if (GuiButton(x, y, w, h, "Clear Messages"))
            {
                ClearMessages();
            }

            return y;
        }

        private void RenderDetails(float x, float y, float w, float h, float p)
        {
            if (Agents.Length < 1)
            {
                return;
            }

            if (!_menuOpen)
            {
                w = ClosedSize;
            }

            if (w + 4 * p > Screen.width)
            {
                w = Screen.width - 4 * p;
            }
            
            if (GuiButton(x, y, w, h, _menuOpen ? "Close" : "Details"))
            {
                _menuOpen = !_menuOpen;
            }
            
            if (!_menuOpen)
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
                if (GuiButton(x, y, w, h, $"Back to {_selectedAgent.name} Sensors and Actuators"))
                {
                    _state = GuiState.Components;
                }
                
                RenderComponent(x, y, w, h, p);
                return;
            }

            y = NextItem(y, h, p);
            GuiBox(x, y, w, h, p, 1);
            GuiLabel(x, y, w, h, p, $"{Agents.Length} Agents:");

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
            GuiBox(x, y, w, h, p, Agents.Length > 1 ? 3 : 2);
            if (Agents.Length > 1)
            {
                GuiLabel(x, y, w, h, p, _selectedAgent.name);
                y = NextItem(y, h, p);
            }
            
            GuiLabel(x, y, w, h, p, $"Performance: {_selectedAgent.Performance}");

            Mind mind = _selectedAgent.Mind;
            y = NextItem(y, h, p);
            GuiLabel(x, y, w, h, p, mind != null ? $"Mind: {mind.GetType().ToString().Split('.').Last()}" : "Mind: None");

            if (_selectedAgent.Sensors.Length > 0 && _selectedAgent.Actuators.Length > 0)
            {
                y = NextItem(y, h, p);
                if (GuiButton(x, y, w, h, "Sensors and Actuators"))
                {
                    _state = GuiState.Components;
                }
            }

            if (mind == null)
            {
                return;
            }

            y = RenderMessageOptions(x, y, w, h, p);
            
            y = NextItem(y, h, p);
            GuiBox(x, y, w, h, p, !mind.HasMessages ? 1 : mind.MessageCount);

            if (!mind.HasMessages)
            {
                GuiLabel(x, y, w, h, p, "No messages.");
                return;
            }
            
            foreach (string message in mind.Messages)
            {
                GuiLabel(x, y, w, h, p, message);
                y = NextItem(y, h, p);
            }
        }

        private void RenderComponents(float x, float y, float w, float h, float p)
        {
            y = NextItem(y, h, p);
            GuiBox(x, y, w, h, p, 1);
            GuiLabel(x, y, w, h, p, _selectedAgent.Sensors.Length switch
            {
                0 => "No Sensors",
                1 => "1 Sensor",
                _ => $"{_selectedAgent.Sensors.Length} Sensors"
            });

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
            
            y = NextItem(y, h, p);
            GuiBox(x, y, w, h, p, 1);
            GuiLabel(x, y, w, h, p, _selectedAgent.Actuators.Length switch
            {
                0 => "No Actuators",
                1 => "1 Actuator",
                _ => $"{_selectedAgent.Actuators.Length} Actuators"
            });
            
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
            GuiBox(x, y, w, h, p, 1);
            GuiLabel(x, y, w, h, p, $"{_selectedAgent.name} | {_selectedComponent.GetType().ToString().Split('.').Last()}");
            
            y = RenderMessageOptions(x, y, w, h, p);
            
            y = NextItem(y, h, p);
            GuiBox(x, y, w, h, p, !_selectedComponent.HasMessages ? 1 : _selectedComponent.MessageCount);
            
            if (!_selectedComponent.HasMessages)
            {
                GuiLabel(x, y, w, h, p, "No messages.");
                return;
            }

            foreach (string message in _selectedComponent.Messages)
            {
                GuiLabel(x, y, w, h, p, message);
                y = NextItem(y, h, p);
            }
        }

        private void RenderControls(float x, float y, float w, float h, float p)
        {
            if (!_controls)
            {
                w = ClosedSize;
            }
            
            if (Agents.Length == 0 && w + 4 * p > Screen.width)
            {
                w = Screen.width - 4 * p;
            }

            if (Agents.Length > 0 && Screen.width < (_menuOpen ? detailsWidth : ClosedSize) + controlsWidth + 5 * p)
            {
                return;
            }
            
            x = Screen.width - x - w;

            if (GuiButton(x, y, w, h, _controls ? "Close" : "Controls"))
            {
                _controls = !_controls;
            }
            
            if (!_controls)
            {
                return;
            }

            y = NextItem(y, h, p);
            y = CustomRendering(x, y, w, h, p);

            if (GuiButton(x, y, w, h, Playing ? "Pause" : "Resume"))
            {
                if (Playing)
                {
                    Pause();
                }
                else
                {
                    Resume();
                }
            }

            if (!Playing)
            {
                y = NextItem(y, h, p);
                if (GuiButton(x, y, w, h, "Step"))
                {
                    Step();
                }
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
            
            
            y = NextItem(y, h, p);
            if (GuiButton(x, y, w, h, "Quit"))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
#else
                Application.Quit();
#endif
            }
        }
    }
}