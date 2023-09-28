using System.ComponentModel.DataAnnotations;

namespace Numerics.Data
{
    /// <summary>
    /// Enumeration of data transformations.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public enum Transform
    {
        /// <summary>
        /// Linear, or no transform. 
        /// </summary>
        [Display(Name = "None")]
        None,

        /// <summary>
        /// Logarithmic transform. Values must be greater than 0.
        /// </summary>
        [Display(Name = "Logarithmic")]
        Logarithmic,

        /// <summary>
        /// Normal distribution Z-variate transform. Values must be between 0 and 1. 
        /// </summary>
        [Display(Name = "Normal Z-variate")]
        NormalZ
    }
}
