using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NodeArea : NodeBase
{
    public const char Open = ' ';

    public const char Closed = '#';

    public const char Node = '*';
    
    public struct Connection
    {
        public readonly Vector3 A;
        public readonly Vector3 B;

        public Connection(Vector3 a, Vector3 b)
        {
            A = a;
            B = b;
        }
    }
    
    [SerializeField]
    [Tooltip("One of the corner coordinates (X, Z) of the area to generate nodes in.")]
    private int2 corner1 = new int2(5, 5);
    
    [SerializeField]
    [Tooltip("One of the corner coordinates (X, Z) of the area to generate nodes in.")]
    private int2 corner2 = new int2(-5, -5);

    [SerializeField]
    [Tooltip("How high up to start raycasts for node generation. Ensure this is above all level geometry.")]
    private float height = 10;

    [SerializeField]
    [Min(float.Epsilon)]
    [Tooltip("How far to raycast for node generation. Ensure this extends down past or equal to the level geometry.")]
    private float distance = 11;

    [SerializeField]
    [Min(1)]
    [Tooltip(
        "How many nodes to place for every unit of world space. Example values:\n." +
        "1 - Node per every 1 unit.\n" +
        "2 - Node per every 0.5 units.\n" +
        "4 - Node per every 0.25 units."
    )]
    private int nodesPerStep = 1;

    [SerializeField]
    [Min(0)]
    [Tooltip("How high off the ground set nodes for both visuals and raycasts for connections.")]
    private float nodeHeightOffset = 0.1f;

    [SerializeField]
    [Tooltip("Which layers can nodes be placed on.")]
    private LayerMask groundLayers;

    [SerializeField]
    [Min(0)]
    [Tooltip("How wide is the agent radius for connecting nodes to ensure enough space for movement.")]
    private float navigationRadius;

    public char[,] Data { get; private set; }

    public int RangeX => (corner1.x - corner2.x) * nodesPerStep + 1;
    
    public int RangeZ => (corner1.y - corner2.y) * nodesPerStep + 1;

    public int NodesPerStep => nodesPerStep;

    private float _nodeDistance;

    private void OnDrawGizmosSelected()
    {
        // Vertical lines.
        Gizmos.DrawLine(new Vector3(corner1.x, height, corner1.y), new Vector3(corner1.x, height - distance, corner1.y));
        Gizmos.DrawLine(new Vector3(corner1.x, height, corner2.y), new Vector3(corner1.x, height - distance, corner2.y));
        Gizmos.DrawLine(new Vector3(corner2.x, height, corner1.y), new Vector3(corner2.x, height - distance, corner1.y));
        Gizmos.DrawLine(new Vector3(corner2.x, height, corner2.y), new Vector3(corner2.x, height - distance, corner2.y));
        
        // Top horizontal lines.
        Gizmos.DrawLine(new Vector3(corner1.x, height, corner1.y), new Vector3(corner1.x, height, corner2.y));
        Gizmos.DrawLine(new Vector3(corner1.x, height, corner1.y), new Vector3(corner2.x, height, corner1.y));
        Gizmos.DrawLine(new Vector3(corner2.x, height, corner2.y), new Vector3(corner1.x, height, corner2.y));
        Gizmos.DrawLine(new Vector3(corner2.x, height, corner2.y), new Vector3(corner2.x, height, corner1.y));
        
        // Bottom horizontal lines.
        Gizmos.DrawLine(new Vector3(corner1.x, height - distance, corner1.y), new Vector3(corner1.x, height - distance, corner2.y));
        Gizmos.DrawLine(new Vector3(corner1.x, height - distance, corner1.y), new Vector3(corner2.x, height - distance, corner1.y));
        Gizmos.DrawLine(new Vector3(corner2.x, height - distance, corner2.y), new Vector3(corner1.x, height - distance, corner2.y));
        Gizmos.DrawLine(new Vector3(corner2.x, height - distance, corner2.y), new Vector3(corner2.x, height - distance, corner1.y));
    }

    public void Generate()
    {
        if (corner2.x > corner1.x)
        {
            (corner1.x, corner2.x) = (corner2.x, corner1.x);
        }
        
        if (corner2.y > corner1.y)
        {
            (corner1.y, corner2.y) = (corner2.y, corner1.y);
        }

        Data = new char[RangeX, RangeZ];
        
        for (int i = 0; i < RangeX; i++)
        {
            for (int j = 0; j < RangeZ; j++)
            {
                Data[i, j] = ScanOpen(i, j) ? Open : Closed;
            }
        }
        
        List<NodeGenerator> generators = GetComponents<NodeGenerator>().ToList();
        generators.AddRange(GetComponentsInChildren<NodeGenerator>());
        NodeGenerator generator = generators.FirstOrDefault(g => g.enabled);
        if (generator != null)
        {
            generator.NodeArea = this;
            _nodeDistance = generator.SetNodeDistance();
            generator.Generate();

            for (int i = 0; i < AgentManager.Singleton.nodes.Count; i++)
            {
                for (int j = 0; j < AgentManager.Singleton.nodes.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    float d = Vector3.Distance(AgentManager.Singleton.nodes[i], AgentManager.Singleton.nodes[j]);
                    if (_nodeDistance > 0 && d > _nodeDistance)
                    {
                        continue;
                    }

                    if (navigationRadius <= 0)
                    {
                        if (Physics.Linecast(AgentManager.Singleton.nodes[i], AgentManager.Singleton.nodes[j]))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Vector3 p1 = AgentManager.Singleton.nodes[i];
                        p1.y += navigationRadius / 2;
                        Vector3 p2 = AgentManager.Singleton.nodes[j];
                        p2.y += navigationRadius / 2;
                        Vector3 direction = (p2 - p1).normalized;
                        if (Physics.SphereCast(p1, navigationRadius, direction, out _, d))
                        {
                            continue;
                        }
                    }

                    if (AgentManager.Singleton.connections.Any(c => c.A == AgentManager.Singleton.nodes[i] && c.B == AgentManager.Singleton.nodes[j] || c.A == AgentManager.Singleton.nodes[j] && c.B == AgentManager.Singleton.nodes[i]))
                    {
                        continue;
                    }
                
                    AgentManager.Singleton.connections.Add(new Connection(AgentManager.Singleton.nodes[i], AgentManager.Singleton.nodes[j]));
                }
            }
        }

        foreach (NodeGenerator g in generators)
        {
            g.Finish();
        }

        const string folder = "Maps";

        if (!Directory.Exists(folder))
        {
            DirectoryInfo info = Directory.CreateDirectory(folder);
            if (!info.Exists)
            {
                Finish();
                return;
            }
        }
        
        string fileName = $"{folder}/{SceneManager.GetActiveScene().name}";
        NodeArea[] levelSections = FindObjectsOfType<NodeArea>();
        if (levelSections.Length > 1)
        {
            fileName += $"_{levelSections.ToList().IndexOf(this)}";
        }
        fileName += ".txt";
        
        StreamWriter writer = new StreamWriter(fileName, false);
        writer.Write(ToString());
        writer.Close();
        
        Finish();
    }

    private float2 GetRealPosition(int i, int j)
    {
        return new float2(corner2.x + i * 1f / nodesPerStep, corner2.y + j * 1f / nodesPerStep);
    }

    private bool ScanOpen(int i, int j)
    {
        float2 pos = GetRealPosition(i, j);
        if (Physics.Raycast(new Vector3(pos.x, height, pos.y), Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            return (groundLayers.value & (1 << hit.transform.gameObject.layer)) > 0;
        }

        return false;
    }

    public bool IsOpen(int i, int j)
    {
        if (Data == null)
        {
            return false;
        }

        return Data[i, j] != Closed;
    }

    public void AddNode(int i, int j)
    {
        float2 pos = GetRealPosition(i, j);
        float y = height;
        if (Physics.Raycast(new Vector3(pos.x, height, pos.y), Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayers))
        {
            y = hit.point.y;
        }

        y += nodeHeightOffset;
        
        Vector3 v = new Vector3(pos.x, y, pos.y);
        if (!AgentManager.Singleton.nodes.Contains(v))
        {
            AgentManager.Singleton.nodes.Add(v);
        }
    }

    public override string ToString()
    {
        if (Data == null)
        {
            return "No data.";
        }

        string s = string.Empty;
        for (int i = 0; i < RangeX; i++)
        {
            for (int j = 0; j < RangeZ; j++)
            {
                s += Data[i, j];
            }

            if (i != RangeX - 1)
            {
                s += '\n';
            }
        }

        return s;
    }
}