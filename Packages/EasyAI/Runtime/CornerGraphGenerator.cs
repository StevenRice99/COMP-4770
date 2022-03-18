using UnityEngine;

public class CornerGraphGenerator : NodeGenerator
{
    [SerializeField]
    [Tooltip("How far away can nodes connect to each other from. Setting to zero means there is no limit.")]
    private float nodeDistance;
    
    [SerializeField]
    [Min(0)]
    [Tooltip("How far away from corners should the nodes be placed.")]
    private int cornerNodeSteps;
    
    public override float SetNodeDistance()
    {
        return nodeDistance;
    }
    
    public override void Generate()
    {
        for (int x = cornerNodeSteps * 2; x < NodeArea.RangeX - cornerNodeSteps * 2; x++)
        {
            for (int z = cornerNodeSteps * 2; z < NodeArea.RangeZ - cornerNodeSteps * 2; z++)
            {
                if (NodeArea.IsOpen(x, z))
                {
                    continue;
                }
                
                // ++
                if (NodeArea.IsOpen(x + 1, z) && NodeArea.IsOpen(x, z + 1))
                {
                    bool good = true;
                    for (int x1 = x + 1; x1 <= x + 1 + cornerNodeSteps * 2; x1++)
                    {
                        for (int z1 = z + 1; z1 <= z + 1 + cornerNodeSteps * 2; z1++)
                        {
                            if (NodeArea.IsOpen(x1, z1))
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
                        NodeArea.AddNode(x + 1 + cornerNodeSteps, z + 1 + cornerNodeSteps);
                    }
                }
                
                // +-
                if (NodeArea.IsOpen(x + 1, z) && NodeArea.IsOpen(x, z - 1))
                {
                    bool good = true;
                    for (int x1 = x + 1; x1 <= x + 1 + cornerNodeSteps * 2; x1++)
                    {
                        for (int z1 = z - 1; z1 >= z - 1 - cornerNodeSteps * 2; z1--)
                        {
                            if (NodeArea.IsOpen(x1, z1))
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
                        NodeArea.AddNode(x + 1 + cornerNodeSteps, z - 1 - cornerNodeSteps);
                    }
                }
                
                // -+
                if (NodeArea.IsOpen(x - 1, z) && NodeArea.IsOpen(x, z + 1))
                {
                    bool good = true;
                    for (int x1 = x - 1; x1 >= x - 1 - cornerNodeSteps * 2; x1--)
                    {
                        for (int z1 = z + 1; z1 <= z + 1 + cornerNodeSteps * 2; z1++)
                        {
                            if (NodeArea.IsOpen(x1, z1))
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
                        NodeArea.AddNode(x - 1 - cornerNodeSteps, z + 1 + cornerNodeSteps);
                    }
                }
                
                // --
                if (NodeArea.IsOpen(x - 1, z) && NodeArea.IsOpen(x, z - 1))
                {
                    bool good = true;
                    for (int x1 = x - 1; x1 >= x - 1 - cornerNodeSteps * 2; x1--)
                    {
                        for (int z1 = z - 1; z1 >= z - 1 - cornerNodeSteps * 2; z1--)
                        {
                            if (NodeArea.IsOpen(x1, z1))
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
                        NodeArea.AddNode(x - 1 - cornerNodeSteps, z - 1 - cornerNodeSteps);
                    }
                }
            }
        }
    }
}