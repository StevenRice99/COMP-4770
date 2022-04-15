using System.Collections.Generic;
using System.Linq;

public class FzOr : FuzzyTerm
{
    public FzOr(FzOr fzOr)
    {
        Terms = new List<FuzzyTerm>();
        foreach (FuzzyTerm term in fzOr.Terms)
        {
            Terms.Add(term.Clone());
        }
    }

    public FzOr(FuzzyTerm op1, FuzzyTerm op2)
    {
        Terms = new List<FuzzyTerm> { op1.Clone(), op2.Clone() };
    }

    public FzOr(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3)
    {
        Terms = new List<FuzzyTerm> { op1.Clone(), op2.Clone(), op3.Clone() };
    }

    public FzOr(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3, FuzzyTerm op4)
    {
        Terms = new List<FuzzyTerm> { op1.Clone(), op2.Clone(), op3.Clone(), op4.Clone() };
    }

    public List<FuzzyTerm> Terms { get; }

    public override FuzzyTerm Clone()
    {
        return new FzOr(this);
    }

    /// <summary>
    /// The OR operator returns the maximum DOM of the sets it is operating on.
    /// </summary>
    /// <returns>The DOM.</returns>
    public override float GetDom()
    {
        return Terms.Select(term => term.GetDom()).Prepend(float.MinValue).Max();
    }

    /// <summary>
    /// Method for updating the DOM of a consequent when a rule fires.
    /// </summary>
    /// <param name="givenValue">Given value.</param>
    public override void OrWithDom(float givenValue) { }
}