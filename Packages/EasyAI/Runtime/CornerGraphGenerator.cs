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
        for (int i = cornerNodeSteps * 2; i < levelInfo.RangeX - cornerNodeSteps * 2; i++)
        {
            for (int j = cornerNodeSteps * 2; j < levelInfo.RangeZ - cornerNodeSteps * 2; j++)
            {
                if (levelInfo.Data[i, j] != LevelInfo.Closed)
                {
                    continue;
                }
                
                // ++
                if (levelInfo.Data[i + 1, j] != LevelInfo.Closed && levelInfo.Data[i, j + 1] != LevelInfo.Closed)
                {
                    bool good = true;
                    for (int x = i + 1; x <= i + 1 + cornerNodeSteps * 2; x++)
                    {
                        for (int z = j + 1; z <= j + 1 + cornerNodeSteps * 2; z++)
                        {
                            if (levelInfo.Data[x, z] != LevelInfo.Closed)
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
                        levelInfo.Data[posX, posY] = LevelInfo.Node;
                        levelInfo.AddNode(posX, posY);
                    }
                }
                
                // +-
                if (levelInfo.Data[i + 1, j] != LevelInfo.Closed && levelInfo.Data[i, j - 1] != LevelInfo.Closed)
                {
                    bool good = true;
                    for (int x = i + 1; x <= i + 1 + cornerNodeSteps * 2; x++)
                    {
                        for (int z = j - 1; z >= j - 1 - cornerNodeSteps * 2; z--)
                        {
                            if (levelInfo.Data[x, z] != LevelInfo.Closed)
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
                        levelInfo.Data[posX, posY] = LevelInfo.Node;
                        levelInfo.AddNode(posX, posY);
                    }
                }
                
                // -+
                if (levelInfo.Data[i - 1, j] != LevelInfo.Closed && levelInfo.Data[i, j + 1] != LevelInfo.Closed)
                {
                    bool good = true;
                    for (int x = i - 1; x >= i - 1 - cornerNodeSteps * 2; x--)
                    {
                        for (int z = j + 1; z <= j + 1 + cornerNodeSteps * 2; z++)
                        {
                            if (levelInfo.Data[x, z] != LevelInfo.Closed)
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
                        levelInfo.Data[posX, posY] = LevelInfo.Node;
                        levelInfo.AddNode(posX, posY);
                    }
                }
                
                // --
                if (levelInfo.Data[i - 1, j] != LevelInfo.Closed && levelInfo.Data[i, j - 1] != LevelInfo.Closed)
                {
                    bool good = true;
                    for (int x = i - 1; x >= i - 1 - cornerNodeSteps * 2; x--)
                    {
                        for (int z = j - 1; z >= j - 1 - cornerNodeSteps * 2; z--)
                        {
                            if (levelInfo.Data[x, z] != LevelInfo.Closed)
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
                        levelInfo.Data[posX, posY] = LevelInfo.Node;
                        levelInfo.AddNode(posX, posY);
                    }
                }
            }
        }
    }
}