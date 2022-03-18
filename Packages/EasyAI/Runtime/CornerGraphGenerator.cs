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
                
                UpperUpper(x, z);
                UpperLower(x, z);
                LowerUpper(x, z);
                LowerLower(x, z);
            }
        }
    }

    private void UpperUpper(int x, int z)
    {
        if (!NodeArea.IsOpen(x + 1, z) || !NodeArea.IsOpen(x, z + 1))
        {
            return;
        }
        
        for (int x1 = x + 1; x1 <= x + 1 + cornerNodeSteps * 2; x1++)
        {
            for (int z1 = z + 1; z1 <= z + 1 + cornerNodeSteps * 2; z1++)
            {
                if (!NodeArea.IsOpen(x1, z1))
                {
                    return;
                }
            }
        }

        NodeArea.AddNode(x + 1 + cornerNodeSteps, z + 1 + cornerNodeSteps);
    }

    private void UpperLower(int x, int z)
    {
        if (!NodeArea.IsOpen(x + 1, z) || !NodeArea.IsOpen(x, z - 1))
        {
            return;
        }

        for (int x1 = x + 1; x1 <= x + 1 + cornerNodeSteps * 2; x1++)
        {
            for (int z1 = z - 1; z1 >= z - 1 - cornerNodeSteps * 2; z1--)
            {
                if (!NodeArea.IsOpen(x1, z1))
                {
                    return;
                }
            }
        }

        NodeArea.AddNode(x + 1 + cornerNodeSteps, z - 1 - cornerNodeSteps);
    }

    private void LowerUpper(int x, int z)
    {
        if (!NodeArea.IsOpen(x - 1, z) || !NodeArea.IsOpen(x, z + 1))
        {
            return;
        }
        
        for (int x1 = x - 1; x1 >= x - 1 - cornerNodeSteps * 2; x1--)
        {
            for (int z1 = z + 1; z1 <= z + 1 + cornerNodeSteps * 2; z1++)
            {
                if (!NodeArea.IsOpen(x1, z1))
                {
                    return;
                }
            }
        }

        NodeArea.AddNode(x - 1 - cornerNodeSteps, z + 1 + cornerNodeSteps);
    }

    private void LowerLower(int x, int z)
    {
        if (!NodeArea.IsOpen(x - 1, z) || !NodeArea.IsOpen(x, z - 1))
        {
            return;
        }
        
        for (int x1 = x - 1; x1 >= x - 1 - cornerNodeSteps * 2; x1--)
        {
            for (int z1 = z - 1; z1 >= z - 1 - cornerNodeSteps * 2; z1--)
            {
                if (!NodeArea.IsOpen(x1, z1))
                {
                    return;
                }
            }
        }

        NodeArea.AddNode(x - 1 - cornerNodeSteps, z - 1 - cornerNodeSteps);
    }
}