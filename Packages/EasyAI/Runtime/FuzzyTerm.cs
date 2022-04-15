/// <summary>
/// Abstract class to provide an interface for classes able to be used as terms in a fuzzy if-then rule base.
/// </summary>
public abstract class FuzzyTerm
{
    public abstract FuzzyTerm Clone();

    public abstract float GetDom();

    public abstract void ClearDom();

    /// <summary>
    /// Method for updating the DOM of a consequent when a rule fires.
    /// </summary>
    /// <param name="givenValue">Given value.</param>
    public abstract void OrWithDom(float givenValue);
}