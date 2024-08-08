using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;
using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using Numerics.Data.Statistics;



namespace Data.TimeSeriesAnalysis
{
    /// <summary>
    /// Unit tests for the TimeSeries class
    /// class.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     <list type="bullet">
    ///     <item>Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil</item>
    ///     <item>Sadie Niblett, USACE Risk Management Center, sadie.s.niblett@usace.army.mil</item>
    ///     </list>
    /// </para>
    /// </remarks>
    [TestClass]
    public class Test_TimeSeriesMethods
    {
        [TestMethod]
        public void Test_Construction()
        {
            var ts1 = new TimeSeries();

            var ts2 = new TimeSeries(TimeInterval.OneYear);

            var ts3 = new TimeSeries(TimeInterval.OneYear, new DateTime(2023, 01, 01), new DateTime(2023, 12, 31));

            var ts4 = new TimeSeries(TimeInterval.OneYear, new DateTime(2023, 01, 01), new DateTime(2023, 12, 31), 5);

            var ts5 = new TimeSeries(TimeInterval.OneYear, new DateTime(2023, 01, 01), new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });

            var xElement = new XElement("xElement",
                new XAttribute("TimeInterval", TimeInterval.OneYear),
                new XElement("SeriesOrdinate", new XAttribute("Index", new DateTime(2023, 01, 01)), new XAttribute("Value", 5)),
                new XElement("SeriesOrdinate", new XAttribute("Index", new DateTime(2023, 12, 31)), new XAttribute("Value", 5)),
                new XElement("SeriesOrdinate", new XAttribute("Index", new DateTime(2024, 01, 01)), new XAttribute("Value", 5)));

            var ts6 = new TimeSeries(xElement);
        }

