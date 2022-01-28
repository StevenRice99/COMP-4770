using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyAI.Actuators;
using EasyAI.Agents;
using EasyAI.Components;
using EasyAI.Minds;
using EasyAI.Percepts;
using EasyAI.Sensors;
using UnityEngine;

namespace EasyAI.Managers
{
    public class AgentManager : MonoBehaviour
    {
        public enum MessagingMode : byte
        {
            All,
            Compact,
            Unique
        }

        public enum GizmosState : byte
        {
            Off,
            All,
            Selected
        }
        
        private enum GuiState : byte
        {
            Main,
            Agent,
            Components,
            Component
        }
        
        private const float ClosedSize = 70;

        public static AgentManager Singleton;

        private static Material _lineMaterial;

        [SerializeField]
        [Min(0)]
        [Tooltip("The maximum number of agents which can be updated in a single frame. Set to zero to be unlimited.")]
        private int maxAgentsPerUpdate;

        [SerializeField]
        [Tooltip("The maximum number of messages any component can hold.")]
        private int maxMessages = 100;
        
        [SerializeField]
        [Min(0)]
        [Tooltip("How wide the details list is. Set to zero to disable details list rendering.")]
        private float detailsWidth = 450;
        
        [SerializeField]
        [Min(0)]
        [Tooltip("How wide the controls list is. Set to zero to disable controls list rendering.")]
        private float controlsWidth = 120;

        public int MaxMessages => maxMessages;

        public bool Playing => !_stepping && Time.timeScale > 0;

        public MessagingMode MessageMode { get; private set; }
        
        public List<string> GlobalMessages { get; private set; } = new List<string>();

        protected List<Agent> Agents = new List<Agent>();

        protected Camera[] Cameras = Array.Empty<Camera>();

        private int _currentAgentIndex;

        private bool _stepping;

        private GuiState _state;

        private GizmosState _gizmos;

        private bool _menuOpen;

        private bool _controls;

        private Agent _selectedAgent;

        private IntelligenceComponent _selectedComponent;

        public static void Resume()
        {
            Time.timeScale = 1;
        }

        public static void Pause()
        {
            Time.timeScale = 0;
        }

        public static bool GuiButton(float x, float y, float w, float h, string message)
        {
            return !(y + h > Screen.height) && GUI.Button(new Rect(x, y, w, h), message);
        }

        public static void GuiLabel(float x, float y, float w, float h, float p, string message)
        {
            if (y + h > Screen.height)
            {
                return;
            }
            
            GUI.Label(new Rect(x + p, y, w - p, h), message);
        }

        public static void GuiBox(float x, float y, float w, float h, float p, int number)
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

        public static float NextItem(float y, float h, float p)
        {
            return y + h + p;
        }

        public void AddGlobalMessage(string message)
        {
            switch (MessageMode)
            {
                case MessagingMode.Compact when GlobalMessages.Count > 0 && GlobalMessages[0] == message:
                    return;
                case MessagingMode.Unique:
                    GlobalMessages = GlobalMessages.Where(m => m != message).ToList();
                    break;
            }

            GlobalMessages.Insert(0, message);
            if (GlobalMessages.Count > Singleton.MaxMessages)
            {
                GlobalMessages.RemoveAt(GlobalMessages.Count - 1);
            }
        }

        public void AddAgent(Agent agent)
        {
            if (Agents.Contains(agent))
            {
                return;
            }
            
            Agents.Add(agent);
            FindCameras();
        }

        public void RemoveAgent(Agent agent)
        {
            if (!Agents.Contains(agent))
            {
                return;
            }

            int index = Agents.IndexOf(agent);
            Agents.Remove(agent);
            if (_currentAgentIndex > index)
            {
                _currentAgentIndex--;
            }

            if (_currentAgentIndex < 0 || _currentAgentIndex >= Agents.Count)
            {
                _currentAgentIndex = 0;
            }

            FindCameras();
        }

        public void FindCameras()
        {
            Cameras = FindObjectsOfType<Camera>().OrderBy(c => c.name).ToArray();
        }

        public void ChangeMessageMode()
        {
            if (MessageMode == MessagingMode.Unique)
            {
                MessageMode = MessagingMode.All;
            }
            else
            {
                MessageMode++;
            }

            if (MessageMode == MessagingMode.Unique)
            {
                ClearMessages();
            }
        }

        public void ChangeMessageMode(MessagingMode mode)
        {
            MessageMode = mode;
        }

        public void ChangeGizmosState()
        {
            if (_gizmos == GizmosState.Selected)
            {
                _gizmos = GizmosState.Off;
                return;
            }

            _gizmos++;
        }

        public void ChangeGizmosState(GizmosState state)
        {
            _gizmos = state;
        }

        public void Step()
        {
            StartCoroutine(StepOneFrame());
        }

        public void ClearMessages()
        {
            GlobalMessages.Clear();
            foreach (IntelligenceComponent component in FindObjectsOfType<IntelligenceComponent>())
            {
                component.ClearMessages();
            }
        }

