using Numerics.Mathematics.Optimization;
using System;
using System.Collections.Generic;

namespace Numerics.Distributions.Copulas
{
    /// <summary>
    /// A class for estimating the parameters of a bivariate copula.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class BivariateCopulaEstimation
    {

        /// <summary>
        /// Estimate the bivariate copula.
        /// </summary>
        /// <param name="estimationMethod"></param>
        /// <param name="copula">The copula to estimate.</param>
        /// <param name="sampleDataX">The sample data for the X variable.</param>
        /// <param name="sampleDataY">The sample data for the Y variable.</param>
        public static void Estimate(ref BivariateCopula copula, IList<double> sampleDataX, IList<double> sampleDataY, CopulaEstimationMethod estimationMethod)
        {
            switch (estimationMethod)
            {
                case CopulaEstimationMethod.PseudoLikelihood:
                    MPL(copula, sampleDataX, sampleDataY);
                    break;
                case CopulaEstimationMethod.InferenceFromMargins:
                    IFM(copula, sampleDataX, sampleDataY);
                    break;
                case CopulaEstimationMethod.FullLikelihood:
                    MLE(copula, sampleDataX, sampleDataY);
                    break;
            }
        }

        /// <summary>
        /// The maximum pseudo likelihood method. 
        /// </summary>
        /// <param name="copula">The copula to estimate.</param>
        /// <param name="sampleDataX">The sample data for the X variable.When estimating with pseudo likelihood, this should be the plotting positions of the data.</param>
        /// <param name="sampleDataY">The sample data for the Y variable.When estimating with pseudo likelihood, this should be the plotting positions of the data.</param>
        private static void MPL(BivariateCopula copula, IList<double> sampleDataX, IList<double> sampleDataY)
        {
            // Get constraints
            var LU = copula.ParameterConstraints(sampleDataX, sampleDataY);

            // Solve using Brent method
            Func<double, double> func = (x) =>
            {
                var C = copula.Clone();
                C.Theta = x;
                return C.PseudoLogLikelihood(sampleDataX, sampleDataY);
            };
            var brent = new BrentSearch(func, LU[0], LU[1]);
            brent.Maximize();
            copula.Theta = brent.BestParameterSet.Values[0];
        }

        /// <summary>
        /// The inference from margins method. 
        /// </summary>
        /// <param name="copula">The copula to estimate.</param>
        /// <param name="sampleDataX">The sample data for the X variable.</param>
        /// <param name="sampleDataY">The sample data for the Y variable.</param>
        private static void IFM(BivariateCopula copula, IList<double> sampleDataX, IList<double> sampleDataY)
        {
            // Get constraints
            var LU = copula.ParameterConstraints(sampleDataX, sampleDataY);

            // Solve using Brent method
            Func<double, double> func = (x) =>
            {
                var C = copula.Clone();
                C.Theta = x;
                return C.IFMLogLikelihood(sampleDataX, sampleDataY);
            };
            var brent = new BrentSearch(func, LU[0], LU[1]);
            brent.Maximize();
            copula.Theta = brent.BestParameterSet.Values[0];
        }

        /// <summary>
        /// The maximum likelihood estimation method.
        /// </summary>
        /// <param name="copula">The copula to estimate.</param>
        /// <param name="sampleDataX">The sample data for the X variable.</param>
        /// <param name="sampleDataY">The sample data for the Y variable.</param>
        private static void MLE(BivariateCopula copula, IList<double> sampleDataX, IList<double> sampleDataY)
        {
            // See if marginals are estimable
            IMaximumLikelihoodEstimation margin1 = (IMaximumLikelihoodEstimation)copula.MarginalDistributionX;
            IMaximumLikelihoodEstimation margin2 = (IMaximumLikelihoodEstimation)copula.MarginalDistributionY;
            if (margin1 == null || margin2 == null) throw new ArgumentOutOfRangeException("marginal distributions", "There marginal distributions must implement the IMaximumLikelihoodEstimation interface to use this method.");

            int np1 = copula.MarginalDistributionX.NumberOfParameters;
            int np2 = copula.MarginalDistributionY.NumberOfParameters;

            // Get constraints     
            var initials = new double[1 + np1 + np2];
            var lowers = new double[1 + np1 + np2];
            var uppers = new double[1 + np1 + np2];

            // Theta
            // Get constraints
            var LU = copula.ParameterConstraints(sampleDataX, sampleDataY);
            lowers[0] = LU[0];
            uppers[0] = LU[1];
            initials[0] = 0.5 * (uppers[0] - lowers[0]);

            var con = margin1.GetParameterConstraints(sampleDataX);
            for (int i = 0; i < np1; i++)
            {
                initials[i + 1] = con.Item1[i];
                lowers[i + 1] = con.Item2[i];
                uppers[i + 1] = con.Item3[i];
            }
            con = margin2.GetParameterConstraints(sampleDataY);
            for (int i = 0; i < np2; i++)
            {
                initials[i + 1 + np1] = con.Item1[i];
                lowers[i + 1 + np1] = con.Item2[i];
                uppers[i + 1 + np1] = con.Item3[i];
            }


            // Solve using Differential Evolution
            Func<double[], double> func = (double[] x) => {
                // Set copula
                var C = copula.Clone();
                C.Theta = x[0];

                // marginal 1
                var m1 = ((UnivariateDistributionBase)copula.MarginalDistributionX).Clone();
                var p1 = new double[np1];
                for (int i = 0; i < np1; i++)
                    p1[i] = x[i + 1];
                m1.SetParameters(p1);

                // marginal 2
                var m2 = ((UnivariateDistributionBase)copula.MarginalDistributionY).Clone();
                var p2 = new double[np2];
                for (int i = 0; i < np2; i++)
                    p2[i] = x[i + 1 + np1];
                m2.SetParameters(p2);

                C.MarginalDistributionX = m1;
                C.MarginalDistributionY = m2;
                return C.LogLikelihood(sampleDataX, sampleDataY);
            };

            var DE = new DifferentialEvolution(func, lowers.Length, lowers, uppers);
            DE.Maximize();

            // Set parameters for copula and marginals
            copula.Theta = DE.BestParameterSet.Values[0];

            var par = new double[np1];
            for (int i = 0; i < np1; i++)
                par[i] = DE.BestParameterSet.Values[i + 1];
            copula.MarginalDistributionX.SetParameters(par);

            par = new double[np1];
            for (int i = 0; i < np2; i++)
                par[i] = DE.BestParameterSet.Values[i + 1 + np1];
            copula.MarginalDistributionY.SetParameters(par);

        }

    }
}
