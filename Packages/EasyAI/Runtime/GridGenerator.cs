public class GridGenerator : NodeGenerator
{
    public override float SetNodeDistance()
    {
        return 1f / levelInfo.NodesPerStep;
    }

    public override void Generate()
    {
        for (int i = 0; i < levelInfo.RangeX; i++)
        {
            for (int j = 0; j < levelInfo.RangeZ; j++)
            {
                if (levelInfo.IsOpen(i, j))
                {
                    levelInfo.AddNode(i, j);
                }
            }
        }
    }
}