public class FzVery : FuzzyTerm
{
    public FzVery(FzSet fzSet)
    {
        Set = fzSet.Set;
    }

    public FzVery(FzVery fzVery)
    {
        Set = fzVery.Set;
    }

    public FuzzySet Set { get; }

    public override FuzzyTerm Clone()
    {
        return new FzVery(this);
    }

    public override float GetDom()
    {
        return Set.Dom * Set.Dom;
    }

    public override void ClearDom()
    {
        Set.ClearDom();
    }

    /// <summary>
    /// Method for updating the DOM of a consequent when a rule fires.
    /// </summary>
    /// <param name="givenValue">Given value.</param>
    public override void OrWithDom(float givenValue)
    {
        Set.OrWithDom(givenValue * givenValue);
    }
}