using System.Collections.Generic;

/// <summary>
/// This class describes a fuzzy module: a collection of fuzzy variables and the rules that operate on them.
/// </summary>
public class FuzzyModule
{
    /// <summary>
    /// When calculating the centroid of the fuzzy manifold this value is used to determine how many cross-sections should be sampled.
    /// </summary>
    public const int CrossSectionSampleCount = 15;

    public FuzzyModule()
    {
        Rules = new List<FuzzyRule>();
        Variables = new Dictionary<string, FuzzyVariable>();
    }

    /// <summary>
    /// You must pass one of these values to the DeFuzzify method. This module only supports the MaxAv and Centroid methods.
    /// </summary>
    public enum DefuzzifyMethod
    {
        MaxAv,
        Centroid
    }

    /// <summary>
    /// Gets a map of all the fuzzy variables this module uses.
    /// </summary>
    public Dictionary<string, FuzzyVariable> Variables { get; }

    public List<FuzzyRule> Rules { get; }

    /// <summary>
    /// This method calls the Fuzzify method of the variable with the same name as the key.
    /// </summary>
    /// <param name="nameOfFlv">Name of the variable.</param>
    /// <param name="val">Value.</param>
    public void Fuzzify(string nameOfFlv, float val)
    {
        if (Variables.ContainsKey(nameOfFlv))
        {
            Variables[nameOfFlv].Fuzzify(val);
        }
    }

    /// <summary>
    /// Given a fuzzy variable and a defuzzification method this returns a crisp value.
    /// </summary>
    /// <param name="nameOfFlv">Name of the variable.</param>
    /// <param name="method">The method.</param>
    /// <returns>The defuzzy value.</returns>
    public float DeFuzzify(string nameOfFlv, DefuzzifyMethod method)
    {
        // First make sure the key exists.
        if (!Variables.ContainsKey(nameOfFlv))
        {
            return 0;
        }

        SetConfidencesOfConsequentsToZero();

        // Process the rules.
        foreach (FuzzyRule rule in Rules)
        {
            rule.Calculate();
        }

        // Now defuzzify the resultant conclusion using the specified method.
        return method switch
        {
            DefuzzifyMethod.Centroid => Variables[nameOfFlv].DeFuzzifyCentroid(CrossSectionSampleCount),
            DefuzzifyMethod.MaxAv => Variables[nameOfFlv].DeFuzzifyMaxAv(),
            _ => 0
        };
    }

    public void AddRule(FuzzyTerm antecedent, FuzzyTerm consequent)
    {
        Rules.Add(new FuzzyRule(antecedent, consequent));
    }

    public FuzzyVariable CreateFlv(string fuzzyVariableName)
    {
        Variables[fuzzyVariableName] = new FuzzyVariable();

        return Variables[fuzzyVariableName];
    }

    /// <summary>
    /// Zeros the DOMs of the consequents of each rule.
    /// </summary>
    private void SetConfidencesOfConsequentsToZero()
    {
        foreach (FuzzyRule rule in Rules)
        {
            rule.SetConfidenceOfConsequentToZero();
        }
    }
}