/**
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from Numerics.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
* **/

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;

namespace Data.TimeSeriesAnalysis
{
    [TestClass]
    public class Test_TimeSeries
    {
        [TestMethod]
        public void Test_DownloadFromGHCN()
        {
            //var t = TimeSeriesDownload.FromGHCN("USC00040741");
            var t = TimeSeriesDownload.FromGHCN("USC00327027");
            //t = TimeSeriesDownload.FromGHCN("AG000060611");
        }

        [TestMethod]
        public void Test_DownloadFromUSGS()
        {
            var t = TimeSeriesDownload.FromUSGS("08133500");
        }


        [TestMethod]
        public void Test_MovingAverage()
        {

            // import at-site data
            string filePath = "C:\\Users\\Q0RMCCHS\\Documents\\TimeSeriesData.csv";
            StreamReader reader = null;
            var data = new List<double>();
            if (File.Exists(filePath))
            {
                reader = new StreamReader(File.OpenRead(filePath));
                List<string> listA = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    double val = 0;
                    double.TryParse(line, out val);
                    data.Add(val);
                }             
            }
            else
            {
                Console.WriteLine("File doesn't exist");
                return;
            }

            var ts = new TimeSeries(TimeInterval.OneDay, new DateTime(1954, 5, 2, 0, 0, 0), data);

            var ma3 = ts.MovingAverage(3);
            var true_ma3 = new double[] { 1080, 1073.33, 946.67, 768.67, 805.33, 868, 923.33, 829, 755, 684.67, 623.67, 568.67, 525.33, 491, 461, 433.33, 411.67, 398.33, 383, 366, 345.33, 330.67, 318, 307, 294.67, 310, 342.67, 347.67 };
            for (int i = 0; i < true_ma3.Length; i++)
                Assert.AreEqual(true_ma3[i], ma3[i].Value, 1E-2);

            var ma7 = ts.MovingAverage(7);
            var true_ma7 = new double[] { 939.43, 934.86, 896.57, 831.86, 798.43, 784.29, 769.71, 697, 637.43, 584.43, 539.29, 499.57, 467.43, 442.71, 420, 399.14, 380.29, 364.14, 349.71, 335.43, 319.71, 318.43, 325.43, 320.71 };
            for (int i = 0; i < true_ma7.Length; i++)
                Assert.AreEqual(true_ma7[i], ma7[i].Value, 1E-2);

            var ma10 = ts.MovingAverage(10);
            var true_ma10 = new double[] { 906.3, 880.9, 833, 769.4, 729.5, 706.6, 686.1, 626.2, 576.2, 532.6, 497, 464.6, 437, 413.5, 393.2, 374.8, 358.3, 343.3, 337.8, 337.6, 328.1 };
            for (int i = 0; i < true_ma10.Length; i++)
                Assert.AreEqual(true_ma10[i], ma10[i].Value, 1E-2);

        }

        [TestMethod]
        public void Test_MovingSum()
        {
            // import at-site data
            string filePath = "C:\\Users\\Q0RMCCHS\\Documents\\TimeSeriesData.csv";
            StreamReader reader = null;
            var data = new List<double>();
            if (File.Exists(filePath))
            {
                reader = new StreamReader(File.OpenRead(filePath));
                List<string> listA = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    double val = 0;
                    double.TryParse(line, out val);
                    data.Add(val);
                }
            }
            else
            {
                Console.WriteLine("File doesn't exist");
                return;
            }

            var ts = new TimeSeries(TimeInterval.OneDay, new DateTime(1954, 5, 2, 0, 0, 0), data);

            var ms3 = ts.MovingSum(3);
            var true_ms3 = new double[] { 3240, 3220, 2840, 2306, 2416, 2604, 2770, 2487, 2265, 2054, 1871, 1706, 1576, 1473, 1383, 1300, 1235, 1195, 1149, 1098, 1036, 992, 954, 921, 884, 930, 1028, 1043 };
            for (int i = 0; i < true_ms3.Length; i++)
                Assert.AreEqual(true_ms3[i], ms3[i].Value, 1E-2);

            var ms7 = ts.MovingSum(7);
            var true_ms7 = new double[] { 6576, 6544, 6276, 5823, 5589, 5490, 5388, 4879, 4462, 4091, 3775, 3497, 3272, 3099, 2940, 2794, 2662, 2549, 2448, 2348, 2238, 2229, 2278, 2245 };
            for (int i = 0; i < true_ms7.Length; i++)
                Assert.AreEqual(true_ms7[i], ms7[i].Value, 1E-2);

