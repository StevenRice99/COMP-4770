using UnityEngine;

public class FzFairly : FuzzyTerm
{
    public FzFairly(FzSet fuzzyTerm)
    {
        Set = fuzzyTerm.Set;
    }

    public FzFairly(FzFairly fzFairly)
    {
        Set = fzFairly.Set;
    }

    public FuzzySet Set { get; }

    public override float GetDom()
    {
        return Mathf.Sqrt(Set.Dom);
    }

    public override FuzzyTerm Clone()
    {
        return new FzFairly(this);
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
        Set.OrWithDom(Mathf.Sqrt(givenValue));
    }
}