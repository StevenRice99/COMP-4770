public abstract class NodeGenerator : NodeBase
{
    public NodeArea NodeArea { get; set; }

    public abstract void Generate();

    public virtual float SetNodeDistance()
    {
        return 0;
    }
}