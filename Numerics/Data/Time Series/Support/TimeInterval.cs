using System;
using System.ComponentModel;

namespace Numerics.Data
{

    /// <summary>
    /// Enumeration of available time-series time intervals.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>

    [Serializable]
    public enum TimeInterval
    {
        [Description("1-Min")]
        OneMinute,
        [Description("5-Min")]
        FiveMinute,
        [Description("15-Min")]
        FifteenMinute,
        [Description("30-Min")]
        ThirtyMinute,
        [Description("1-Hr")]
        OneHour,
        [Description("6-Hr")]
        SixHour,
        [Description("12-Hr")]
        TwelveHour,
        [Description("1-Day")]
        OneDay,
        [Description("7-Day")]
        SevenDay,
        [Description("1-Month")]
        OneMonth,
        [Description("1-Quarter")]
        OneQuarter,
        [Description("1-Year")]
        OneYear,
        [Description("Irregular")]
        Irregular
    }
}