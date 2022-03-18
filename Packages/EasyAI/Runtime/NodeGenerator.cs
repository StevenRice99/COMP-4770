public abstract class NodeGenerator : NodeBase
{
    protected LevelSection levelSection;

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        levelSection = GetComponent<LevelSection>();
        if (levelSection == null)
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