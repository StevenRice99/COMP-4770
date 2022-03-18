public class GridGenerator : NodeGenerator
{
    public override float SetNodeDistance()
    {
        return 1f / NodeArea.NodesPerStep;
    }

    public override void Generate()
    {
        for (int x = 0; x < NodeArea.RangeX; x++)
        {
            for (int z = 0; z < NodeArea.RangeZ; z++)
            {
                if (NodeArea.IsOpen(x, z))
                {
                    NodeArea.AddNode(x, z);
                }
            }
        }
    }
}