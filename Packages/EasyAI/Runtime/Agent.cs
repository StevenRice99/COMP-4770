using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Base class for all agents.
/// </summary>
public abstract class Agent : MessageComponent
{
    public enum MoveType : byte
    {
        Seek,
        Flee,
        Pursuit,
        Evade,
        Wander
    }

    public class MoveData
    {
        public MoveType MoveType { get; set; }

        public Transform Transform { get; set; }

        private Vector2 _position;
        
        public Vector2 LastPosition { get; set; }
        
        public Vector2 Position
        {
            get
            {
                if (Transform == null)
                {
                    return _position;
                }

                Vector3 pos3 = Transform.position;
                return new Vector2(pos3.x, pos3.z);

            }
        }

        public MoveData(MoveType moveType, Transform transform)
        {
            MoveType = moveType;
            Transform = transform;
            Vector3 pos3 = transform.position;
            _position = new Vector2(pos3.x, pos3.z);
            LastPosition = _position;
        }

        public MoveData(MoveType moveType, Vector3 pos)
        {
            MoveType = moveType;
            Transform = null;
            _position = new Vector2(pos.x, pos.z);
            LastPosition = _position;
        }

        public MoveData(MoveType moveType, Vector2 position)
        {
            MoveType = moveType;
            Transform = null;
            _position = position;
            LastPosition = _position;
        }
    }
    
    [SerializeField]
    [Min(0)]
    [Tooltip("How fast this agent can move in units per second.")]
    public float moveSpeed = 10;
    
    [SerializeField]
    [Min(0)]
    [Tooltip("How fast this agent can increase in move speed in units per second.")]
    public float moveAcceleration = 10;

    [SerializeField]
    [Min(0)]
    [Tooltip("How close an agent can be to a location its seeking or pursuing to declare it as reached?.")]
    public float seekAcceptableDistance = 0.1f;

    [SerializeField]
    [Min(0)]
    [Tooltip("How far an agent can be to a location its fleeing or evading from to declare it as reached?.")]
    public float fleeAcceptableDistance = 10f;

    [SerializeField]
    [Min(0)]
    [Tooltip("If the agent is not moving, ensure it comes to a complete stop when its velocity is less than this.")]
    public float restVelocity = 0.1f;
        
    [SerializeField]
    [Min(0)]
    [Tooltip("How fast this agent can look in degrees per second.")]
    public float lookSpeed = 5;

    [SerializeField]
    [Tooltip("The global state the agent is in. Initialize it with the global state to start it.")]
    private State globalState;
    
    [SerializeField]
    [Tooltip("The current state the agent is in. Initialize it with the state to start in.")]
    private State state;

    /// <summary>
    /// The global state the agent is in.
    /// </summary>
    public State GlobalState
    {
        get => globalState;
        set
        {
            if (globalState != null)
            {
                globalState.Exit(this);
            }

            globalState = value;

            if (globalState != null)
            {
                globalState.Execute(this);
            }
        }
    }

    /// <summary>
    /// The state the agent is in.
    /// </summary>
    public State State
    {
        get => state;
        set
        {
            PreviousState = state;
            
            if (state != null)
            {
                state.Exit(this);
            }

            state = value;

            if (state != null)
            {
                state.Enter(this);
            }
        }
    }

    /// <summary>
    /// The previous state the agent was in.
    /// </summary>
    public State PreviousState { get; private set; }

    /// <summary>
    /// The current move velocity if move acceleration is being used as a Vector3.
    /// </summary>
    public Vector3 MoveVelocity3 => new Vector3(MoveVelocity.x, 0, MoveVelocity.y);

    /// <summary>
    /// The current move velocity if move acceleration is being used.
    /// </summary>
    public Vector2 MoveVelocity { get; protected set; }
        
    /// <summary>
    /// The time passed since the last time the agent's mind made decisions. Use this instead of Time.DeltaTime.
    /// </summary>
    public float DeltaTime { get; set; }

    /// <summary>
    /// The target the agent is currently trying to look towards.
    /// </summary>
    public Vector3 LookTarget { get; private set; }

    /// <summary>
    /// True if the agent is trying to look to a target, false otherwise.
    /// </summary>
    public bool LookingToTarget { get; private set; }

    /// <summary>
    /// The performance measure of the agent.
    /// </summary>
    public float Performance { get; private set; }

    /// <summary>
    /// Get the currently selected mind of the agent.
    /// </summary>
    public Mind SelectedMind => Minds != null && Minds.Length > 0 ? Minds[_selectedMindIndex] : null;
    
