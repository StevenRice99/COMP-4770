using System.Collections.Generic;
using System.Linq;

public class FzAnd : FuzzyTerm
{
    public FzAnd(FzAnd fzAnd)
    {
        Terms = new List<FuzzyTerm>();
        foreach (FuzzyTerm term in fzAnd.Terms)
        {
            Terms.Add(term.Clone());
        }
    }

    public FzAnd(FuzzyTerm op1, FuzzyTerm op2)
    {
        Terms = new List<FuzzyTerm> { op1.Clone(), op2.Clone() };
    }

    public FzAnd(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3)
    {
        Terms = new List<FuzzyTerm> { op1.Clone(), op2.Clone(), op3.Clone() };
    }

    public FzAnd(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3, FuzzyTerm op4)
    {
        Terms = new List<FuzzyTerm> { op1.Clone(), op2.Clone(), op3.Clone(), op4.Clone() };
    }

    public List<FuzzyTerm> Terms { get; }

    public override FuzzyTerm Clone()
    {
        return new FzAnd(this);
    }

    public override float GetDom()
    {
        return Terms.Select(term => term.GetDom()).Prepend(float.MaxValue).Min();
    }

    /// <summary>
    /// Method for updating the DOM of a consequent when a rule fires.
    /// </summary>
    /// <param name="givenValue">Given value.</param>
    public override void OrWithDom(float givenValue)
    {
        foreach (FuzzyTerm term in Terms)
        {
            term.OrWithDom(givenValue);
        }
    }

    public override void ClearDom()
    {
        foreach (FuzzyTerm term in Terms)
        {
            term.ClearDom();
        }
    }
}