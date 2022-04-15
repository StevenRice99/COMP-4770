using UnityEngine;

public class FuzzySetTriangle : FuzzySet
{
    /// <summary>
    /// Peak is point with max value.
    /// LeftOffset is distance to point with value 0 on the left.
    /// RightOffset is distance to point with value 0 on the right.
    /// E.g.,
    /// Dumb = FuzzySetTriangle(80, 80, 20)
    /// Average = FuzzySetTriangle(100, 20, 20)
    /// Clever = FuzzySetTriangle(120, 20, 20)
    /// DOM_Dumb(90) = 0.5
    /// DOM_Average(90) = 0.5
    /// DOM_Average(115) = 0.25
    /// DOM_Clever(115) = 0.75
    /// </summary>
    /// <param name="peak">Peak value.</param>
    /// <param name="left">Left value.</param>
    /// <param name="right">Right value.</param>
    public FuzzySetTriangle(float peak, float left, float right) : base(peak)
    {
        PeakPoint = peak;
        LeftOffset = left;
        RightOffset = right;
    }

    public float PeakPoint { get; }

    public float LeftOffset { get; }

    public float RightOffset { get; }
    
    /// <summary>
    /// Returns the degree of membership in this set of the given value. This does not set
    /// FuzzySet.dom to the degree of membership of the value passed as the
    /// parameter. This is because the centroid defuzzification method also uses this method to
    /// determine the DOMs of the values it uses as its sample points.
    /// </summary>
    /// <param name="givenValue">Given value.</param>
    /// <returns>Calculated value.</returns>
    public override float CalculateDom(float givenValue)
    {
        // Test for the case where the triangle's left or right offsets are zero (to prevent divide by zero errors below),
        // else find DOM if left of center, else find DOM if right of center, otherwise out of range of this FLV so return zero.
        return Mathf.Approximately(RightOffset, 0.0f) && Mathf.Approximately(PeakPoint, givenValue) || Mathf.Approximately(LeftOffset, 0.0f) && Mathf.Approximately(PeakPoint, givenValue)
            ? 1.0f
            : givenValue <= PeakPoint && givenValue >= PeakPoint - LeftOffset
                ? 1.0f / LeftOffset * (givenValue - (PeakPoint - LeftOffset))
                : givenValue > PeakPoint && givenValue < PeakPoint + RightOffset
                    ? 1.0f / -RightOffset * (givenValue - PeakPoint) + 1.0f
                    : 0.0f;
    }
}