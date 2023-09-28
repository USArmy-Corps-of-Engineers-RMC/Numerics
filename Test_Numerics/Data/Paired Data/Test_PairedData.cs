using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;
using Numerics.Distributions;

namespace Data.PairedData
{
    [TestClass]
    public class Test_PairedData
    {

        private readonly OrderedPairedData _dataset1;
        private readonly OrderedPairedData _dataset2;
        private readonly OrderedPairedData _dataset3;
        private readonly OrderedPairedData _dataset4;
        public Test_PairedData()
        {
            double[] xVals = new double[] { 230408, 288010, 345611, 403213, 460815, 518417, 576019, 633612, 691223, 748825, 806427, 864029, 921631, 1036834, 1152038 };
            double[] yVals = new double[] { 1519.7, 1520.5, 1520.9, 1521.7, 1523.5, 1525.9, 1528.4, 1530.9, 1533.2, 1534.7, 1535.9, 1538, 1541.3, 1547.7, 1552.7 };
            _dataset1 = new OrderedPairedData(xVals, yVals, true, SortOrder.Ascending, true, SortOrder.Ascending);
            _dataset2 = new OrderedPairedData(xVals.Reverse().ToArray(), yVals, true, SortOrder.Descending, true, SortOrder.Ascending);
            _dataset3 = new OrderedPairedData(xVals, yVals.Reverse().ToArray(), true, SortOrder.Ascending, true, SortOrder.Descending);
            _dataset4 = new OrderedPairedData(xVals.Reverse().ToArray(), yVals.Reverse().ToArray(), true, SortOrder.Descending, true, SortOrder.Descending);

        }

        [TestMethod()]
        public void Test_ReadWriteXElement()
        {
            UncertainOrderedPairedData originalUncertainCurve = new UncertainOrderedPairedData(false, SortOrder.Ascending, true, SortOrder.Ascending, UnivariateDistributionType.Triangular);
            originalUncertainCurve.Add(new UncertainOrdinate(0, new Triangular(1, 2, 3)));
            originalUncertainCurve.Add(new UncertainOrdinate(2, new Triangular(2, 4, 5)));
            originalUncertainCurve.Add(new UncertainOrdinate(3, new Triangular(6, 8, 12)));
            originalUncertainCurve.Add(new UncertainOrdinate(5, new Triangular(13, 19, 20)));

            var el = originalUncertainCurve.SaveToXElement();
            UncertainOrderedPairedData newUncertainCurve = new UncertainOrderedPairedData(el);
            Assert.AreEqual((originalUncertainCurve == newUncertainCurve), true);
        }

