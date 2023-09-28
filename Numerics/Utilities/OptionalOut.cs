namespace Numerics
{
    /// <summary>
    /// This provides support for optional ByRef or Out parameters for sub/function input. 
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class OptionalOut<Type>
    {
        /// <summary>
        /// The optional result. 
        /// </summary>
        public Type Result { get; set; }
    }
}
