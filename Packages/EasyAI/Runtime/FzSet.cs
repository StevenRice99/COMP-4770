public class FzSet : FuzzyTerm
{
    public FzSet(FuzzySet fuzzySet)
    {
        Set = fuzzySet;
    }

    public FuzzySet Set { get; }

    public override FuzzyTerm Clone()
    {
        return new FzSet(Set);
    }

    public override float GetDom()
    {
        return Set.Dom;
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
        Set.OrWithDom(givenValue);
    }
}