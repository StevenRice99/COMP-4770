public abstract class NodeGenerator : NodeBase
{
    public LevelSection LevelSection { get; set; }

    public abstract void Generate();

    public virtual float SetNodeDistance()
    {
        return 0;
    }
}