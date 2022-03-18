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
        for (int i = cornerNodeSteps * 2; i < levelSection.RangeX - cornerNodeSteps * 2; i++)
        {
            for (int j = cornerNodeSteps * 2; j < levelSection.RangeZ - cornerNodeSteps * 2; j++)
            {
                if (levelSection.Data[i, j] != LevelSection.Closed)
                {
                    continue;
                }
                
                // ++
                if (levelSection.Data[i + 1, j] != LevelSection.Closed && levelSection.Data[i, j + 1] != LevelSection.Closed)
                {
                    bool good = true;
                    for (int x = i + 1; x <= i + 1 + cornerNodeSteps * 2; x++)
                    {
                        for (int z = j + 1; z <= j + 1 + cornerNodeSteps * 2; z++)
                        {
                            if (levelSection.Data[x, z] != LevelSection.Closed)
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
                        levelSection.Data[posX, posY] = LevelSection.Node;
                        levelSection.AddNode(posX, posY);
                    }
                }
                
                // +-
                if (levelSection.Data[i + 1, j] != LevelSection.Closed && levelSection.Data[i, j - 1] != LevelSection.Closed)
                {
                    bool good = true;
                    for (int x = i + 1; x <= i + 1 + cornerNodeSteps * 2; x++)
                    {
                        for (int z = j - 1; z >= j - 1 - cornerNodeSteps * 2; z--)
                        {
                            if (levelSection.Data[x, z] != LevelSection.Closed)
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
                        levelSection.Data[posX, posY] = LevelSection.Node;
                        levelSection.AddNode(posX, posY);
                    }
                }
                
                // -+
                if (levelSection.Data[i - 1, j] != LevelSection.Closed && levelSection.Data[i, j + 1] != LevelSection.Closed)
                {
                    bool good = true;
                    for (int x = i - 1; x >= i - 1 - cornerNodeSteps * 2; x--)
                    {
                        for (int z = j + 1; z <= j + 1 + cornerNodeSteps * 2; z++)
                        {
                            if (levelSection.Data[x, z] != LevelSection.Closed)
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
                        levelSection.Data[posX, posY] = LevelSection.Node;
                        levelSection.AddNode(posX, posY);
                    }
                }
                
                // --
                if (levelSection.Data[i - 1, j] != LevelSection.Closed && levelSection.Data[i, j - 1] != LevelSection.Closed)
                {
                    bool good = true;
                    for (int x = i - 1; x >= i - 1 - cornerNodeSteps * 2; x--)
                    {
                        for (int z = j - 1; z >= j - 1 - cornerNodeSteps * 2; z--)
                        {
                            if (levelSection.Data[x, z] != LevelSection.Closed)
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
                        levelSection.Data[posX, posY] = LevelSection.Node;
                        levelSection.AddNode(posX, posY);
                    }
                }
            }
        }
    }
}