            var ms10 = ts.MovingSum(10);
            var true_ms10 = new double[] { 9063, 8809, 8330, 7694, 7295, 7066, 6861, 6262, 5762, 5326, 4970, 4646, 4370, 4135, 3932, 3748, 3583, 3433, 3378, 3376, 3281 };
            for (int i = 0; i < true_ms10.Length; i++)
                Assert.AreEqual(true_ms10[i], ms10[i].Value, 1E-2);
        }

        [TestMethod]
        public void Test_AnnualMax()
        {
            // import at-site data
            string filePath = "C:\\Users\\Q0RMCCHS\\Documents\\TimeSeriesData.csv";
            StreamReader reader = null;
            var data = new List<double>();
            if (File.Exists(filePath))
            {
                reader = new StreamReader(File.OpenRead(filePath));
                List<string> listA = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    double val = 0;
                    double.TryParse(line, out val);
                    data.Add(val);
                }
            }
            else
            {
                Console.WriteLine("File doesn't exist");
                return;
            }

            var ts = new TimeSeries(TimeInterval.OneDay, new DateTime(1954, 5, 2, 0, 0, 0), data);

            var ams = ts.AnnualMaxSeries();
            var true_ams = new double[] { 2720, 4000, 3300, 3820, 2910, 3860, 4390, 5860, 3520, 2650, 7010, 1530, 4170, 3160, 2680, 0, 0, 0, 20391, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1753, 3399, 5992, 2415, 3934, 5596, 2763, 3646, 2141, 6594, 5667, 4118, 8392, 2717, 4282, 3288, 2979, 2132, 4177, 5016, 15811, 4885, 3133, 2785, 6945, 2320, 8596, 6352, 2210, 4550, 4904, 4446, 1799 };
            for (int i = 0; i < true_ams.Length; i++)
                Assert.AreEqual(true_ams[i], ams[i].Value, 1E-2);

        }

        [TestMethod]
        public void Test_AnnualMax_WaterYear()
        {
            // import at-site data
            string filePath = "C:\\Users\\Q0RMCCHS\\Documents\\TimeSeriesData.csv";
            StreamReader reader = null;
            var data = new List<double>();
            if (File.Exists(filePath))
            {
                reader = new StreamReader(File.OpenRead(filePath));
                List<string> listA = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    double val = 0;
                    double.TryParse(line, out val);
                    data.Add(val);
                }
            }
            else
            {
                Console.WriteLine("File doesn't exist");
                return;
            }

            var ts = new TimeSeries(TimeInterval.OneDay, new DateTime(1954, 5, 2, 0, 0, 0), data);

            var ams = ts.AnnualMaxSeries(10);
            var true_ams = new double[] { 1200, 3260,   4000,   3820,   2910,   3860,   4390,   5860,   3520,   2650,   7010,   1530,   4170,   3160,   2680,   0,  0,  0,  20391,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2569,   5992,   2524,   3934,   5596,   2716,   3646,   2141,   6594,   6301,   3182,   8392,   4747,   4282,   3288,   2979,   2132,   4177,   4831,   15811,  3913,   4885,   2785,   6945,   2320,   5207,   8596,   4085,   4550,   4904,   4446,   1799 };
            for (int i = 0; i < true_ams.Length; i++)
                Assert.AreEqual(true_ams[i], ams[i].Value, 1E-2);

        }

        [TestMethod]
        public void Test_AnnualMax_Seasonal()
        {
            // import at-site data
            string filePath = "C:\\Users\\Q0RMCCHS\\Documents\\TimeSeriesData.csv";
            StreamReader reader = null;
            var data = new List<double>();
            if (File.Exists(filePath))
            {
                reader = new StreamReader(File.OpenRead(filePath));
                List<string> listA = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    double val = 0;
                    double.TryParse(line, out val);
                    data.Add(val);
                }
            }
            else
            {
                Console.WriteLine("File doesn't exist");
                return;
            }

            var ts = new TimeSeries(TimeInterval.OneDay, new DateTime(1954, 5, 2, 0, 0, 0), data);

            var ams = ts.AnnualMaxSeries(new[] { 1,2,3,4,5});
            var true_ams = new double[] { 1200, 3260, 3300, 3820, 2910, 3860, 4390, 5860, 3520, 2650, 7010, 1530, 4170, 2860, 2680, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2569, 5992, 2415, 3934, 2393, 1962, 3646, 2141, 6594, 5667, 3182, 8392, 2203, 4282, 3288, 2979, 2132, 3635, 3337, 3706, 3913, 3133, 2785, 6945, 2320, 5207, 5710, 2210, 4550, 4904, 2132, 1799 };
            for (int i = 0; i < true_ams.Length; i++)
                Assert.AreEqual(true_ams[i], ams[i].Value, 1E-2);
        }


        [TestMethod]
        public void ConvertTimeInterval_1hr_to_15min()
        {
            var data = new double[] { 43, 42, 42, 46, 51, 56, 58, 60, 67, 278, 278, 218, 256, 243, 225, 199, 262, 616, 1440, 2180, 3140, 4520, 4700, 4340, 4190, 3720, 3120, 2550, 2110, 1720, 1350, 1110, 950, 830, 755, 696, 664, 612, 572, 520, 458, 410, 362, 315, 273, 270, 277, 246, 247, 248, 261, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162 };
            var trueValues = new double[] { 43, 42.75, 42.5, 42.25, 42, 42, 42, 42, 42, 43, 44, 45, 46, 47.25, 48.5, 49.75, 51, 52.25, 53.5, 54.75, 56, 56.5, 57, 57.5, 58, 58.5, 59, 59.5, 60, 61.75, 63.5, 65.25, 67, 119.75, 172.5, 225.25, 278, 278, 278, 278, 278, 263, 248, 233, 218, 227.5, 237, 246.5, 256, 252.75, 249.5, 246.25, 243, 238.5, 234, 229.5, 225, 218.5, 212, 205.5, 199, 214.75, 230.5, 246.25, 262, 350.5, 439, 527.5, 616, 822, 1028, 1234, 1440, 1625, 1810, 1995, 2180, 2420, 2660, 2900, 3140, 3485, 3830, 4175, 4520, 4565, 4610, 4655, 4700, 4610, 4520, 4430, 4340, 4302.5, 4265, 4227.5, 4190, 4072.5, 3955, 3837.5, 3720, 3570, 3420, 3270, 3120, 2977.5, 2835, 2692.5, 2550, 2440, 2330, 2220, 2110, 2012.5, 1915, 1817.5, 1720, 1627.5, 1535, 1442.5, 1350, 1290, 1230, 1170, 1110, 1070, 1030, 990, 950, 920, 890, 860, 830, 811.25, 792.5, 773.75, 755, 740.25, 725.5, 710.75, 696, 688, 680, 672, 664, 651, 638, 625, 612, 602, 592, 582, 572, 559, 546, 533, 520, 504.5, 489, 473.5, 458, 446, 434, 422, 410, 398, 386, 374, 362, 350.25, 338.5, 326.75, 315, 304.5, 294, 283.5, 273, 272.25, 271.5, 270.75, 270, 271.75, 273.5, 275.25, 277, 269.25, 261.5, 253.75, 246, 246.25, 246.5, 246.75, 247, 247.25, 247.5, 247.75, 248, 251.25, 254.5, 257.75, 261, 254, 247, 240, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 215.25, 197.5, 179.75, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162 };

            var ts = new TimeSeries(TimeInterval.OneHour, new DateTime(1973, 5, 1), data);
            var newTS = ts.ConvertTimeInterval(TimeInterval.FifteenMinute);

            for (int i = 0; i < trueValues.Length; i++)
                Assert.AreEqual(trueValues[i], newTS[i].Value, 1E-2);
        }

        [TestMethod]
        public void ConvertTimeInterval_1hr_to_6hr()
        {
            var data = new double[] { 43, 42, 42, 46, 51, 56, 58, 60, 67, 278, 278, 218, 256, 243, 225, 199, 262, 616, 1440, 2180, 3140, 4520, 4700, 4340, 4190, 3720, 3120, 2550, 2110, 1720, 1350, 1110, 950, 830, 755, 696, 664, 612, 572, 520, 458, 410, 362, 315, 273, 270, 277, 246, 247, 248, 261, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162 };
            var trueValues = new double[] { 46.67, 159.83, 300.17, 3386.67, 2901.67, 948.5, 539.33, 290.5, 242.5, 233, 233, 233, 209.33, 162, 162, 162, 162 };

            var ts = new TimeSeries(TimeInterval.OneHour, new DateTime(1973, 5, 1), data);
            var newTS = ts.ConvertTimeInterval(TimeInterval.SixHour);

            for (int i = 0; i < trueValues.Length; i++)
                Assert.AreEqual(trueValues[i], newTS[i].Value, 1E-2);
        }

        [TestMethod]
        public void ConvertTimeInterval_1hr_to_1Day()
        {
            var data = new double[] { 43, 42, 42, 46, 51, 56, 58, 60, 67, 278, 278, 218, 256, 243, 225, 199, 262, 616, 1440, 2180, 3140, 4520, 4700, 4340, 4190, 3720, 3120, 2550, 2110, 1720, 1350, 1110, 950, 830, 755, 696, 664, 612, 572, 520, 458, 410, 362, 315, 273, 270, 277, 246, 247, 248, 261, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162 };
            var trueValues = new double[] { 973.33, 1170.00, 235.38, 173.83 };

            var ts = new TimeSeries(TimeInterval.OneHour, new DateTime(1973, 5, 1), data);
            var newTS = ts.ConvertTimeInterval(TimeInterval.OneDay);

            for (int i = 0; i < trueValues.Length; i++)
                Assert.AreEqual(trueValues[i], newTS[i].Value, 1E-2);
        }
    }
}
