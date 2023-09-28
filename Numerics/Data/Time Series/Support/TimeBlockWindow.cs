using System;

namespace Numerics.Data
{
    /// <summary>
    /// Enumeration of time block window options.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>

    [Serializable]
    public enum TimeBlockWindow
    {
        CalendarYear,
        WaterYear,
        CustomYear,
        Quarter,
        Month,       
    }
}
