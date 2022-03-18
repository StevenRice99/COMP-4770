public class GridGenerator : NodeGenerator
{
    public override float SetNodeDistance()
    {
        return 1f / LevelSection.NodesPerStep;
    }

    public override void Generate()
    {
        for (int i = 0; i < LevelSection.RangeX; i++)
        {
            for (int j = 0; j < LevelSection.RangeZ; j++)
            {
                if (LevelSection.IsOpen(i, j))
                {
                    LevelSection.AddNode(i, j);
                }
            }
        }
    }
}