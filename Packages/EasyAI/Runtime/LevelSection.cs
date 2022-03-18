using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSection : NodeBase
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
    private int2 pos1 = new int2(5, 5);
        
    [SerializeField]
    private int2 pos2 = new int2(-5, -5);

    [SerializeField]
    private float height = 10;

    [SerializeField]
    [Min(float.Epsilon)]
    private float distance = 11;

    [SerializeField]
    [Min(1)]
    private int nodesPerStep = 1;

    [SerializeField]
    [Min(0)]
    private float nodeHeightOffset = 0.1f;

    [SerializeField]
    private LayerMask groundLayers;

    public char[,] Data { get; set; }

    public int RangeX => (pos1.x - pos2.x) * nodesPerStep + 1;
    
    public int RangeZ => (pos1.y - pos2.y) * nodesPerStep + 1;

    public int NodesPerStep => nodesPerStep;

    private float _nodeDistance;

    private void OnDrawGizmosSelected()
    {
        // Vertical lines.
        Gizmos.DrawLine(new Vector3(pos1.x, height, pos1.y), new Vector3(pos1.x, height - distance, pos1.y));
        Gizmos.DrawLine(new Vector3(pos1.x, height, pos2.y), new Vector3(pos1.x, height - distance, pos2.y));
        Gizmos.DrawLine(new Vector3(pos2.x, height, pos1.y), new Vector3(pos2.x, height - distance, pos1.y));
        Gizmos.DrawLine(new Vector3(pos2.x, height, pos2.y), new Vector3(pos2.x, height - distance, pos2.y));
        
        // Top horizontal lines.
        Gizmos.DrawLine(new Vector3(pos1.x, height, pos1.y), new Vector3(pos1.x, height, pos2.y));
        Gizmos.DrawLine(new Vector3(pos1.x, height, pos1.y), new Vector3(pos2.x, height, pos1.y));
        Gizmos.DrawLine(new Vector3(pos2.x, height, pos2.y), new Vector3(pos1.x, height, pos2.y));
        Gizmos.DrawLine(new Vector3(pos2.x, height, pos2.y), new Vector3(pos2.x, height, pos1.y));
        
        // Bottom horizontal lines.
        Gizmos.DrawLine(new Vector3(pos1.x, height - distance, pos1.y), new Vector3(pos1.x, height - distance, pos2.y));
        Gizmos.DrawLine(new Vector3(pos1.x, height - distance, pos1.y), new Vector3(pos2.x, height - distance, pos1.y));
        Gizmos.DrawLine(new Vector3(pos2.x, height - distance, pos2.y), new Vector3(pos1.x, height - distance, pos2.y));
        Gizmos.DrawLine(new Vector3(pos2.x, height - distance, pos2.y), new Vector3(pos2.x, height - distance, pos1.y));
    }

    public void Generate()
    {
        if (pos2.x > pos1.x)
        {
            (pos1.x, pos2.x) = (pos2.x, pos1.x);
        }
        
        if (pos2.y > pos1.y)
        {
            (pos1.y, pos2.y) = (pos2.y, pos1.y);
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
            generator.LevelSection = this;
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

                    if (Physics.Linecast(AgentManager.Singleton.nodes[i], AgentManager.Singleton.nodes[j]))
                    {
                        continue;
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
        LevelSection[] levelSections = FindObjectsOfType<LevelSection>();
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
        return new float2(pos2.x + i * 1f / nodesPerStep, pos2.y + j * 1f / nodesPerStep);
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