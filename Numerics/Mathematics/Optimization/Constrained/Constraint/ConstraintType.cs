namespace Numerics.Mathematics.Optimization
{

    /// <summary>
    /// Enumeration of constraint types. 
    /// </summary>
    public enum ConstraintType
    {
        /// <summary>
        /// Equality constraint, h(x) = 0
        /// </summary>
        EqualTo,

        /// <summary>
        /// Inequality constraint for greater than or equal to, h(x) >= 0
        /// </summary>
        GreaterThanOrEqualTo,

        /// <summary>
        /// Inequality constraint for lesser than or equal to, h(x) <= 0
        /// </summary>
        LesserThanOrEqualTo
    }
}
