using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;

namespace Data.TimeSeriesAnalysis
{
    [TestClass]
    public class Test_TimeSeries
    {
        [TestMethod]
        public void Test_MovingAverage()
        {

            var t = TimeSeries.DownloadfromUSGS("08133500");

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

    }
}
