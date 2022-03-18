public class GridGenerator : NodeGenerator
{
    public override float SetNodeDistance()
    {
        return 1f / NodeArea.NodesPerStep;
    }

    public override void Generate()
    {
        for (int i = 0; i < NodeArea.RangeX; i++)
        {
            for (int j = 0; j < NodeArea.RangeZ; j++)
            {
                if (NodeArea.IsOpen(i, j))
                {
                    NodeArea.AddNode(i, j);
                }
            }
        }
    }
}