    /// <summary>
    /// The mind of this agent.
    /// </summary>
    public Mind[] Minds { get; private set; }

    /// <summary>
    /// The sensors of this agent.
    /// </summary>
    public Sensor[] Sensors { get; private set; }

    /// <summary>
    /// The percepts of this agent.
    /// </summary>
    public Percept[] Percepts { get; private set; }

    /// <summary>
    /// The actuators of this agent.
    /// </summary>
    public Actuator[] Actuators { get; private set; }

    /// <summary>
    /// The actions of this agent.
    /// </summary>
    public Action[] Actions { get; private set; }

    /// <summary>
    /// The root transform that holds the visuals for this agent used to rotate the agent towards its look target.
    /// </summary>
    public Transform Visuals { get; private set; }

    /// <summary>
    /// The performance measure of this agent.
    /// </summary>
    public PerformanceMeasure PerformanceMeasure { get; private set; }

    public List<MoveData> MovesData { get; private set; } = new List<MoveData>();

    /// <summary>
    /// The index of the currently selected mind.
    /// </summary>
    private int _selectedMindIndex;
    
    /// <summary>
    /// Display a green line from the agent's position to its move target and a blue line from the agent's position
    /// to its look target. If the look target is the same as the move target, the blue line will not be drawn as
    /// otherwise they would overlap.
    /// </summary>
    public override void DisplayGizmos()
    {
        foreach (MoveData moveData in MovesData)
        {
            GL.Color(moveData.MoveType switch
            {
                MoveType.Seek => Color.blue,
                MoveType.Flee => Color.red,
                MoveType.Pursuit => Color.cyan,
                MoveType.Evade => Color.yellow,
                _ => Color.magenta
            });
            Vector3 position = transform.position;
            GL.Vertex(position);
            GL.Vertex(new Vector3(moveData.Position.x, position.y, moveData.Position.y));
        }

        if (MoveVelocity != Vector2.zero)
        {
            GL.Color(Color.green);
            Vector3 position = transform.position;
            GL.Vertex(position);
            GL.Vertex(position + transform.rotation * (MoveVelocity3.normalized * 2));
        }
        
        if (LookingToTarget)
        {
            GL.Color(Color.green);
            GL.Vertex(transform.position);
            GL.Vertex(LookTarget);
        }
    }

    public void SetMoveData(MoveType moveType, Transform tr)
    {
        MovesData.Clear();
        AddMoveData(moveType, tr);
    }

    public void SetMoveData(MoveType moveType, Vector3 pos)
    {
        MovesData.Clear();
        AddMoveData(moveType, pos);
    }

    public void AddMoveData(MoveType moveType, Transform tr)
    {
        if (IsCompleteMove(moveType, new Vector2(transform.position.x, transform.position.z), new Vector2(tr.position.x, tr.position.z)))
        {
            return;
        }
        MovesData.Add(new MoveData(moveType, tr));
    }

    public void AddMoveData(MoveType moveType, Vector3 pos)
    {
        if (IsCompleteMove(moveType, new Vector2(transform.position.x, transform.position.z), new Vector2(pos.x, pos.z)))
        {
            return;
        }
        MovesData.Add(new MoveData(moveType, pos));
    }

    public void AddMoveData(MoveType moveType, Vector2 pos)
    {
        if (IsCompleteMove(moveType, new Vector2(transform.position.x, transform.position.z), pos))
        {
            return;
        }
        MovesData.Add(new MoveData(moveType, pos));
    }

    public void ClearMoveData()
    {
        MovesData.Clear();
    }

    public void RemoveMoveData(Transform tr)
    {
        MovesData = MovesData.Where(m => m.Transform != tr).ToList();
    }

    public void RemoveMoveData(Vector3 pos)
    {
        RemoveMoveData(new Vector2(pos.x, pos.z));
    }

    public void RemoveMoveData(Vector2 pos)
    {
        MovesData = MovesData.Where(m => m.Position != pos).ToList();
    }

    /// <summary>
    /// Fire an event to an agent.
    /// </summary>
    /// <param name="receiver">The agent to send the event to.</param>
    /// <param name="eventId">The event ID which the receiver will use to identify the type of message.</param>
    /// <param name="details">Object which contains all data for this message.</param>
    /// <returns>True if the receiver handled the message, false otherwise.</returns>
    public bool FireEvent(Agent receiver, int eventId, object details = null)
    {
        return receiver != null && receiver != this && receiver.HandleEvent(new AIEvent(eventId, this, details));
    }

