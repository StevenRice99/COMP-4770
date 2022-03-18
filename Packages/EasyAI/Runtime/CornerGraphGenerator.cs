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
        for (int i = cornerNodeSteps * 2; i < NodeArea.RangeX - cornerNodeSteps * 2; i++)
        {
            for (int j = cornerNodeSteps * 2; j < NodeArea.RangeZ - cornerNodeSteps * 2; j++)
            {
                if (NodeArea.Data[i, j] != NodeArea.Closed)
                {
                    continue;
                }
                
                // ++
                if (NodeArea.Data[i + 1, j] != NodeArea.Closed && NodeArea.Data[i, j + 1] != NodeArea.Closed)
                {
                    bool good = true;
                    for (int x = i + 1; x <= i + 1 + cornerNodeSteps * 2; x++)
                    {
                        for (int z = j + 1; z <= j + 1 + cornerNodeSteps * 2; z++)
                        {
                            if (NodeArea.Data[x, z] != NodeArea.Closed)
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
                        NodeArea.Data[posX, posY] = NodeArea.Node;
                        NodeArea.AddNode(posX, posY);
                    }
                }
                
                // +-
                if (NodeArea.Data[i + 1, j] != NodeArea.Closed && NodeArea.Data[i, j - 1] != NodeArea.Closed)
                {
                    bool good = true;
                    for (int x = i + 1; x <= i + 1 + cornerNodeSteps * 2; x++)
                    {
                        for (int z = j - 1; z >= j - 1 - cornerNodeSteps * 2; z--)
                        {
                            if (NodeArea.Data[x, z] != NodeArea.Closed)
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
                        NodeArea.Data[posX, posY] = NodeArea.Node;
                        NodeArea.AddNode(posX, posY);
                    }
                }
                
                // -+
                if (NodeArea.Data[i - 1, j] != NodeArea.Closed && NodeArea.Data[i, j + 1] != NodeArea.Closed)
                {
                    bool good = true;
                    for (int x = i - 1; x >= i - 1 - cornerNodeSteps * 2; x--)
                    {
                        for (int z = j + 1; z <= j + 1 + cornerNodeSteps * 2; z++)
                        {
                            if (NodeArea.Data[x, z] != NodeArea.Closed)
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
                        NodeArea.Data[posX, posY] = NodeArea.Node;
                        NodeArea.AddNode(posX, posY);
                    }
                }
                
                // --
                if (NodeArea.Data[i - 1, j] != NodeArea.Closed && NodeArea.Data[i, j - 1] != NodeArea.Closed)
                {
                    bool good = true;
                    for (int x = i - 1; x >= i - 1 - cornerNodeSteps * 2; x--)
                    {
                        for (int z = j - 1; z >= j - 1 - cornerNodeSteps * 2; z--)
                        {
                            if (NodeArea.Data[x, z] != NodeArea.Closed)
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
                        NodeArea.Data[posX, posY] = NodeArea.Node;
                        NodeArea.AddNode(posX, posY);
                    }
                }
            }
        }
    }
}