        public void SwitchCamera(Camera cam)
        {
            cam.enabled = true;
            foreach (Camera cam2 in Cameras)
            {
                if (cam != cam2)
                {
                    cam2.enabled = false;
                }
            }
        }

        protected virtual void Start()
        {
            FindCameras();
            if (Cameras.Length > 0)
            {
                SwitchCamera(Cameras[0]);
            }
        }

        protected virtual void Update()
        {
            if (maxAgentsPerUpdate <= 0)
            {
                foreach (Agent agent in Agents)
                {
                    agent.Perform();
                }
            
                return;
            }
        
            for (int i = 0; i < maxAgentsPerUpdate && i < Agents.Count; i++)
            {
                Agents[_currentAgentIndex].Perform();
                NextAgent();
            }
        }
        
        protected virtual float CustomRendering(float x, float y, float w, float h, float p)
        {
            return 0;
        }

        private static void LineMaterial()
        {
            if (_lineMaterial)
            {
                return;
            }

            // Unity has a built-in shader that is useful for drawing simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            
            // Turn on alpha blending.
            _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            
            // Turn backface culling off.
            _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            
            // Turn off depth writes.
            _lineMaterial.SetInt("_ZWrite", 0);
        }

        private static void AgentGizmos(Agent agent)
        {
            if (agent.Mind != null)
            {
                agent.Mind.DisplayGizmos();
            }

            foreach (Actuator actuator in agent.Actuators)
            {
                actuator.DisplayGizmos();
            }

            foreach (Sensor sensor in agent.Sensors)
            {
                sensor.DisplayGizmos();
            }
        }

        private void OnGUI()
        {
            Render(10, 10, 20, 5);
        }
        
        private void OnRenderObject()
        {
            if (_gizmos == GizmosState.Off)
            {
                return;
            }
            
            LineMaterial();
            _lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.LINES);

            if (_gizmos == GizmosState.All)
            {
                foreach (Agent agent in Agents)
                {
                    AgentGizmos(agent);
                }
            }
            else
            {
                if (Agents.Count == 1)
                {
                    _selectedAgent = Agents[0];
                }
                
                if (_selectedComponent != null)
                {
                    _selectedComponent.DisplayGizmos();
                }
                else if (_selectedAgent != null)
                {
                    AgentGizmos(_selectedAgent);
                }
            }
            
            GL.End();
            GL.PopMatrix();
        }

        private void Render(float x, float y, float h, float p)
        {
            if (detailsWidth > 0)
            {
                RenderDetails(x, y, detailsWidth, h, p);
            }

            if (controlsWidth > 0)
            {
                RenderControls(x, y, controlsWidth, h, p);
            }
        }

        private float RenderMessageOptions(float x, float y, float w, float h, float p)
        {
            y = NextItem(y, h, p);
            if (GuiButton(x, y, w / 2 - p, h, MessageMode switch
                {
                    MessagingMode.Compact => "Message Mode: Compact",
                    MessagingMode.All => "Message Mode: All",
                    _ => "Message Mode: Unique"
                }))
            {
                ChangeMessageMode();
            }

            if (GuiButton(x + w / 2 + p, y, w / 2 - p, h, "Clear Messages"))
            {
                ClearMessages();
            }

            return y;
        }