    /// <summary>
    /// Broadcast a message to all other agents.
    /// </summary>
    /// <param name="eventId">The event ID which the receivers will use to identify the type of message.</param>
    /// <param name="details">Object which contains all data for this message.</param>
    /// <param name="requireAll">Setting to true will check for all agents handling the message, false means only one agent needs to handle it.</param>
    /// <returns>If require all is true, true if all agents handle the message and false otherwise and if require all is false, true if at least one agent handles the message, false otherwise.</returns>
    public bool BroadcastEvent(int eventId, object details = null, bool requireAll = false)
    {
        bool all = true;
        bool one = false;
        foreach (bool result in AgentManager.Singleton.Agents.Where(a => a != this).Select(a => a.HandleEvent(new AIEvent(eventId, this, details))))
        {
            if (result)
            {
                one = true;
            }
            else
            {
                all = false;
            }
        }

        return requireAll ? all : one;
    }

    /// <summary>
    /// Assign the movement speed of the agent.
    /// </summary>
    /// <param name="speed">The movement speed.</param>
    public void AssignMoveSpeed(float speed)
    {
        moveSpeed = Math.Abs(speed);
    }

    /// <summary>
    /// Assign the look speed of the agent.
    /// </summary>
    /// <param name="speed">The look speed.</param>
    public void AssignLookSpeed(float speed)
    {
        lookSpeed = Math.Abs(speed);
    }

    /// <summary>
    /// Assign a mind to this agent.
    /// </summary>
    /// <param name="type">The type of mind to assign.</param>
    public void AssignMind(Type type)
    {
        if (Minds == null || Minds.Length == 0 || type == null)
        {
            _selectedMindIndex = 0;
            return;
        }

        Mind mind = Minds.FirstOrDefault(m => m.GetType() == type);
        if (mind == null)
        {
            _selectedMindIndex = 0;
            return;
        }

        _selectedMindIndex = Minds.ToList().IndexOf(mind);
    }

    /// <summary>
    /// Assign a performance measure to this agent.
    /// </summary>
    /// <param name="performanceMeasure">The performance measure to assign.</param>
    public void AssignPerformanceMeasure(PerformanceMeasure performanceMeasure)
    {
        PerformanceMeasure = performanceMeasure;
        ConfigurePerformanceMeasure();
    }

    /// <summary>
    /// Resume looking towards the look target currently assigned to the agent.
    /// </summary>
    public void LookAtTarget()
    {
        LookingToTarget = LookTarget != transform.position;
    }

    /// <summary>
    /// Set a target position for the agent to look towards.
    /// </summary>
    /// <param name="target">The target position to look to.</param>
    public void LookAtTarget(Vector3 target)
    {
        LookTarget = target;
        LookAtTarget();
    }

    /// <summary>
    /// Set a target transform for the agent to look towards.
    /// </summary>
    /// <param name="target">The target transform to look to.</param>
    public void LookAtTarget(Transform target)
    {
        if (target == null)
        {
            StopLookAtTarget();
            return;
        }
            
        LookAtTarget(target.position);
    }

    /// <summary>
    /// Have the agent stop looking towards its look target.
    /// </summary>
    public void StopLookAtTarget()
    {
        LookingToTarget = false;
    }

    /// <summary>
    /// Instantly stop all actions this agent is performing.
    /// </summary>
    public void StopAllActions()
    {
        Actions = null;
    }

    /// <summary>
    /// Called by the AgentManager to have the agent sense, think, and act.
    /// </summary>
    public void Perform()
    {
        if (globalState != null)
        {
            globalState.Execute(this);
        }

        if (state != null)
        {
            state.Execute(this);
        }

        // Can only sense, think, and act if there is a mind attached.
        if (Minds != null && Minds.Length > 0)
        {
            // Sense the agent's surroundings.
            Sense();
                
            // Have the mind make decisions on what actions to take.
            Action[] decisions = Minds[_selectedMindIndex].Think(Percepts);
            
            // If new decisions were made, update the actions to be them.
            if (decisions != null)
            {
                // Remove any null actions.
                List<Action> updated = decisions.Where(a => a != null).ToList();

                // If there were previous actions, keep actions of types which were not in the current decisions.
                if (Actions != null)
                {
                    foreach (Action action in Actions)
                    {
                        if (action == null)
                        {
                            continue;
                        }
            
                        if (!updated.Exists(a => a.GetType() == action.GetType()))
                        {
                            updated.Add(action);
                        }
                    }
                }
        
                Actions = updated.ToArray();
            }

            // Act on the actions.
            Act();
        }

        // After all actions are performed, calculate the agent's new performance.
        if (PerformanceMeasure != null)
        {
            Performance = PerformanceMeasure.GetPerformance();
        }
            
        // Reset the elapsed time for the next time this method is called.
        DeltaTime = 0;
    }

