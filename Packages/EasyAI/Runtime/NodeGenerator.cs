public abstract class NodeGenerator : NodeBase
{
    protected LevelInfo levelInfo;

    private void Start()
    {
        levelInfo = GetComponent<LevelInfo>();
        if (levelInfo == null)
        {
            Finish();
        }
    }

    public abstract void Generate();

    public virtual float SetNodeDistance()
    {
        return 0;
    }
}