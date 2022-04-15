using UnityEngine;

// Class to define an interface for a fuzzy set.
public abstract class FuzzySet
{
    private float _dom;
    
    public float PeakPoint { get; }

    public float LeftOffset { get; }

    public float RightOffset { get; }

    protected FuzzySet(float representativeValue, float peak, float left, float right)
    {
        _dom = 0.0f;
        RepresentativeValue = representativeValue;
        PeakPoint = peak;
        LeftOffset = left;
        RightOffset = right;
    }

    /// <summary>
    /// Gets the maximum of the set's membership function. For instance, if the set is
    /// triangular then this will be the peak point of the triangular. If the set has a plateau
    /// then this value will be the mid-point of the plateau. This value is set in the
    /// constructor to avoid run-time calculation of mid-point values.
    /// </summary>
    public float RepresentativeValue { get; }

    public float Dom
    {
        get => _dom;
        set => _dom = Mathf.Clamp(value, 0, 1);
    }

    /// <summary>
    /// Returns the degree of membership in this set of the given value. This does not set
    /// dom to the degree of membership of the value passed as the parameter.
    /// This is because the centroid defuzzification method also uses this method to determine
    /// the DOMs of the values it uses as its sample points.
    /// </summary>
    public abstract float CalculateDom(float givenValue);

    /// <summary>
    /// If this fuzzy set is part of a consequent FLV, and it is fired by a rule then this
    /// method sets the DOM (in this context, the DOM represents a confidence level) to the
    /// maximum of the parameter value or the set's existing dom value.
    /// </summary>
    public void OrWithDom(float givenValue)
    {
        if (givenValue > _dom)
        {
            _dom = givenValue;
        }
    }

    public void ClearDom()
    {
        _dom = 0.0f;
    }
}