    /// <summary>
    /// Override to easily display the type of the component for easy usage in messages.
    /// </summary>
    /// <returns>Name of this type.</returns>
    public override string ToString()
    {
        return GetType().Name;
    }

    public void Setup()
    {
        // Register this agent with the manager.
        AgentManager.Singleton.AddAgent(this);
            
        // Find all minds.
        List<Mind> minds = GetComponents<Mind>().ToList();
        minds.AddRange(GetComponentsInChildren<Mind>());
        Minds = minds.Distinct().ToArray();
        foreach (Mind mind in minds)
        {
            mind.Agent = this;
        }
            
        // Find the performance measure.
        PerformanceMeasure = GetComponent<PerformanceMeasure>();
        if (PerformanceMeasure == null)
        {
            PerformanceMeasure = GetComponentInChildren<PerformanceMeasure>();
            if (PerformanceMeasure == null)
            {
                PerformanceMeasure = FindObjectsOfType<PerformanceMeasure>().FirstOrDefault(m => m.Agent == null);
            }
        }

        ConfigurePerformanceMeasure();

        // Find all attached actuators.
        List<Actuator> actuators = GetComponents<Actuator>().ToList();
        actuators.AddRange(GetComponentsInChildren<Actuator>());
        Actuators = actuators.Distinct().ToArray();
        foreach (Actuator actuator in Actuators)
        {
            actuator.Agent = this;
        }
            
        // Find all attached sensors.
        List<Sensor> sensors = GetComponents<Sensor>().ToList();
        sensors.AddRange(GetComponentsInChildren<Sensor>());
        Sensors = sensors.Distinct().ToArray();
            
        // Setup the percepts array to match the size of the sensors so each sensor can return a percept to its index.
        foreach (Sensor sensor in Sensors)
        {
            sensor.Agent = this;
        }

        // Setup the root visuals transform for agent rotation.
        Transform[] children = GetComponentsInChildren<Transform>();
        if (children.Length == 0)
        {
            GameObject go = new GameObject("Visuals");
            Visuals = go.transform;
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            return;
        }

        Visuals = children.FirstOrDefault(t => t.name == "Visuals");
        if (Visuals == null)
        {
            Visuals = children[0];
        }
    }
        
    /// <summary>
    /// Implement movement behaviour.
    /// </summary>
    public abstract void Move();

    /// <summary>
    /// Look towards the agent's look target.
    /// </summary>
    public void Look()
    {
        Transform visuals = Visuals;

        Vector3 target;
        
        // If the agent has no otherwise set point to look, look in the direction it is moving.
        if (!LookingToTarget)
        {
            if (MoveVelocity == Vector2.zero)
            {
                return;
            }
            
            Transform t = transform;
            target = t.position + t.rotation * MoveVelocity3.normalized;
        }
        else
        {
            // We only want to rotate along the Y axis so update the target rotation to be at the same Y level.
            target = new Vector3(LookTarget.x, visuals.position.y, LookTarget.z);
        }

        // If the position to look at is the current position, simply return.
        if (visuals.position == target)
        {
            return;
        }

        // Look towards the target.
        Quaternion rotation = Visuals.rotation;

        target -= visuals.position;
        if (Quaternion.LookRotation(Vector3.RotateTowards(visuals.forward, target, float.MaxValue, 0)) == rotation)
        {
            return;
        }

        rotation = Quaternion.LookRotation(Vector3.RotateTowards(visuals.forward, target, lookSpeed * Time.deltaTime, 0.0f));
        Visuals.rotation = rotation;
    }

    protected virtual void Start()
    {
        Setup();
    }

    protected virtual void OnEnable()
    {
        try
        {
            AgentManager.Singleton.AddAgent(this);
        }
        catch { }
    }

    protected virtual void OnDisable()
    {
        try
        {
            AgentManager.Singleton.RemoveAgent(this);
        }
        catch { }
    }

    protected virtual void OnDestroy()
    {
        try
        {
            AgentManager.Singleton.RemoveAgent(this);
        }
        catch { }
    }

