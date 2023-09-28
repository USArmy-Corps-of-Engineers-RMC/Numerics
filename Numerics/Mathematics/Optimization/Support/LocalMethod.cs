namespace Numerics.Mathematics.Optimization
{

    /// <summary>
    /// The enumeration of local optimization methods for use in global optimizers.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public enum LocalMethod
    {
        BFGS,
        NelderMead,
        Powell
    }
}
