public class GridGenerator : NodeGenerator
{
    public override float SetNodeDistance()
    {
        return 1f / levelSection.NodesPerStep;
    }

    public override void Generate()
    {
        for (int i = 0; i < levelSection.RangeX; i++)
        {
            for (int j = 0; j < levelSection.RangeZ; j++)
            {
                if (levelSection.IsOpen(i, j))
                {
                    levelSection.AddNode(i, j);
                }
            }
        }
    }
}