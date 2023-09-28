using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;

namespace Data.Interpolation
{
    [TestClass]
    public class Test_Polynomial
    {

        [TestMethod]
        public void Test_Interpolate_order3()
        {
            var XArray = new double[] { 6d, 24d, 48d, 72d };
            var YArray = new double[] { 9.96d, 22.13d, 32.27d, 37.60d };
            var poly = new Polynomial(3, XArray, YArray);
            double X = 8d;
            double Y = poly.Interpolate(X);
            Assert.AreEqual(Y, 11.5415808882467, 1E-6);
        }

        [TestMethod]
        public void Test_Interpolate_List()
        {
            var XArray = new double[] { 6d, 24d, 48d, 72d };
            var YArray = new double[] { 9.96d, 22.13d, 32.27d, 37.60d };
            var poly = new Polynomial(3, XArray, YArray);
            var X = new double[34];
            X[0] = 6;
            for (int i = 1; i < X.Length; i++)
                X[i] = X[i - 1] + 2;

            var Y = poly.Interpolate(X);
            var true_Y = new double[] { 9.95999999999906, 11.5415808882467, 13.0626606341182, 14.5245941558435, 15.9287363716526, 17.2764421997752, 18.5690665584413, 19.807964365881, 20.994490540324, 22.1300000000003, 23.2158476631398, 24.2533884479724, 25.2439772727281, 26.1889690556368, 27.0897187149283, 27.9475811688326, 28.7639113355797, 29.5400641333994, 30.2773944805217, 30.9772572951765, 31.6410074955936, 32.2700000000031, 32.8655897266348, 33.4291315937186, 33.9619805194845, 34.4654914221624, 34.9410192199823, 35.3899188311739, 35.8135451739673, 36.2132531665923, 36.5903977272789, 36.9463337742571, 37.2824162257566, 37.6000000000075, 37.9004400152396, 38.1850911896829, 38.4553084415673, 38.7124466891227, 38.9578608505791, 39.1929058441663 };
            for (int i = 0; i < X.Length; i++)
                Assert.AreEqual(Y[i], true_Y[i], 1E-6);
        }
    }
}