    /// <summary>
    /// Calculate how fast to move.
    /// </summary>
    /// <param name="deltaTime">The elapsed time step.</param>
    protected void CalculateMoveVelocity(float deltaTime)
    {
        Vector2 movement = Vector2.zero;
        float acceleration = moveAcceleration > 0 ? moveAcceleration : int.MaxValue;

        if (MovesData.Count > 0)
        {
            Vector3 positionVector3 = transform.position;
            Vector2 position = new Vector2(positionVector3.x, positionVector3.z);
            for (int i = 0; i < MovesData.Count; i++)
            {
                Vector2 target = MovesData[i].Position;

                if (IsCompleteMove(MovesData[i].MoveType, position, target))
                {
                    MovesData.RemoveAt(i--);
                    continue;
                }
            
                switch (MovesData[i].MoveType)
                {
                    case MoveType.Seek:
                        movement += Steering.Seek(position, MoveVelocity, target, acceleration);
                        break;
                    case MoveType.Flee:
                        movement += Steering.Flee(position, MoveVelocity, target, acceleration);
                        break;
                    case MoveType.Pursuit:
                        movement += Steering.Pursuit(position, MoveVelocity, target, MovesData[i].LastPosition, acceleration, deltaTime);
                        break;
                    case MoveType.Evade:
                        movement += Steering.Evade(position, MoveVelocity, target, MovesData[i].LastPosition, acceleration, deltaTime);
                        break;
                    case MoveType.Wander:
                        movement += Steering.Wander(transform, target, 1, 1,1);
                        break;
                }

                MovesData[i].LastPosition = target;
            }
        }

        if (movement == Vector2.zero)
        {
            MoveVelocity = Vector2.Lerp(MoveVelocity, Vector2.zero, acceleration * deltaTime);
            if (MoveVelocity.magnitude < restVelocity)
            {
                MoveVelocity = Vector2.zero;
            }
            return;
        }

        MoveVelocity += movement * deltaTime;
        if (MoveVelocity.magnitude > moveSpeed)
        {
            MoveVelocity = MoveVelocity.normalized * moveSpeed;
        }
    }

    private bool IsCompleteMove(MoveType moveType, Vector2 position, Vector2 target)
    {
        return (moveType == MoveType.Seek || moveType == MoveType.Pursuit) && Vector2.Distance(position, target) <= seekAcceptableDistance ||
               (moveType == MoveType.Flee || moveType == MoveType.Evade) && Vector2.Distance(position, target) >= fleeAcceptableDistance;
    }

    /// <summary>
    /// Handle receiving an event.
    /// </summary>
    /// <param name="aiEvent">The event to handle.</param>
    /// <returns>True if either the global state or normal state handles the event, false otherwise.</returns>
    private bool HandleEvent(AIEvent aiEvent)
    {
        return state != null && state.HandleEvent(this, aiEvent) || globalState != null && globalState.HandleEvent(this, aiEvent);
    }

    /// <summary>
    /// Read percepts from all the agent's sensors.
    /// </summary>
    private void Sense()
    {
        List<Percept> perceptsRead = new List<Percept>();
        int sensed = 0;
            
        // Read from every sensor.
        foreach (Sensor sensor in Sensors)
        {
            Percept percept = sensor.Read();
            if (percept == null)
            {
                continue;
            }

            AddMessage($"Perceived {percept} from sensor {sensor}.");
            perceptsRead.Add(percept);
            sensed++;
        }

        if (sensed == 0)
        {
            AddMessage("Did not perceive anything.");
        }
        else if (sensed > 1)
        {
            AddMessage($"Perceived {sensed} percepts.");
        }

        Percepts = perceptsRead.ToArray();
    }

    /// <summary>
    /// Perform actions.
    /// </summary>
    private void Act()
    {
        if (Actions == null || Actions.Length == 0)
        {
            AddMessage("Did not perform any actions.");
            return;
        }
            
        // Pass all actions to all actuators.
        foreach (Actuator actuator in Actuators)
        {
            actuator.Act(Actions);
        }

        foreach (Action action in Actions)
        {
            if (action.Complete)
            {
                AddMessage($"Completed action {action}.");
            }
        }

        // Remove actions which were completed.
        Actions = Actions.Where(a => !a.Complete).ToArray();
    }
        
    /// <summary>
    /// Link the performance measure to this agent.
    /// </summary>
    private void ConfigurePerformanceMeasure()
    {
        if (PerformanceMeasure != null)
        {
            PerformanceMeasure.Agent = this;
        }
    }
}