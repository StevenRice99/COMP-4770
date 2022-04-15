public class FuzzySetSingleton : FuzzySet
{
    public FuzzySetSingleton(float peak, float left, float right) : base(peak, peak, left, right) { }

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
        return givenValue >= PeakPoint - LeftOffset && givenValue <= PeakPoint + RightOffset ? 1.0f : 0.0f;
    }
}