        [TestMethod]
        public void Test_Getters()
        {
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });

            var ti = ts.TimeInterval;
            Assert.AreEqual(TimeInterval.OneMonth, ti);

            var missing = ts.HasMissingValues;
            Assert.IsFalse(missing);

            var start = ts.StartDate;
            Assert.AreEqual(new DateTime(2023, 01, 01), start);

            var end = ts.EndDate;
            Assert.AreEqual(new DateTime(2023, 12, 01), end);
        }

        [TestMethod]
        public void Test_ToXElement()
        {
            var ts = new TimeSeries(TimeInterval.OneYear, new DateTime(2023, 01, 01), new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 });
            var xElement = ts.ToXElement();

            var name = xElement.Name;

            // will be overwritten, but must initialize to something;
            TimeInterval time = TimeInterval.OneDay;
            List<SeriesOrdinate<DateTime, double>> ordinates = new List<SeriesOrdinate<DateTime, double>>();

            if (xElement.Attribute(nameof(TimeInterval)) != null)
                Enum.TryParse(xElement.Attribute(nameof(TimeInterval)).Value, out time);

            foreach (XElement ordinate in xElement.Elements("SeriesOrdinate"))
            {
                DateTime index = DateTime.Now;
                DateTime.TryParse(ordinate.Attribute("Index").Value, out index);
                double value = 0;
                double.TryParse(ordinate.Attribute("Value").Value, out value);
                ordinates.Add(new SeriesOrdinate<DateTime, double>(index, value));
            }

            for (int i = 0; i < ts.Count; i++)
            {
                Assert.AreEqual(ts[i].Index, ordinates[i].Index);
                Assert.AreEqual(ts[i].Value, ordinates[i].Value);
            }

            Assert.AreEqual("TimeSeries", name);
            Assert.AreEqual(ts.TimeInterval, time);
        }

        [TestMethod]
        public void Test_Clone()
        {
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 });

            var newTS = ts.Clone();

            for (int i = 0; i < ts.Count; i++)
            {
                Assert.AreEqual(ts[i].Index, newTS[i].Index);
                Assert.AreEqual(ts[i].Value, newTS[i].Value);
            }

            Assert.AreEqual(ts.HasMissingValues, newTS.HasMissingValues);
            Assert.AreEqual(ts.TimeInterval, newTS.TimeInterval);
            Assert.AreEqual(ts.StartDate, newTS.StartDate);
            Assert.AreEqual(ts.EndDate, newTS.EndDate);
        }

        [TestMethod]
        public void Test_Sort()
        {
            var values = new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 };
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), values);
            ts.SortByTime();

            for (int i = 0; i < ts.Count; i++)
            {
                // values are in original order b/c that's how they were added and time stamped
                Assert.AreEqual(values[i], ts[i].Value);
            }

            ts.SortByValue();
            // now values must be sorted
            Array.Sort(values);

            for (int i = 0; i < ts.Count; i++)
            {
                Assert.AreEqual(values[i], ts[i].Value);
            }
        }

        // helper function for math
        public void Equal(TimeSeries ts, double[] values, double tol = 1E-6)
        {
            for (int i = 0; i < ts.Count; i++)
            {
                Assert.AreEqual(values[i], ts[i].Value, tol);
            }
        }

        [TestMethod]
        public void Test_Math()
        {
            var values = new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 };
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), values);

            for (int i = 0; i < values.Length; i++) { values[i] += 10; }
            ts.Add(10);
            Equal(ts, values);

            for (int i = 0; i < values.Length; i++) { values[i] -= 5; }
            ts.Subtract(5);
            Equal(ts, values);

            for (int i = 0; i < values.Length; i++) { values[i] *= 4; }
            ts.Multiply(4);
            Equal(ts, values);

            for (int i = 0; i < values.Length; i++) { values[i] /= -2; }
            ts.Divide(-2);
            Equal(ts, values);

            for (int i = 0; i < values.Length; i++) { values[i] = Math.Abs(values[i]); }
            ts.AbsoluteValue();
            Equal(ts, values);

            for (int i = 0; i < values.Length; i++) { values[i] = Math.Pow(values[i], 2); }
            ts.Exponentiate(2);
            Equal(ts, values);

            for (int i = 0; i < values.Length; i++) { values[i] = Math.Log10(values[i]); }
            ts.LogTransform();
            Equal(ts, values);

            for (int i = 0; i < values.Length; i++) { values[i] = 1 / values[i]; }
            ts.Inverse();
            Equal(ts, values);

            var mean = Numerics.Data.Statistics.Statistics.Mean(values); var sd = Numerics.Data.Statistics.Statistics.StandardDeviation(values);
            for (int i = 0; i < values.Length; i++) { values[i] = (values[i] - mean) / sd; }
            ts.Standardize();
            Equal(ts, values, 1E-1);
        }

        [TestMethod]
        public void Test_Cumulative()
        {
            var values = new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 };
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), values);
            var newValues = new double[12];

            for (int i = 1; i < values.Length; i++) { values[i] = values[i - 1] + values[i]; }
            var newTS = ts.CumulativeSum();
            Equal(newTS, values);

            values = new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 };
            ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), values);

            for (int i = 1; i < newValues.Length; i++) { newValues[i] = values[i] - values[i - 1]; }
            newTS = ts.Difference();
            // don't want first value 
            Equal(newTS, newValues.Skip(1).ToArray());
        }

        [TestMethod]
        public void Test_Missing()
        {
            var values = new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, double.NaN, double.NaN };
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), values);

            var missing = ts.NumberOfMissingValues();
            Assert.AreEqual(2, missing);

            ts.ReplaceMissingData(-1);
            missing = ts.NumberOfMissingValues();
            Assert.AreEqual(0, missing);
            Assert.AreEqual(-1, ts[10].Value);
            Assert.AreEqual(-1, ts[11].Value);

            values = new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, double.NaN, double.NaN };
            ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), values);

            ts.InterpolateMissingData(2);
            missing = ts.NumberOfMissingValues();
            Assert.AreEqual(0, missing);
            Assert.AreEqual(11.9, ts[10].Value, 1E-6);
            Assert.AreEqual(8.9, ts[11].Value, 1E-6);
        }

        [TestMethod]
        public void Test_AddInterval()
        {
            // DateTime(year, month, day, hour, minute, second)
            DateTime original = new DateTime(2023, 01, 01, 09, 30, 25);

            var test = TimeSeries.AddTimeInterval(original, TimeInterval.OneMinute);
            Assert.AreEqual(original.AddMinutes(1), test);

            test = TimeSeries.AddTimeInterval(original, TimeInterval.FiveMinute);
            Assert.AreEqual(original.AddMinutes(5), test);

            test = TimeSeries.AddTimeInterval(original, TimeInterval.FifteenMinute);
            Assert.AreEqual(original.AddMinutes(15), test);

            test = TimeSeries.AddTimeInterval(original, TimeInterval.ThirtyMinute);
            Assert.AreEqual(original.AddMinutes(30), test);

            test = TimeSeries.AddTimeInterval(original, TimeInterval.OneHour);
            Assert.AreEqual(original.AddHours(1), test);

            test = TimeSeries.AddTimeInterval(original, TimeInterval.SixHour);
            Assert.AreEqual(original.AddHours(6), test);

            test = TimeSeries.AddTimeInterval(original, TimeInterval.TwelveHour);
            Assert.AreEqual(original.AddHours(12), test);

            test = TimeSeries.AddTimeInterval(original, TimeInterval.OneDay);
            Assert.AreEqual(original.AddDays(1), test);

            test = TimeSeries.AddTimeInterval(original, TimeInterval.SevenDay);
            Assert.AreEqual(original.AddDays(7), test);

            test = TimeSeries.AddTimeInterval(original, TimeInterval.OneMonth);
            Assert.AreEqual(original.AddMonths(1), test);

            test = TimeSeries.AddTimeInterval(original, TimeInterval.OneQuarter);
            Assert.AreEqual(original.AddMonths(3), test);

            test = TimeSeries.AddTimeInterval(original, TimeInterval.OneYear);
            Assert.AreEqual(original.AddYears(1), test);
        }

        [TestMethod]
        public void Test_SubtractInterval()
        {
            // DateTime(year, month, day, hour, minute, second)
            DateTime original = new DateTime(2023, 01, 01, 09, 30, 25);

            var test = TimeSeries.SubtractTimeInterval(original, TimeInterval.OneMinute);
            Assert.AreEqual(original.AddMinutes(-1), test);

            test = TimeSeries.SubtractTimeInterval(original, TimeInterval.FiveMinute);
            Assert.AreEqual(original.AddMinutes(-5), test);

            test = TimeSeries.SubtractTimeInterval(original, TimeInterval.FifteenMinute);
            Assert.AreEqual(original.AddMinutes(-15), test);

            test = TimeSeries.SubtractTimeInterval(original, TimeInterval.ThirtyMinute);
            Assert.AreEqual(original.AddMinutes(-30), test);

            test = TimeSeries.SubtractTimeInterval(original, TimeInterval.OneHour);
            Assert.AreEqual(original.AddHours(-1), test);

            test = TimeSeries.SubtractTimeInterval(original, TimeInterval.SixHour);
            Assert.AreEqual(original.AddHours(-6), test);

            test = TimeSeries.SubtractTimeInterval(original, TimeInterval.TwelveHour);
            Assert.AreEqual(original.AddHours(-12), test);

            test = TimeSeries.SubtractTimeInterval(original, TimeInterval.OneDay);
            Assert.AreEqual(original.AddDays(-1), test);

            test = TimeSeries.SubtractTimeInterval(original, TimeInterval.SevenDay);
            Assert.AreEqual(original.AddDays(-7), test);

            test = TimeSeries.SubtractTimeInterval(original, TimeInterval.OneMonth);
            Assert.AreEqual(original.AddMonths(-1), test);

            test = TimeSeries.SubtractTimeInterval(original, TimeInterval.OneQuarter);
            Assert.AreEqual(original.AddMonths(-3), test);

            test = TimeSeries.SubtractTimeInterval(original, TimeInterval.OneYear);
            Assert.AreEqual(original.AddYears(-1), test);
        }

        [TestMethod]
        public void Test_TimeIntervalInHours()
        {
            var valid = new double[] { 1d / 60d, 1 / 12, 3 / 12, 1 / 2, 1, 6, 12, 24, 168 };

            double test = TimeSeries.TimeIntervalInHours(TimeInterval.OneMinute);
            Assert.AreEqual(1d / 60d, test);

            test = TimeSeries.TimeIntervalInHours(TimeInterval.FiveMinute);
            Assert.AreEqual(1d / 12d, test);

            test = TimeSeries.TimeIntervalInHours(TimeInterval.FifteenMinute);
            Assert.AreEqual(3d / 12d, test);

            test = TimeSeries.TimeIntervalInHours(TimeInterval.ThirtyMinute);
            Assert.AreEqual(1d / 2d, test);

            test = TimeSeries.TimeIntervalInHours(TimeInterval.OneHour);
            Assert.AreEqual(1, test);

            test = TimeSeries.TimeIntervalInHours(TimeInterval.SixHour);
            Assert.AreEqual(6, test);

            test = TimeSeries.TimeIntervalInHours(TimeInterval.TwelveHour);
            Assert.AreEqual(12, test);

            test = TimeSeries.TimeIntervalInHours(TimeInterval.OneDay);
            Assert.AreEqual(24, test);

            test = TimeSeries.TimeIntervalInHours(TimeInterval.SevenDay);
            Assert.AreEqual(168, test);
        }

        [TestMethod]
        public void Test_MovingAverage()
        {
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 });

            var newTS = ts.MovingAverage(5);
            var valid = new double[] { 17.6, 20.4, 26.8, 22.2, 24.8, 25.4, 22.6, 15.6 };
            for (int i = 0; i < newTS.Count; i++)
            {
                Assert.AreEqual(valid[i], newTS[i].Value);
            }
        }

        [TestMethod]
        public void Test_MovingSum()
        {
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 });

            var newTS = ts.MovingSum(5);
            var valid = new double[] { 88, 102, 134, 111, 124, 127, 113, 78 };
            for (int i = 0; i < newTS.Count; i++)
            {
                Assert.AreEqual(valid[i], newTS[i].Value);
            }
        }

        [TestMethod]
        public void Test_ShiftAllDates()
        {
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 });
            var newTS = ts.Clone();
            newTS.ShiftAllDates(new DateTime(2024, 02, 04));

            for (int i = 0; i < newTS.Count; i++)
            {
                Assert.AreEqual(ts[i].Index.AddYears(1).AddMonths(1).AddDays(3), newTS[i].Index);
                Assert.AreEqual(ts[i].Value, newTS[i].Value);
            }
        }

        [TestMethod]
        public void Test_ShiftDatesByMonth()
        {
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 });
            var newTS = ts.ShiftDatesByMonth(5);

            for (int i = 0; i < newTS.Count; i++)
            {
                //Assert.AreEqual(ts[i].Index.AddMonths(5), newTS[i].Index);
                //Assert.AreEqual(ts[i].Value, newTS[i].Value);
            }
        }

        [TestMethod]
        public void Test_ShiftDatesByYear()
        {
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 });
            var newTS = ts.ShiftDatesByYear(5);

            for (int i = 0; i < newTS.Count; i++)
            {
                Assert.AreEqual(ts[i].Index.AddYears(5), newTS[i].Index);
                Assert.AreEqual(ts[i].Value, newTS[i].Value);
            }
        }

        // test clip time series
        [TestMethod]
        public void Test_ClipTimeSeries()
        {
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 });
            var start = new DateTime(2023, 11, 01); 
            var end = new DateTime(2023, 12, 01);
            var newTS = ts.ClipTimeSeries(start,end);

            Assert.AreEqual(2, newTS.Count);
            Assert.AreEqual(start, newTS.StartDate);
            Assert.AreEqual(end, newTS.EndDate);

            //var orgIndex = ts.IndexOf(new SeriesOrdinate<DateTime, double>(start, 22));
            //var orgIndex = ts.IndexOf(new SeriesOrdinate<DateTime, double>(new DateTime(2023, 11, 01), 22));
            for (int i = 0; i < newTS.Count; i++)
            {
                Assert.AreEqual(ts[10 + i].Index, newTS[i].Index);
                Assert.AreEqual(ts[10 + i].Value, newTS[i].Value);
            }
        }

        // test convert time interval

        [TestMethod]
        public void Test_Stats()
        {
            var values = new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 };
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), values);

            double min = ts.MinValue();
            Assert.AreEqual(5, min);

            double max = ts.MaxValue();
            Assert.AreEqual(48, max);

            double mean = ts.MeanValue();
            Assert.AreEqual(20.83333333333, mean, 1E-10);

            double sd = ts.StandardDeviation();
            Assert.AreEqual(12.40112, sd, 1E-3);

            var duration = ts.Duration();
            var dValues = new double[] { 48, 36, 33, 22, 22, 18, 16, 15, 13, 12, 10, 5 };
            var dValid = new double[] { 7.69230769230769, 15.3846153846154, 23.0769230769231, 30.7692307692308, 38.4615384615385, 46.1538461538462, 53.8461538461538, 61.5384615384615, 69.2307692307692, 76.9230769230769, 84.6153846153846, 92.3076923076923 };

            for(int i = 0; i < dValid.Length; i++)
            {
                Assert.AreEqual(dValid[i], duration[i, 0], 1E-10);
                Assert.AreEqual(dValues[i], duration[i, 1], 1E-10);
            }
        }

        [TestMethod]
        public void Test_SummaryStats()
        {
            var values = new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13 };
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), values);

            var summaryP = ts.SummaryPercentiles();
            var spValid = new double[] { 7.75, 12.75, 17.00, 24.75, 41.40 };
            for (int i = 0; i < summaryP.Length; i++)
            {
                Assert.AreEqual(spValid[i], summaryP[i], 1E-10);
            }

            var percentiles = ts.Percentiles(new double[] { 0.10, 0.20, 0.30, 0.40, 0.60, 0.70, 0.80, 0.90 });
            var pValid = new double[] { 10.2, 12.2, 13.6, 15.4, 20.4, 22.0, 30.8, 35.7 };
            for (int i = 0; i < percentiles.Length; i++)
            {
                Assert.AreEqual(pValid[i], percentiles[i], 1E-10);
            }

            var summaryStats = ts.SummaryStatistics();
            var testStats = new double[13];

            summaryStats.TryGetValue("Record Length", out testStats[0]);
            summaryStats.TryGetValue("Missing Values", out testStats[1]);
            summaryStats.TryGetValue("Minimum", out testStats[2]);
            summaryStats.TryGetValue("Maximum", out testStats[3]);
            summaryStats.TryGetValue("Mean", out testStats[4]);
            summaryStats.TryGetValue("Std Dev", out testStats[5]);
            summaryStats.TryGetValue("Skewness", out testStats[6]);
            summaryStats.TryGetValue("Kurtosis", out testStats[7]);
            summaryStats.TryGetValue("5%", out testStats[8]);
            summaryStats.TryGetValue("25%", out testStats[9]);
            summaryStats.TryGetValue("50%", out testStats[10]);
            summaryStats.TryGetValue("75%", out testStats[11]);
            summaryStats.TryGetValue("95%", out testStats[12]);

            var validStats = new double[] { 12, 0, 5, 48, 20.83333333333, 12.4011240937215, 1.06382220138287, 0.682181791356259, 7.75, 12.75, 17.00, 24.75, 41.40 };
            for (int i = 0; i < validStats.Length; i++)
            {
                Assert.AreEqual(validStats[i], testStats[i], 1E-10);
            }
        }

        [TestMethod]
        public void Test_SummaryHypothesisTest()
        {
            var values = new double[] { 122d, 244d, 214d, 173d, 229d, 156d, 212d, 263d, 146d, 183d, 161d, 205d, 135d, 331d, 225d, 174d, 98.8d, 149d, 238d, 262d, 132d, 235d, 216d, 240d, 230d, 192d, 195d, 172d, 173d, 172d, 153d, 142d, 317d, 161d, 201d, 204d, 194d, 164d, 183d, 161d, 167d, 179d, 185d, 117d, 192d, 337d, 125d, 166d, 99.1d, 202d, 230d, 158d, 262d, 154d, 164d, 182d, 164d, 183d, 171d, 250d, 184d, 205d, 237d, 177d, 239d, 187d, 180d, 173d, 174d };
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), values);
            var hypothesis = ts.SummaryHypothesisTest();
            var testH = new double[7];

            hypothesis.TryGetValue("Jarque-Bera test for normality", out testH[0]);
            hypothesis.TryGetValue("Ljung-Box test for independence", out testH[1]);
            hypothesis.TryGetValue("Wald-Wolfowitz test for independence and stationarity (trend)", out testH[2]);
            hypothesis.TryGetValue("Mann-Whitney test for homogeneity and stationarity (jump)", out testH[3]);
            hypothesis.TryGetValue("Mann-Kendall test for homogeneity and stationarity (trend)", out testH[4]);
            hypothesis.TryGetValue("t-test for differences in the means of two samples", out testH[5]);
            hypothesis.TryGetValue("F-test for differences in the variances of two samples", out testH[6]);

            var validH = new double[] { 0.002439d/2d, 0.1762, 0.4608, 0.5956, 0.7757, 0.7921, 0.03607 };
            for (int i = 0; i < validH.Length; i++)
            {
                //Assert.AreEqual(validH[i], testH[i], 1E-2);
            }
        }

        [TestMethod]
        public void Test_MonthlyStats()
        {
            var values = new double[] { 22, 16, 33, 5, 12, 36, 48, 10, 18, 15, 22, 13, 38, 17, 3, 6, 27, 11, 2, 41, 44, 37, 50, 8 };
            var ts = new TimeSeries(TimeInterval.OneMonth, new DateTime(2023, 01, 01), values);

            var percentiles = ts.MonthlyPercentiles(new double[] { 0.10, 0.20, 0.30, 0.40, 0.60, 0.70, 0.80, 0.90 });
            var validP = new double[,] { { 23.6, 25.2, 26.8, 28.4, 31.6, 33.2, 34.8, 36.4 },
                                         { 16.1, 16.2, 16.3, 16.4, 16.6, 16.7, 16.8, 16.9 },
                                         { 6, 9, 12, 15, 21, 24, 27, 30 },
                                         { 5.1, 5.2, 5.3, 5.4, 5.6, 5.7, 5.8, 5.9 },
                                         { 13.5, 15.0, 16.5, 18.0, 21.0, 22.5, 24.0, 25.5 },
                                         { 13.5, 16.0, 18.5, 21.0, 26.0, 28.5, 31.0, 33.5 },
                                         { 6.6, 11.2, 15.8, 20.4, 29.6, 34.2, 38.8, 43.4 },
                                         { 13.1, 16.2, 19.3, 22.4, 28.6, 31.7, 34.8, 37.9 },
                                         { 20.6, 23.2, 25.8, 28.4, 33.6, 36.2, 38.8, 41.4 },
                                         { 17.2, 19.4, 21.6, 23.8, 28.2, 30.4, 32.6, 34.8 },
                                         { 24.8, 27.6, 30.4, 33.2, 38.8, 41.6, 44.4, 47.2 },
                                         { 8.5,  9.0,  9.5, 10.0, 11.0, 11.5, 12.0, 12.5 } };
            
            var summary = ts.MonthlySummaryStatistics();
            var validS = new double[,] { { 22, 22.8, 26.0, 30.0, 34.0, 37.2, 38, 30 },
                                         { 16, 16.05, 16.25, 16.50, 16.75, 16.95, 17, 16.5 },
                                         { 3, 4.5, 10.5, 18.0, 25.5, 31.5, 33, 18 },
                                         { 5, 5.05, 5.25, 5.50, 5.75, 5.95, 6, 5.5 },
                                         { 12, 12.75, 15.75, 19.50, 23.25, 26.25, 27, 19.5},
                                         { 11, 12.25, 17.25, 23.50, 29.75, 34.75 , 36, 23.5 },
                                         { 2, 4.3, 13.5, 25.0, 36.5, 45.7, 48, 25},
                                         { 10, 11.55, 17.75, 25.50, 33.25, 39.45, 41, 25.5},
                                         { 18, 19.3, 24.5, 31.0, 37.5, 42.7, 44, 31 },
                                         { 15, 16.1, 20.5, 26.0, 31.5, 35.9, 37, 26 },
                                         { 22, 23.4, 29.0, 36.0, 43.0, 48.6, 50, 36 },
                                         { 8, 8.25, 9.25, 10.50, 11.75, 12.75, 13, 10.5 } };

            for (int i = 0; i < 11; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    Assert.AreEqual(validP[i,j], percentiles[i,j], 1E-10);
                    Assert.AreEqual(validS[i,j], summary[i,j], 1E-10);
                }
            }
        }

        // test monthly frequency

        // test annual max series (x2)

        // test calendar year series

        // test water year series

        // test custom year series

        // test monthly series

        // test quarterly series

        // test peaks over threshold series

    }
}