        private void RenderDetails(float x, float y, float w, float h, float p)
        {
            if (Agents.Count < 1)
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

            if (_state == GuiState.Main && Agents.Count == 1)
            {
                _selectedAgent = Agents[0];
                _state = GuiState.Agent;
            }

            if (_state == GuiState.Agent)
            {
                if (Agents.Count > 1)
                {
                    y = NextItem(y, h, p);
                    if (GuiButton(x, y, w, h, "Back to Overview"))
                    {
                        _selectedAgent = null;
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
                    _selectedComponent = null;
                    _state = GuiState.Components;
                }
                
                RenderComponent(x, y, w, h, p);
                return;
            }

            y = NextItem(y, h, p);
            GuiBox(x, y, w, h, p, 1);
            GuiLabel(x, y, w, h, p, $"{Agents.Count} Agents:");

            foreach (Agent agent in Agents)
            {
                y = NextItem(y, h, p);
                if (!GuiButton(x, y, w, h, $"{agent.name} - {agent}" + (agent.Mind == null ? " - No Mind." : $" - {agent.Mind}")))
                {
                    continue;
                }

                _selectedAgent = agent;
                _state = GuiState.Agent;
            }
            
            if (GlobalMessages.Count == 0)
            {
                y = NextItem(y, h, p);
                GuiBox(x, y, w, h, p, 1);
                GuiLabel(x, y, w, h, p, $"No messages.");
                return;
            }
            
            y = RenderMessageOptions(x, y, w, h, p);
            
            y = NextItem(y, h, p);
            GuiBox(x, y, w, h, p, GlobalMessages.Count);
            
            foreach (string message in GlobalMessages)
            {
                GuiLabel(x, y, w, h, p, message);
                y = NextItem(y, h, p);
            }
        }

        private void RenderAgent(float x, float y, float w, float h, float p)
        {
            if (_selectedAgent == null)
            {
                _state = GuiState.Main;
                return;
            }
            
            y = NextItem(y, h, p);
            int length = 4;
            if (Agents.Count > 1)
            {
                length++;
            }
            
            GuiBox(x, y, w, h, p, length);
            if (Agents.Count > 1)
            {
                GuiLabel(x, y, w, h, p, _selectedAgent.name);
                y = NextItem(y, h, p);
            }

            GuiLabel(x, y, w, h, p, $"Type: {_selectedAgent}");
            y = NextItem(y, h, p);
            Mind mind = _selectedAgent.Mind;
            GuiLabel(x, y, w, h, p, (mind != null ? $"Mind: {mind}" : "Mind: None") + $" | Performance: {_selectedAgent.Performance}");
            y = NextItem(y, h, p);
            GuiLabel(x, y, w, h, p, $"Position: {_selectedAgent.Position} | " + (_selectedAgent.MovingToTarget ? $"Moving to {_selectedAgent.MoveTarget}." : "Not moving."));
            y = NextItem(y, h, p);
            GuiLabel(x, y, w, h, p, $"Rotation: {_selectedAgent.Rotation.eulerAngles.y} | " + (_selectedAgent.LookingToTarget ? $"Looking to {_selectedAgent.LookTarget}." : "Not looking."));

            if (mind != null)
            {
                y = _selectedAgent.Mind.DisplayDetails(x, y, w, h, p);
            }

            if (_selectedAgent.Sensors.Length > 0 && _selectedAgent.Actuators.Length > 0)
            {
                y = NextItem(y, h, p);
                if (GuiButton(x, y, w, h, "Sensors, Actuators, Percepts, and Actions"))
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
            if (_selectedAgent == null)
            {
                _state = GuiState.Main;
                return;
            }
            
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
                if (!GuiButton(x, y, w, h, sensor.ToString()))
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
                if (!GuiButton(x, y, w, h, actuator.ToString()))
                {
                    continue;
                }

                _selectedComponent = actuator;
                _state = GuiState.Component;
            }
            
            Percept[] percepts = _selectedAgent.Percepts.Where(p => p != null).ToArray();
            if (percepts.Length > 0)
            {
                y = NextItem(y, h, p);
                GuiBox(x, y, w, h, p, 1 + percepts.Length);
            
                GuiLabel(x, y, w, h, p, percepts.Length == 1 ? "1 Percept" :$"{percepts.Length} Percepts");

                foreach (Percept percept in percepts)
                {
                    string msg = percept.DetailsDisplay();
                    y = NextItem(y, h, p);
                    GuiLabel(x, y, w, h, p, percept + (string.IsNullOrWhiteSpace(msg) ? string.Empty : $": {msg}"));
                }
            }

            EasyAI.Actions.Action[] actions = _selectedAgent.Actions?.Where(a => a != null).ToArray();
            if (actions != null && actions.Length > 0)
            {
                y = NextItem(y, h, p);
                GuiBox(x, y, w, h, p, 1 + actions.Length);
            
                GuiLabel(x, y, w, h, p, actions.Length == 1 ? "1 Action" :$"{actions.Length} Actions");

                foreach (EasyAI.Actions.Action action in actions)
                {
                    string msg = action.DetailsDisplay();
                    y = NextItem(y, h, p);
                    GuiLabel(x, y, w, h, p, action + (string.IsNullOrWhiteSpace(msg) ? string.Empty : $": {msg}"));
                }
            }
        }

        private void RenderComponent(float x, float y, float w, float h, float p)
        {
            if (_selectedComponent == null)
            {
                _state = GuiState.Components;
                return;
            }
            
            y = NextItem(y, h, p);
            GuiBox(x, y, w, h, p, 1);
            GuiLabel(x, y, w, h, p, $"{_selectedAgent.name} | {_selectedComponent}");
            
            y = _selectedComponent.DisplayDetails(x, y, w, h, p);
            
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
            
            if (Agents.Count == 0 && w + 4 * p > Screen.width)
            {
                w = Screen.width - 4 * p;
            }

            if (Agents.Count > 0 && Screen.width < (_menuOpen ? detailsWidth : ClosedSize) + controlsWidth + 5 * p)
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
            
            y = NextItem(y, h, p);
            if (GuiButton(x, y, w, h, _gizmos switch
                {
                    GizmosState.Off => "Gizmos: Off",
                    GizmosState.Selected => "Gizmos: Selected",
                    _ => "Gizmos: All"
                }))
            {
                ChangeGizmosState();
            }

            foreach (Camera cam in Cameras)
            {
                y = NextItem(y, h, p);
                if (GUI.Button(new Rect(x, y, w, h), cam.name))
                {
                    SwitchCamera(cam);
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
    
        private void Awake()
        {
            if (Singleton == this)
            {
                return;
            }

            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        private void NextAgent()
        {
            _currentAgentIndex++;
            if (_currentAgentIndex >= Agents.Count)
            {
                _currentAgentIndex = 0;
            }
        }
        
        private IEnumerator StepOneFrame()
        {
            _stepping = true;
            Resume();
            yield return 0;
            Pause();
            _stepping = false;
        }
    }
}