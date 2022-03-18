using UnityEngine;

public class CornerGraphGenerator : NodeGenerator
{
    [SerializeField]
    private float nodeDistance;
    
    [SerializeField]
    [Min(0)]
    private int cornerNodeSteps;
    
    public override float SetNodeDistance()
    {
        return nodeDistance;
    }
    
    public override void Generate()
    {
        for (int i = cornerNodeSteps * 2; i < LevelSection.RangeX - cornerNodeSteps * 2; i++)
        {
            for (int j = cornerNodeSteps * 2; j < LevelSection.RangeZ - cornerNodeSteps * 2; j++)
            {
                if (LevelSection.Data[i, j] != LevelSection.Closed)
                {
                    continue;
                }
                
                // ++
                if (LevelSection.Data[i + 1, j] != LevelSection.Closed && LevelSection.Data[i, j + 1] != LevelSection.Closed)
                {
                    bool good = true;
                    for (int x = i + 1; x <= i + 1 + cornerNodeSteps * 2; x++)
                    {
                        for (int z = j + 1; z <= j + 1 + cornerNodeSteps * 2; z++)
                        {
                            if (LevelSection.Data[x, z] != LevelSection.Closed)
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
                        LevelSection.Data[posX, posY] = LevelSection.Node;
                        LevelSection.AddNode(posX, posY);
                    }
                }
                
                // +-
                if (LevelSection.Data[i + 1, j] != LevelSection.Closed && LevelSection.Data[i, j - 1] != LevelSection.Closed)
                {
                    bool good = true;
                    for (int x = i + 1; x <= i + 1 + cornerNodeSteps * 2; x++)
                    {
                        for (int z = j - 1; z >= j - 1 - cornerNodeSteps * 2; z--)
                        {
                            if (LevelSection.Data[x, z] != LevelSection.Closed)
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
                        LevelSection.Data[posX, posY] = LevelSection.Node;
                        LevelSection.AddNode(posX, posY);
                    }
                }
                
                // -+
                if (LevelSection.Data[i - 1, j] != LevelSection.Closed && LevelSection.Data[i, j + 1] != LevelSection.Closed)
                {
                    bool good = true;
                    for (int x = i - 1; x >= i - 1 - cornerNodeSteps * 2; x--)
                    {
                        for (int z = j + 1; z <= j + 1 + cornerNodeSteps * 2; z++)
                        {
                            if (LevelSection.Data[x, z] != LevelSection.Closed)
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
                        LevelSection.Data[posX, posY] = LevelSection.Node;
                        LevelSection.AddNode(posX, posY);
                    }
                }
                
                // --
                if (LevelSection.Data[i - 1, j] != LevelSection.Closed && LevelSection.Data[i, j - 1] != LevelSection.Closed)
                {
                    bool good = true;
                    for (int x = i - 1; x >= i - 1 - cornerNodeSteps * 2; x--)
                    {
                        for (int z = j - 1; z >= j - 1 - cornerNodeSteps * 2; z--)
                        {
                            if (LevelSection.Data[x, z] != LevelSection.Closed)
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
                        LevelSection.Data[posX, posY] = LevelSection.Node;
                        LevelSection.AddNode(posX, posY);
                    }
                }
            }
        }
    }
}