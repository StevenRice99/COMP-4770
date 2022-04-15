public class FuzzyRule
{
    public FuzzyRule(FuzzyTerm antecedent, FuzzyTerm consequent)
    {
        Antecedent = antecedent.Clone();
        Consequent = consequent.Clone();
    }

    /// <summary>
    /// Gets the antecedent (usually a composite of several fuzzy sets and operators).
    /// </summary>
    public FuzzyTerm Antecedent { get; }

    /// <summary>
    /// Gets the consequent (usually a single fuzzy set, but can be several ANDed together).
    /// </summary>
    public FuzzyTerm Consequent { get; }

    /// <summary>
    /// Clear the degree of membership of the consequent.
    /// </summary>
    public void SetConfidenceOfConsequentToZero()
    {
        Consequent.ClearDom();
    }

    /// <summary>
    /// This method updates the DOM (the confidence) of the consequent term with the DOM of the antecedent term.
    /// </summary>
    public void Calculate()
    {
        Consequent.OrWithDom(Antecedent.GetDom());
    }
}