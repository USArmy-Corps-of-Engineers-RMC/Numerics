using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data.Statistics;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_EmpiricalDistribution
    {

       [TestMethod]
        public void Test_Empirical()
        {
            var xValues = new double[] { 60, 62.4, 64.8, 67.2, 69.6, 72, 74.4, 76.8, 79.2, 81.6, 84, 86.4, 88.8, 91.2, 93.6, 96, 98.4, 100.8, 103.2, 105.6, 108, 110.4, 112.8, 115.2, 117.6, 120, 122.4, 124.8, 127.2, 129.6, 132, 134.4, 136.8, 139.2, 141.6, 144, 146.4, 148.8, 151.2, 153.6, 156, 158.4, 160.8, 163.2, 165.6, 168, 170.4, 172.8, 175.2, 177.6, 180 };
            var yValues = new double[] { 0.00656255299999997, 0.011235394, 0.0182305659999999, 0.028188409, 0.041732992, 0.0594110450000001, 0.0816336, 0.10862863, 0.140410952, 0.176772673, 0.217294242, 0.261373384, 0.308267235, 0.357142059, 0.407124886, 0.457352183, 0.507011821, 0.555375994, 0.60182409, 0.645855616, 0.687094171, 0.72528395, 0.760280572, 0.792038023, 0.820593355, 0.846050583, 0.868564861, 0.888327776, 0.905554271, 0.920471492, 0.933309648, 0.944294835, 0.953643664, 0.961559494, 0.968230023, 0.973825984, 0.978500726, 0.982390458, 0.985614972, 0.988278692, 0.99047192, 0.992272179, 0.993745587, 0.994948194, 0.995927251, 0.99672239, 0.997366695, 0.997887667, 0.998308071, 0.998646683, 0.998918935 };

            var zValues = new double[] { 0.0065625533783763, 0.0190603864061158, 0.0446793337039374, 0.0883823576250979, 0.152531525445008, 0.235660550894875, 0.332704646887913, 0.436424142578906, 0.53925460323174, 0.634847734825551, 0.718918986533906, 0.789391568445138, 0.846050583035643, 0.889763316925072, 0.92261118427365, 0.946632041977346, 0.963783003861332, 0.975773793010283, 0.984002715097366 };
            var true_values = new double[] { 60, 65, 70, 75, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150 };
            double[] output = new double[zValues.Length];
            var emp = new EmpiricalDistribution(xValues, yValues);

            for (int i = 0; i < zValues.Length; i++)
            {
                output[i] = emp.InverseCDF(zValues[i]);
            }



        }

        [TestMethod]
        public void Test_SRP()
        {
            var xValues = new double[] { 535, 563.6, 574.3, 582.8, 587.7 };
            var yValues = new double[] { 0.00000002142, 0.0000014994, 0.00000745416, 0.0000369852, 0.0001272348 };


            var emp = new EmpiricalDistribution(xValues, yValues);

            var p = emp.CDF(525.5);




        }



        [TestMethod]
        public void TestMethod1()
        {

            var data = new double[] { 57985, 14607, 8403, 23479, 66629, 30925, 25046, 37772, 9074, 21382, 43769, 50678, 7360, 15617, 23189, 12637, 30064, 52914, 22607, 14191, 19098, 35383, 25046, 13355, 20330, 25701, 26897, 20019, 18240, 21084, 28886, 17032, 45014, 12637, 17037, 9074, 36066, 15261, 23886, 22432, 55536, 43938, 22490, 26955, 53417, 36920, 42584, 39587, 12996, 30294, 8952, 28109, 12637, 15142, 16624, 81464, 13952, 40946, 29317, 24930, 42076, 26551, 30122, 32586, 34357, 32586, 45065, 29547, 14310, 44107, 15676, 18922, 17981, 40097, 42471, 17451, 14964, 20798, 19157, 33729, 58319, 35041, 32700, 35212, 34585, 46921, 15795, 42189, 55982, 34585, 48324 };
            var emp = new EmpiricalDistribution(data);
            var hist = new Histogram(data);

            var LP3 = new LogPearsonTypeIII();
            LP3.Estimate(data, ParameterEstimationMethod.MaximumLikelihood);

            //var p3PDF = LP3.CreatePDFGraph();
            //for (int i = 0; i < p3PDF.Length; i++)
            //{
            //    Debug.WriteLine(p3PDF[i, 0] + "," + p3PDF[i, 1]);
            //}



            double sum = 0;
            double sum2 = 0;
            for (int i = 0; i < hist.NumberOfBins; i++)
            {
                sum += (double)hist[i].Frequency * hist.BinWidth;
                sum2 += emp.PDF(hist[i].Midpoint) * hist.BinWidth;
            }
            for (int i = 0; i < hist.NumberOfBins; i++)
            {
                var epdf = emp.PDF(hist[i].Midpoint) / sum2;
               
                var hpdf = (double)hist[i].Frequency / sum;
                Debug.WriteLine(hist[i].Midpoint + "," + epdf );
            }
        }
    }
}
