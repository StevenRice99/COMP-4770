using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    private const char Open = ' ';

    private const char Closed = '#';
    
    private enum GenerationType : byte
    {
        None,
        Grid
    }
    
    private struct Connection
    {
        public Vector3 A;
        public Vector3 B;

        public Connection(Vector3 a, Vector3 b)
        {
            A = a;
            B = b;
        }
    }

    [SerializeField]
    private GenerationType type;
    
    [SerializeField]
    private int2 pos1;
        
    [SerializeField]
    private int2 pos2;

    [SerializeField]
    private float height;

    [SerializeField]
    [Min(float.Epsilon)]
    private float distance;

    [SerializeField]
    private int2 nodesPerStep = new int2(4, 4);

    [SerializeField]
    [Min(0)]
    private float nodeHeightOffset = 1;

    [SerializeField]
    private float nodeDistance;

    [SerializeField]
    private LayerMask groundLayers;

    private char[,] data;

    private int RangeX => (pos1.x - pos2.x) * nodesPerStep.x;
    
    private int RangeZ => (pos1.y - pos2.y) * nodesPerStep.y;

    private readonly List<Vector3> nodes = new List<Vector3>();

    private readonly List<Connection> connections = new List<Connection>();

    private void Start()
    {
        Perform();
    }

    private void OnDrawGizmosSelected()
    {
        // VERTICAL
        Gizmos.DrawLine(new Vector3(pos1.x, height, pos1.y), new Vector3(pos1.x, height - distance, pos1.y));
        Gizmos.DrawLine(new Vector3(pos1.x, height, pos2.y), new Vector3(pos1.x, height - distance, pos2.y));
        Gizmos.DrawLine(new Vector3(pos2.x, height, pos1.y), new Vector3(pos2.x, height - distance, pos1.y));
        Gizmos.DrawLine(new Vector3(pos2.x, height, pos2.y), new Vector3(pos2.x, height - distance, pos2.y));
        
        // TOP
        Gizmos.DrawLine(new Vector3(pos1.x, height, pos1.y), new Vector3(pos1.x, height, pos2.y));
        Gizmos.DrawLine(new Vector3(pos1.x, height, pos1.y), new Vector3(pos2.x, height, pos1.y));
        Gizmos.DrawLine(new Vector3(pos2.x, height, pos2.y), new Vector3(pos1.x, height, pos2.y));
        Gizmos.DrawLine(new Vector3(pos2.x, height, pos2.y), new Vector3(pos2.x, height, pos1.y));
        
        // BOTTOM
        Gizmos.DrawLine(new Vector3(pos1.x, height - distance, pos1.y), new Vector3(pos1.x, height - distance, pos2.y));
        Gizmos.DrawLine(new Vector3(pos1.x, height - distance, pos1.y), new Vector3(pos2.x, height - distance, pos1.y));
        Gizmos.DrawLine(new Vector3(pos2.x, height - distance, pos2.y), new Vector3(pos1.x, height - distance, pos2.y));
        Gizmos.DrawLine(new Vector3(pos2.x, height - distance, pos2.y), new Vector3(pos2.x, height - distance, pos1.y));
        
        // CONNECTIONS
        Gizmos.color = Color.green;
        foreach (Connection connection in connections)
        {
            Gizmos.DrawLine(connection.A, connection.B);
        }
    }

    private void Perform()
    {
        Read();
        Generate();
        
        StreamWriter writer = new StreamWriter("MapData.txt", false);
        writer.Write(ToString());
        writer.Close();
    }

    private void Read()
    {
        if (pos2.x > pos1.x)
        {
            (pos1.x, pos2.x) = (pos2.x, pos1.x);
        }
        
        if (pos2.y > pos1.y)
        {
            (pos1.y, pos2.y) = (pos2.y, pos1.y);
        }

        data = new char[RangeX, RangeZ];
        
        for (int i = 0; i < RangeX; i++)
        {
            for (int j = 0; j < RangeZ; j++)
            {
                data[i, j] = ScanOpen(i, j) ? Open : Closed;
            }
        }
    }

    private void Generate()
    {
        switch (type)
        {
            case GenerationType.None:
                return;
            case GenerationType.Grid:
                GenerateGrid();
                break;
        }

        ConnectNodes();
    }

    private void ConnectNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                if (connections.Any(c => c.A == nodes[i] && c.B == nodes[j] || c.A == nodes[j] && c.B == nodes[i]))
                {
                    continue;
                }

                if (nodeDistance > 0 && Vector3.Distance(nodes[i], nodes[j]) > nodeDistance)
                {
                    continue;
                }

                if (Physics.Raycast(nodes[i], nodes[i] - nodes[j], out RaycastHit _, Mathf.Infinity))
                {
                    continue;
                }
                
                connections.Add(new Connection(nodes[i], nodes[j]));
            }
        }
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < RangeX; i++)
        {
            for (int j = 0; j < RangeZ; j++)
            {
                if (IsOpen(i, j))
                {
                    AddNode(i, j);
                }
            }
        }
    }

    private float2 GetRealPosition(int i, int j)
    {
        /*
        int baseX = i / nodesPerStep.x;
        int baseZ = j / nodesPerStep.y;
        float multiX = (float) i / nodesPerStep.x - baseX;
        float multiZ = (float) i / nodesPerStep.y - baseZ;
        float2 pos = new float2(pos2.x + baseX + 1f / nodesPerStep.x * multiX, pos2.y + baseZ + 1f / nodesPerStep.y * multiZ);
        Debug.Log($"Index = ({i} , {j}) | Base = ({baseX}, {baseZ}) | Position = ({pos.x}, {pos.y})");
        return pos;
        */
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

    private bool IsOpen(int i, int j)
    {
        if (data == null)
        {
            return false;
        }

        return data[i, j] != Closed;
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
        if (!nodes.Contains(v))
        {
            nodes.Add(v);
        }
    }

    public override string ToString()
    {
        if (data == null)
        {
            return "No data.";
        }

        string s = string.Empty;
        for (int i = 0; i < RangeX; i++)
        {
            for (int j = 0; j < RangeZ; j++)
            {
                s += data[i, j];
            }

            if (i != RangeX - 1)
            {
                s += '\n';
            }
        }

        return s;
    }
}