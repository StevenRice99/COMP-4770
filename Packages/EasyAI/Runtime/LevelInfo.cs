using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    private const char Open = ' ';

    private const char Closed = '#';

    private const char Node = '*';
    
    private enum GenerationType : byte
    {
        None,
        Grid,
        CornerGraph
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
    [Min(1)]
    private int nodesPerStep = 1;

    [SerializeField]
    [Min(0)]
    private int cornerNodeSteps;

    [SerializeField]
    [Min(0)]
    private float nodeHeightOffset = 1;

    [SerializeField]
    private float nodeDistance;

    [SerializeField]
    private LayerMask groundLayers;

    private char[,] _data;

    private int RangeX => (pos1.x - pos2.x) * nodesPerStep + 1;
    
    private int RangeZ => (pos1.y - pos2.y) * nodesPerStep + 1;

    private readonly List<Vector3> _nodes = new List<Vector3>();

    private readonly List<Connection> _connections = new List<Connection>();

    private void Start()
    {
        Perform();
    }

    private void OnDrawGizmos()
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
        foreach (Connection connection in _connections)
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

        _data = new char[RangeX, RangeZ];
        
        for (int i = 0; i < RangeX; i++)
        {
            for (int j = 0; j < RangeZ; j++)
            {
                _data[i, j] = ScanOpen(i, j) ? Open : Closed;
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
            case GenerationType.CornerGraph:
                GenerateCornerGraph();
                break;
        }

        ConnectNodes();
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

        nodeDistance = 1f / nodesPerStep;
    }

    private void GenerateCornerGraph()
    {
        for (int i = cornerNodeSteps * 2; i < RangeX - cornerNodeSteps * 2; i++)
        {
            for (int j = cornerNodeSteps * 2; j < RangeZ - cornerNodeSteps * 2; j++)
            {
                if (_data[i, j] != Closed)
                {
                    continue;
                }
                
                // ++
                if (_data[i + 1, j] != Closed && _data[i, j + 1] != Closed)
                {
                    bool good = true;
                    for (int x = i + 1; x <= i + 1 + cornerNodeSteps * 2; x++)
                    {
                        for (int z = j + 1; z <= j + 1 + cornerNodeSteps * 2; z++)
                        {
                            if (_data[x, z] != Closed)
                            {
                                continue;
                            }

                            good = false;
                            break;
                        }

                        if (!good)
                        {
                            break;
                        }
                    }

                    if (good)
                    {
                        int posX = i + 1 + cornerNodeSteps;
                        int posY = j + 1 + cornerNodeSteps;
                        _data[posX, posY] = Node;
                        AddNode(posX, posY);
                    }
                }
                
                // +-
                if (_data[i + 1, j] != Closed && _data[i, j - 1] != Closed)
                {
                    bool good = true;
                    for (int x = i + 1; x <= i + 1 + cornerNodeSteps * 2; x++)
                    {
                        for (int z = j - 1; z >= j - 1 - cornerNodeSteps * 2; z--)
                        {
                            if (_data[x, z] != Closed)
                            {
                                continue;
                            }

                            good = false;
                            break;
                        }

                        if (!good)
                        {
                            break;
                        }
                    }

                    if (good)
                    {
                        int posX = i + 1 + cornerNodeSteps;
                        int posY = j - 1 - cornerNodeSteps;
                        _data[posX, posY] = Node;
                        AddNode(posX, posY);
                    }
                }
                
                // -+
                if (_data[i - 1, j] != Closed && _data[i, j + 1] != Closed)
                {
                    bool good = true;
                    for (int x = i - 1; x >= i - 1 - cornerNodeSteps * 2; x--)
                    {
                        for (int z = j + 1; z <= j + 1 + cornerNodeSteps * 2; z++)
                        {
                            if (_data[x, z] != Closed)
                            {
                                continue;
                            }

                            good = false;
                            break;
                        }

                        if (!good)
                        {
                            break;
                        }
                    }

                    if (good)
                    {
                        int posX = i - 1 - cornerNodeSteps;
                        int posY = j + 1 + cornerNodeSteps;
                        _data[posX, posY] = Node;
                        AddNode(posX, posY);
                    }
                }
                
                // --
                if (_data[i - 1, j] != Closed && _data[i, j - 1] != Closed)
                {
                    bool good = true;
                    for (int x = i - 1; x >= i - 1 - cornerNodeSteps * 2; x--)
                    {
                        for (int z = j - 1; z >= j - 1 - cornerNodeSteps * 2; z--)
                        {
                            if (_data[x, z] != Closed)
                            {
                                continue;
                            }

                            good = false;
                            break;
                        }

                        if (!good)
                        {
                            break;
                        }
                    }

                    if (good)
                    {
                        int posX = i - 1 - cornerNodeSteps;
                        int posY = j - 1 - cornerNodeSteps;
                        _data[posX, posY] = Node;
                        AddNode(posX, posY);
                    }
                }
            }
        }
    }

    private void ConnectNodes()
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            for (int j = 0; j < _nodes.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                float d = Vector3.Distance(_nodes[i], _nodes[j]);
                if (nodeDistance > 0 && d > nodeDistance)
                {
                    continue;
                }

                if (Physics.Linecast(_nodes[i], _nodes[j]))
                {
                    continue;
                }

                if (_connections.Any(c => c.A == _nodes[i] && c.B == _nodes[j] || c.A == _nodes[j] && c.B == _nodes[i]))
                {
                    continue;
                }
                
                _connections.Add(new Connection(_nodes[i], _nodes[j]));
            }
        }
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

    private bool IsOpen(int i, int j)
    {
        if (_data == null)
        {
            return false;
        }

        return _data[i, j] != Closed;
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
        if (!_nodes.Contains(v))
        {
            _nodes.Add(v);
        }
    }

    public override string ToString()
    {
        if (_data == null)
        {
            return "No data.";
        }

        string s = string.Empty;
        for (int i = 0; i < RangeX; i++)
        {
            for (int j = 0; j < RangeZ; j++)
            {
                s += _data[i, j];
            }

            if (i != RangeX - 1)
            {
                s += '\n';
            }
        }

        return s;
    }
}