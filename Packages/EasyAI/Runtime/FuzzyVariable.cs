using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FuzzyVariable
{
    public FuzzyVariable()
    {
        MemberSets = new Dictionary<string, FuzzySet>();
        MinRange = 0.0f;
        MaxRange = 0.0f;
    }

    public Dictionary<string, FuzzySet> MemberSets { get; }

    public float MinRange { get; set; }

    public float MaxRange { get; set; }

    /// <summary>
    /// Takes a crisp value and calculates its degree of membership for each set in the variable.
    /// </summary>
    /// <param name="crispValue">The crisp value.</param>
    public void Fuzzify(float crispValue)
    {
        // Make sure the value is within the bounds of this variable.
        if (crispValue < MinRange || crispValue > MaxRange)
        {
            return;
        }

        // For each set in the flv calculate the DOM for the given value.
        foreach ((string _, FuzzySet value) in MemberSets)
        {
            value.Dom = value.CalculateDom(crispValue);
        }
    }
    
    /// <summary>
    /// Defuzzifies the value by averaging the maxima of the sets that have fired.
    /// </summary>
    /// <returns>Sum (maxima * DOM) / sum (DOMs).</returns>
    public float DeFuzzifyMaxAv()
    {
        float bottom = 0.0f;
        float top = 0.0f;

        foreach ((string _, FuzzySet value) in MemberSets)
        {
            bottom += value.Dom;
            top += value.RepresentativeValue * value.Dom;
        }

        // Make sure bottom is not equal to zero.
        return Mathf.Approximately(0, bottom) ? 0.0f : top / bottom;
    }

    /// <summary>
    /// Defuzzify the variable using the centroid method.
    /// </summary>
    /// <param name="sampleCount">The count.</param>
    /// <returns>Defuzzy centroid value.</returns>
    public float DeFuzzifyCentroid(int sampleCount)
    {
        // Calculate the step size.
        float stepSize = (MaxRange - MinRange) / sampleCount;

        float totalArea = 0.0f;
        float sumOfMoments = 0.0f;

        // Step through the range of this variable in increments equal to
        // stepSize adding up the contribution (lower of CalculateDOM or
        // the actual DOM of this variable's fuzzified value) for each
        // subset. This gives an approximation of the total area of the
        // fuzzy manifold. (This is similar to how the area under a curve
        // is calculated using calculus... the heights of lots of 'slices'
        // are summed to give the total area.).
        // In addition the moment of each slice is calculated and summed.
        // Dividing the total area by the sum of the moments gives the
        // centroid. (Just like calculating the center of mass of an object).
        for (int sample = 1; sample <= sampleCount; ++sample)
        {
            // For each set get the contribution to the area. This is the
            // lower of the value returned from CalculateDOM or the actual
            // DOM of the fuzzified value itself.
            int sample1 = sample;
            foreach (float contribution in from FuzzySet value in MemberSets select Mathf.Min(value.CalculateDom(MinRange + sample1 * stepSize), value.Dom))
            {
                totalArea += contribution;
                sumOfMoments += (MinRange + sample * stepSize) * contribution;
            }
        }

        // Make sure total area is not equal to zero.
        return Mathf.Approximately(0, totalArea) ? 0.0f : sumOfMoments / totalArea;
    }

    public FzSet AddTriangularSet(string name, float minimumBound, float peak, float maximumBound)
    {
        MemberSets[name] = new FuzzySetTriangle(peak, peak - minimumBound, maximumBound - peak);

        // Adjust range if necessary.
        AdjustRangeToFit(minimumBound, maximumBound);

        return new FzSet(MemberSets[name]);
    }

    public FzSet AddLeftShoulderSet(string name, float minimumBound, float peak, float maximumBound)
    {
        MemberSets[name] = new FuzzySetLeftShoulder(peak, peak - minimumBound, maximumBound - peak);

        // Adjust range if necessary.
        AdjustRangeToFit(minimumBound, maximumBound);

        return new FzSet(MemberSets[name]);
    }

    public FzSet AddRightShoulderSet(string name, float minimumBound, float peak, float maximumBound)
    {
        MemberSets[name] = new FuzzySetRightShoulder(peak, peak - minimumBound, maximumBound - peak);

        // Adjust range if necessary.
        AdjustRangeToFit(minimumBound, maximumBound);

        return new FzSet(MemberSets[name]);
    }

    public FzSet AddSingletonSet(string name, float minimumBound, float peak, float maximumBound)
    {
        MemberSets[name] = new FuzzySetSingleton(peak, peak - minimumBound, maximumBound - peak);

        // Adjust range if necessary.
        AdjustRangeToFit(minimumBound, maximumBound);

        return new FzSet(MemberSets[name]);
    }
    
    /// <summary>
    /// This method is called with the upper and lower bound of a set each time a new set is added to adjust the upper and lower range values accordingly.
    /// </summary>
    /// <param name="minimumBound">Minimum.</param>
    /// <param name="maximumBound">Maximum.</param>
    private void AdjustRangeToFit(float minimumBound, float maximumBound)
    {
        if (minimumBound < MinRange)
        {
            MinRange = minimumBound;
        }

        if (maximumBound > MaxRange)
        {
            MaxRange = maximumBound;
        }
    }
}