        [TestMethod()]
        public void Test_GetY()
        {
            double[] xVals = new double[] { 1018627, 742619, 770076, 350167, 260164, 502421, 1034555, 810438, 655158, 253951, 1149424, 973525, 397450, 1128872, 640330, 303494, 668286, 373493, 731302, 518190, 553336, 897626, 682656, 580591, 915612, 949596, 411371, 660301, 793095, 651386, 289081, 958856, 346709, 523205, 930560, 500896, 1056651, 581860, 653012, 1000323, 592979, 1042845, 418936, 494984, 447825, 653758, 250286, 233175, 532844, 567222, 508867, 941194, 607007, 275898, 565432, 654433, 1138135, 465458, 433568, 983439, 765617, 810132, 570295, 1135444, 1039781, 346336, 851548, 607131, 312623, 836800, 348436, 756059, 275085, 903306, 955898, 374842, 597348, 741079 };
            double[] yVals = new double[] { 1546.68852634046, 1534.53839102809, 1535.14271379466, 1520.96327558071, 1520.11326342835, 1525.23352314156, 1547.57339218597, 1536.04622929759, 1531.76017947961, 1520.02697475782, 1552.58654907816, 1544.18292492383, 1521.61996111246, 1551.6945661609, 1531.16820225304, 1520.60752591101, 1532.28428772283, 1521.28723655429, 1534.24368771918, 1525.89054199507, 1527.41552897469, 1539.92476129301, 1532.85798024683, 1528.5984616186, 1540.95517343148, 1542.85357065354, 1521.95492864831, 1531.96550311572, 1535.62225964376, 1531.60959018243, 1520.50743737088, 1543.36800170134, 1520.9152494705, 1526.10780528454, 1541.79604263778, 1525.16998368112, 1548.56008298323, 1528.65354643794, 1531.67450486886, 1545.67166393236, 1529.13620057993, 1547.96088503871, 1522.19132669005, 1524.92365890073, 1523.09407659456, 1531.70428737567, 1519.97607374744, 1519.73842922121, 1526.52615013368, 1528.01819902087, 1525.5020971494, 1542.38680503112, 1529.74512874829, 1520.33178361862, 1527.94051074615, 1531.73123535436, 1552.09659213222, 1523.69345161626, 1522.64856081386, 1544.73368835881, 1535.04982118676, 1536.03507343495, 1528.151571126, 1551.97979931252, 1547.82790354502, 1520.91006909482, 1537.544979341, 1529.75051134687, 1520.67092064374, 1537.00731051005, 1520.93923474879, 1534.85070310059, 1520.32049234402, 1540.25016666088, 1543.20367264741, 1521.30597201486, 1529.32585036376, 1534.49828825388 };
            SampleDataset(_dataset1, xVals, false, yVals);
            SampleDataset(_dataset4, xVals, false, yVals);
            yVals = new double[] { 1520.56321710372, 1526.16934828652, 1525.01457241068, 1541.03898822958, 1550.11710357279, 1535.03323842922, 1520.50791298838, 1523.37466060206, 1529.96502230477, 1550.65640776362, 1519.71815214749, 1520.71981719226, 1538.3301604111, 1519.86086941426, 1530.60847581191, 1545.97958542386, 1529.39533943171, 1539.70264921357, 1526.66052046804, 1534.70472900247, 1533.79068261519, 1521.23339120169, 1528.77176060127, 1533.01741531089, 1520.98359431964, 1520.80290183415, 1537.70258324364, 1529.74184443943, 1524.05548071248, 1530.12870632345, 1547.58100206594, 1520.77074989367, 1541.23709593417, 1534.57531682928, 1520.86899733514, 1535.06500815944, 1520.36238672268, 1532.9667372771, 1530.05814688167, 1520.62677100423, 1532.52269546646, 1520.45825839381, 1537.42678552828, 1535.18817054963, 1536.37357730634, 1530.02577459166, 1550.9745390785, 1552.45981736745, 1534.32430991979, 1533.42908058748, 1534.8989514253, 1520.83207468556, 1531.96248155158, 1548.7513523836, 1533.47569355231, 1529.99648331048, 1519.79654525885, 1535.80327419187, 1536.89334571716, 1520.68539447757, 1525.20035762647, 1523.38422277004, 1533.3490573244, 1519.81523211, 1520.4795354328, 1541.25846498385, 1522.09001770772, 1531.95752956088, 1544.96526970018, 1522.55087670567, 1541.13815666123, 1525.59859379883, 1548.8219228499, 1521.15450505191, 1520.78102045954, 1539.6253654387, 1532.34821766534, 1526.23618624353 };
            SampleDataset(_dataset2, xVals, false, yVals);
            SampleDataset(_dataset3, xVals, false, yVals);
            //




        }

        private void SampleDataset(OrderedPairedData data, double[] samples, bool samplesOrdered, double[] expectedResults)
        {
            for (int i = 0; i < samples.Count(); i++)
                Assert.AreEqual(Math.Abs(data.GetYfromX(samples[i]) - expectedResults[i]) <= 0.00000001, true);

            double[] interps = data.GetYValues(samples, samplesOrdered);
            for (int i = 0; i < samples.Count(); i++)
            {
                Assert.AreEqual(Math.Abs(interps[i] - expectedResults[i]) <= 0.00000001, true);
            }
        }
    }
}
