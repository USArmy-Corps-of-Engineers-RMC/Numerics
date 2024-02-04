using Numerics.Distributions;
using Numerics.Mathematics.SpecialFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numerics.Data.Statistics
{

    /// <summary>
    /// A class for performing probability calculations for risk analysis. 
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class Probability
    {
        /// <summary>
        /// Enumeration of dependency types. 
        /// </summary>
        public enum DependencyType
        {
            Independent,
            PerfectlyPositive,
            PerfectlyNegative
        }

        #region Basic Probability Rules for Two Random Variables

        /// <summary>
        /// Returns the probability of intersection (or joint probability) of A and B, P(A and B). 
        /// </summary>
        /// <param name="A">Marginal probability of A.</param>
        /// <param name="B">Marginal probability of B.</param>
        /// <param name="rho">Pearson's correlation coefficient. Default = 0.</param>
        public static double AandB(double A, double B, double rho = 0d)
        {
            if (rho == 0) return A * B;
            if (rho == 1) return Math.Min(A, B);
            return MultivariateNormal.BivariateCDF(Normal.StandardZ(1 - A), Normal.StandardZ(1 - B), rho);
        }

        /// <summary>
        /// Returns the probability of union of A and B, P(A or B).
        /// </summary>
        /// <param name="A">Marginal probability of A.</param>
        /// <param name="B">Marginal probability of B.</param>
        /// <param name="rho">Pearson's correlation coefficient. Default = 0.</param>
        public static double AorB(double A, double B, double rho = 0d)
        {
            return A + B - AandB(A, B, rho);
        }

        /// <summary>
        /// Returns the probability of A and not B, P(A and not B).
        /// </summary>
        /// <param name="A">Marginal probability of A.</param>
        /// <param name="B">Marginal probability of B.</param>
        /// <param name="rho">Pearson's correlation coefficient. Default = 0.</param>
        public static double AnotB(double A, double B, double rho = 0d)
        {
            return A - AandB(A, B, rho);
        }

        /// <summary>
        /// Returns the probability of B and not A, P(B and not A).
        /// </summary>
        /// <param name="A">Marginal probability of A.</param>
        /// <param name="B">Marginal probability of B.</param>
        /// <param name="rho">Pearson's correlation coefficient. Default = 0.</param>
        public static double BnotA(double A, double B, double rho = 0d)
        {
            return B - AandB(A, B, rho);
        }

        /// <summary>
        /// Returns the probability of A given B, P(A|B).
        /// </summary>
        /// <param name="A">Marginal probability of A.</param>
        /// <param name="B">Marginal probability of B.</param>
        /// <param name="rho">Pearson's correlation coefficient. Default = 0.</param>
        public static double AgivenB(double A, double B, double rho = 0d)
        {
            return AandB(A, B, rho) / B;
        }

        /// <summary>
        /// Returns the probability of B given A, P(B|A).
        /// </summary>
        /// <param name="A">Marginal probability of A.</param>
        /// <param name="B">Marginal probability of B.</param>
        /// <param name="rho">Pearson's correlation coefficient. Default = 0.</param>
        public static double BgivenA(double A, double B, double rho = 0d)
        {
            return AandB(A, B, rho) / A;
        }

        #endregion

        #region Joint Probability

        /// <summary>
        /// Compute the joint probability.
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        /// <param name="dependency">The dependency type. Default = Independent.</param>
        public static double JointProbability(IList<double> probabilities, DependencyType dependency = DependencyType.Independent)
        {
            if (dependency == DependencyType.Independent)
            {
                return IndependentJointProbability(probabilities);
            }
            else if (dependency == DependencyType.PerfectlyPositive)
            {
                return PositiveJointProbability(probabilities);
            }
            else if (dependency == DependencyType.PerfectlyNegative)
            {
                return NegativeJointProbability(probabilities);
            }
            return double.NaN;
        }

        /// <summary>
        /// Returns the joint probability assuming perfect independence. 
        /// </summary>
        /// <param name="probabilities">An array of probabilities for each event.</param>
        public static double IndependentJointProbability(IList<double> probabilities)
        {
            double p = 1;
            for (int i = 0; i < probabilities.Count; i++)
            {
                p *= probabilities[i];
            }
            return p;
        }

        /// <summary>
        /// Returns the joint probability assuming perfect independence. 
        /// </summary>
        /// <param name="probabilities">An array of probabilities for each event.</param>
        /// <param name="indicators">An array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        public static double IndependentJointProbability(IList<double> probabilities, int[] indicators)
        {
            return Tools.Product(probabilities, indicators);
        }

        /// <summary>
        /// Returns the joint probability assuming perfect positive dependence. 
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        public static double PositiveJointProbability(IList<double> probabilities)
        {
            return Tools.Min(probabilities);
        }

        /// <summary>
        /// Returns the joint probability assuming perfect positive dependence. 
        /// </summary>
        /// <param name="probabilities">An array of probabilities for each event.</param>
        /// <param name="indicators">An array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        public static double PositiveJointProbability(IList<double> probabilities, int[] indicators)
        {
            return Tools.Min(probabilities, indicators);
        }

        /// <summary>
        /// Returns the joint probability assuming perfect negative dependence. 
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        public static double NegativeJointProbability(IList<double> probabilities)
        {
            return Math.Max(0, Tools.Sum(probabilities) - 1);
        }

        /// <summary>
        /// Returns the joint probability assuming perfect negative dependence. 
        /// </summary>
        /// <param name="probabilities">An array of probabilities for each event.</param>
        /// <param name="indicators">An array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        public static double NegativeJointProbability(IList<double> probabilities, int[] indicators)
        {
            return Math.Max(0, Tools.Sum(probabilities, indicators) - 1);
        }

        /// <summary>
        /// Returns the joint probability of multiple events with dependency, using the Product of Conditional Marginals (PCM) method.
        /// </summary>
        /// <param name="probabilities">An array of probabilities for each event.</param>
        /// <param name="indicators">An array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution for computing the joint probability.</param>
        public static double JointProbabilityPCM(IList<double> probabilities, int[] indicators, MultivariateNormal multivariateNormal, double[] conditionalProbabilities = null)
        {
            // Get z-values
            var R = multivariateNormal.Covariance;
            int n = indicators.Length;
            int i, j, k, ir, ic;
            double A1, B1;
            for (i = 0; i < n; i++)
            {
                if (indicators[i] == 0)
                {
                    R[i, i] = Normal.StandardZ(1);
                }
                else
                {
                    R[i, i] = Normal.StandardZ(probabilities[i]);
                }
            }
            // Update the conditional correlation matrix
            // First cycle
            A1 = Normal.StandardPDF(R[0, 0]) / Normal.StandardCDF(R[0, 0]);
            B1 = A1 * (R[0, 0] + A1);
            // calculate c[k|0] and store them in R[k,0], k = 1,...,n
            for (k = 1; k < n; k++)
            {
                R[k,0] = (R[k, k] + R[0, k] * A1) / Math.Sqrt(1d - R[0, k] * R[0, k] * B1);
            }
            // update r[ir|ic] and store them in R[ir,ic]
            for (ir = 1; ir < n - 1; ir++)
            {
                for (ic = ir + 1; ic < n; ic++)
                {
                    R[ir, ic] = (R[ir, ic] - R[0, ir] * R[0, ic] * B1)/ Math.Sqrt((1d - R[0, ir] * R[0, ir] * B1) * (1d - R[0, ic] * R[0, ic] * B1));
                }
            }
            // Remaining cycles
            for (j = 1; j < n - 1; j++)
            {
                A1 = Normal.StandardPDF(R[j, j - 1]) / Normal.StandardCDF(R[j, j - 1]);
                B1 = A1 * (R[j, j - 1] + A1);
                for (k = j + 1; k < n; k++)
                {
                    R[k, j] = (R[k, j - 1] + R[j, k] * A1) / Math.Sqrt(1d - R[j, k] * R[j, k] * B1);
                }
                for (ir = j + 1; ir < n - 1; ir++)
                {
                    for (ic = ir + 1; ic < n; ic++)
                    {
                        R[ir, ic] = (R[ir, ic] - R[j, ir] * R[j, ic] * B1) / Math.Sqrt((1d - R[j, ir] * R[j, ir] * B1) * (1d - R[j, ic] * R[j, ic] * B1));
                    }
                }
            }
            // Calculate the product of conditional marginals (PCM)
            double jp = Math.Log(Normal.StandardCDF(R[0, 0]));
            if (conditionalProbabilities != null && conditionalProbabilities.Length == n) 
                conditionalProbabilities[0] = Normal.StandardCDF(R[0, 0]);
            for (i = 1; i < n; i++)
            {
                jp += Math.Log(Normal.StandardCDF(R[i, i - 1]));
                if (conditionalProbabilities != null && conditionalProbabilities.Length == n) 
                    conditionalProbabilities[i] = Normal.StandardCDF(R[i, i - 1]);
            }
            jp = Math.Exp(jp);
            jp = Math.Min(1, Math.Max(0, jp));
            if (double.IsNaN(jp)) jp = 0;
            return jp;
        }

        /// <summary>
        /// Returns an array of joint probabilities of multiple events with dependency, using the Product of Conditional Marginals (PCM) method.
        /// </summary>
        /// <param name="probabilities">And array of probabilities for each event.</param>
        /// <param name="indicators">An 2D array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution for computing the joint probability.</param>
        public static double[] JointProbabilitiesPCM(IList<double> probabilities, int[,] indicators, MultivariateNormal multivariateNormal)
        {
            var result = new double[indicators.GetLength(0)];

            Parallel.For(0, indicators.GetLength(0), idx =>
            {
                if (idx < probabilities.Count)
                {
                    result[idx] = probabilities[idx];
                }
                else
                {
                    result[idx] = JointProbabilityPCM(probabilities, indicators.GetRow(idx), (MultivariateNormal)multivariateNormal.Clone());
                }
            });
            return result;
        }

        /// <summary>
        /// Returns the joint probability of multiple events with dependency. 
        /// </summary>
        /// <param name="probabilities">An array of probabilities for each event.</param>
        /// <param name="indicators">An array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution for computing the joint probability.</param>
        public static double JointProbability(IList<double> probabilities, int[] indicators, MultivariateNormal multivariateNormal)
        {
            var zVals = new double[indicators.Length];
            for (int i = 0; i < indicators.Length; i++)
            {
                if (indicators[i] == 0)
                {
                    zVals[i] = double.PositiveInfinity;
                }
                else
                {
                    zVals[i] = Normal.StandardZ(probabilities[i]);
                }
            }
            var p = multivariateNormal.CDF(zVals);
            return p;
        }

        /// <summary>
        /// Returns an array of joint probabilities of multiple events with dependency.
        /// </summary>
        /// <param name="probabilities">An array of probabilities for each event.</param>
        /// <param name="indicators">An 2D array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution for computing the joint probability.</param>
        public static double[] JointProbabilities(IList<double> probabilities, int[,] indicators, MultivariateNormal multivariateNormal)
        {
            var result = new double[indicators.GetLength(0)];

            Parallel.For(0, indicators.GetLength(0), idx =>
            {
                if (idx < probabilities.Count)
                {
                    result[idx] = probabilities[idx];
                }
                else
                {
                    result[idx] = JointProbability(probabilities, indicators.GetRow(idx), (MultivariateNormal)multivariateNormal.Clone());
                }           
            });
            return result;
        }

        #endregion

        #region Probability of Union

        /// <summary>
        /// Compute the probability of union.
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        /// <param name="dependency">The dependency type. Default = Independent.</param>
        public static double Union(IList<double> probabilities, DependencyType dependency = DependencyType.Independent)
        {
            if (dependency == DependencyType.Independent)
            {
                return IndependentUnion(probabilities);
            }
            else if (dependency == DependencyType.PerfectlyPositive)
            {
                return PositivelyDependentUnion(probabilities);
            }
            else if (dependency == DependencyType.PerfectlyNegative)
            {
                return NegativelyDependentUnion(probabilities);
            }
            return double.NaN;
        }

        /// <summary>
        /// Returns the probability of union assuming independence (De Morgan's Rule).
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        public static double IndependentUnion(IList<double> probabilities)
        {
            if (probabilities.Count == 1) return probabilities[0];
            double numerator = 1d;
            for (int i = 0; i < probabilities.Count; i++)
                numerator *= 1d - probabilities[i];
            return 1d - numerator;
        }

        /// <summary>
        /// Returns the unimodal bound for the probability of union assuming perfect positive dependence. 
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        public static double PositivelyDependentUnion(IList<double> probabilities)
        {
            return Tools.Max(probabilities);
        }

        /// <summary>
        /// Returns the unimodal bound for the probability of union assuming perfect negative dependence. 
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        public static double NegativelyDependentUnion(IList<double> probabilities)
        {
            return Math.Min(1, Tools.Sum(probabilities));
        }

        /// <summary>
        /// Returns the probability of union using the inclusion-exclusion method. Dependence between events is captured with the multivariate normal distribution.
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution used to compute the joint probabilities.</param>
        /// <param name="tolerance">The tolerance level for inclusion-exclusion convergence. Default = 1E-8</param>
        public static double UnionPCM(IList<double> probabilities, MultivariateNormal multivariateNormal, double tolerance = 1E-8)
        {

            double result = 0;

            // Get number of unique combinations by subset
            int N = probabilities.Count;
            var binomialCombinations = new int[N];
            for (int i = 1; i <= N; i++)
            {
                binomialCombinations[i - 1] = (int)Factorial.BinomialCoefficient(N, i);
            }

            // Get combination indicators
            var indicators = Factorial.AllCombinations(N);

            double s = 1;
            int j = 0;
            int c = binomialCombinations[j];
            double inc = double.NaN;
            double exc = double.NaN;

            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                if (i == c)
                {
                    if (j > 0 && s == 1)
                    {
                        inc = result;
                    }
                    else if (j > 0 && s == -1)
                    {
                        exc = result;
                    }

                    // Check convergence and early exit
                    // The tolerance is that each inclusion-exclusion step have less than 
                    // 1E-8 difference between them.
                    double diff = Math.Abs(inc - exc);
                    double tol = tolerance * Math.Min(inc, exc);
                    if (j > 0 && diff <= tolerance)
                        return result + 0.5 * diff;
                    
                    s *= -1;
                    j++;
                    if (j < binomialCombinations.Length)
                    {
                        c += binomialCombinations[j];
                    }
                        
                }
                if (i < probabilities.Count)
                {
                    result += s * probabilities[i];
                }
                else
                {
                    result += s * JointProbabilityPCM(probabilities, indicators.GetRow(i), multivariateNormal);
                }


            }

            return result;
        }

        /// <summary>
        /// Returns the probability of union using the inclusion-exclusion method. Dependence between events is captured with the multivariate normal distribution.
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        /// <param name="binomialCombinations">An array of binomial combinations.</param>
        /// <param name="indicators">An 2D array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution used to compute the joint probabilities.</param>
        public static double UnionPCM(IList<double> probabilities, int[] binomialCombinations, int[,] indicators, MultivariateNormal multivariateNormal)
        {

            double result = 0;
            double s = 1;
            int j = 0;
            int c = binomialCombinations[j];

            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                if (i == c)
                {
                    s *= -1;
                    j++;
                    if (j < binomialCombinations.Length) c += binomialCombinations[j];
                }
                if (i < probabilities.Count)
                {
                    result += s * probabilities[i];
                }
                else
                {
                    result += s * JointProbabilityPCM(probabilities, indicators.GetRow(i), multivariateNormal);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the probability of union using the inclusion-exclusion method. Dependence between events is captured with the multivariate normal distribution.
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution used to compute the joint probabilities.</param>
        public static double Union(IList<double> probabilities, MultivariateNormal multivariateNormal)
        {

            double result = 0;

            // Get number of unique combinations by subset
            int N = probabilities.Count;
            var binomialCombinations = new int[N];
            for (int i = 1; i <= N; i++)
            {
                binomialCombinations[i - 1] = (int)Factorial.BinomialCoefficient(N, i);
            }

            // Get combination indicators
            var indicators = Factorial.AllCombinations(N);

            double s = 1;
            int j = 0;
            int c = binomialCombinations[j];

            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                if (i == c)
                {
                    s *= -1;
                    j++;
                    if (j < binomialCombinations.Length) c += binomialCombinations[j];
                }
                if (i < probabilities.Count)
                {
                    result += s * probabilities[i];
                }
                else
                {
                    result += s * JointProbability(probabilities, indicators.GetRow(i), multivariateNormal);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the probability of union using the inclusion-exclusion method. Dependence between events is captured with the multivariate normal distribution.
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        /// <param name="binomialCombinations">An array of binomial combinations.</param>
        /// <param name="indicators">An 2D array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution used to compute the joint probabilities.</param>
        public static double Union(IList<double> probabilities, int[] binomialCombinations, int[,] indicators, MultivariateNormal multivariateNormal)
        {

            double result = 0;
            double s = 1;
            int j = 0;
            int c = binomialCombinations[j];

            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                if (i == c)
                {
                    s *= -1;
                    j++;
                    if (j < binomialCombinations.Length) c += binomialCombinations[j];
                }
                if (i < probabilities.Count)
                {
                    result += s * probabilities[i];
                }
                else
                {
                    result += s * JointProbability(probabilities, indicators.GetRow(i), multivariateNormal);
                }
            }

            return result;
        }

        #endregion

        #region Exclusive Probability of all Combinations of Events

        #region Independent
        
        /// <summary>
        /// Returns the exclusive probability of multiple events occurring assuming independence.
        /// </summary>
        /// <param name="probabilities">And array of probabilities for each event.</param>
        /// <param name="indicators">An array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        public static double IndependentExclusive(IList<double> probabilities, int[] indicators)
        {
            double result = 1;
            for (int i = 0; i < probabilities.Count; i++)
            {
                if (double.IsNaN(probabilities[i])) return double.NaN;
                if (indicators[i] == 1)
                {
                    result *= probabilities[i];
                }
                else
                {
                    result *= (1 - probabilities[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns an array of exclusive probabilities of multiple events occurring assuming independence.
        /// </summary>
        /// <param name="probabilities">An array of probabilities for each event.</param>
        /// <param name="indicators">An 2D array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        public static double[] IndependentExclusive(IList<double> probabilities, int[,] indicators)
        {
            var result = new double[indicators.GetLength(0)];
            for (int i = 0; i < indicators.GetLength(0); i++)
            {             
                result[i] = IndependentExclusive(probabilities, indicators.GetRow(i));
            }
            return result;         
        }

        /// <summary>
        /// Returns an array of exclusive probabilities of multiple events occurring assuming independence.
        /// </summary>
        /// <param name="probabilities">And array of probabilities for each event.</param>
        public static double[] IndependentExclusive(IList<double> probabilities)
        {
            int n = probabilities.Count;
            int f = (int)Math.Pow(2, n) - 1;
            var result = new double[f];
            int t = 0;
            for (int i = 1; i <= n; i++)
            {
                foreach (int[] combos in Factorial.FindCombinations(i, n))
                {
                    var indicators = new int[n];
                    for (int j = 0; j < combos.Length; j++)
                    {
                        indicators[combos[j]] = 1;
                    }
                    result[t] = IndependentExclusive(probabilities, indicators);
                    t++;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a list of exclusive probabilities of multiple events occurring assuming independence.
        /// </summary>
        /// <param name="probabilities">An array of probabilities for each event.</param>
        /// <param name="binomialCombinations">An array of binomial combinations.</param>
        /// <param name="indicators">An 2D array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        /// <param name="eventProbabilities">Output. A list of exclusive event probabilities.</param>
        /// <param name="eventIndicators">Output. A list of exclusive event indicators that were evaluated.</param>
        /// <param name="absoluteTol">The absolute tolerance for evaluation convergence of the inclusion-exclusion algorithm. Default = 1E-8.</param>
        /// <param name="relativeTol">The realtive tolerance for evaluation convergence of the inclusion-exclusion algorithm. Default = 1E-4.</param>
        public static void IndependentExclusive(IList<double> probabilities, int[] binomialCombinations, int[,] indicators, out List<double> eventProbabilities, out List<int[]> eventIndicators, double absoluteTol = 1E-8, double relativeTol = 1E-4)
        {
            eventProbabilities = new List<double>();
            eventIndicators = new List<int[]>();

            double union = 0;
            double s = 1;
            int j = 0;
            int c = binomialCombinations[j];
            double inc = double.NaN;
            double exc = double.NaN;

            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                if (i == c)
                {
                    if (j > 0 && s == 1)
                    {
                        inc = union;
                    }
                    else if (j > 0 && s == -1)
                    {
                        exc = union;
                    }

                    // Check for convergence
                    // Add the tolerance to the last joint event
                    double diff = Math.Abs(inc - exc);
                    double tol = absoluteTol + relativeTol * Math.Min(inc, exc);
                    if (j > 0 && j < binomialCombinations.Length && diff <= tol)
                    {
                        eventIndicators.Add(indicators.GetRow(indicators.GetLength(0) - 1));
                        eventProbabilities.Add(0.5 * diff);
                        return;
                    }

                    s *= -1;
                    j++;
                    if (j < binomialCombinations.Length)
                    {
                        c += binomialCombinations[j];
                    }

                }

                // Record indicators
                eventIndicators.Add(indicators.GetRow(i));
                // Compute the exclusive event probability
                eventProbabilities.Add(IndependentExclusive(probabilities, eventIndicators.Last()));
                // Compute the union
                if (i < probabilities.Count)
                {
                    union += s * probabilities[i];
                }
                else
                {
                    union += s * IndependentJointProbability(probabilities, eventIndicators.Last());
                }
                
            }

        }

        #endregion

        #region Positively Dependent

        /// <summary>
        /// Returns the exclusive probability of multiple events occurring assuming perfect positive dependence.
        /// </summary>
        /// <param name="probabilities">And array of probabilities for each event.</param>
        /// <param name="indicators">An array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        public static double PositivelyDependentExclusive(IList<double> probabilities, int[] indicators)
        {
            double min = 1.0;
            double max = 0.0;
            for (int i = 0; i < probabilities.Count; i++)
            {
                if (double.IsNaN(probabilities[i])) return double.NaN;
                if (indicators[i] == 1)
                {
                    if (probabilities[i] < min) min = probabilities[i];
                }
                else
                {
                    if (probabilities[i] > max) max = probabilities[i];
                }
            }
            return Math.Max(min - max, 0);
        }

        /// <summary>
        /// Returns an array of exclusive probabilities of multiple events occurring assuming perfect positive dependence.
        /// </summary>
        /// <param name="probabilities">And array of probabilities for each event.</param>
        /// <param name="indicators">An 2D array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        public static double[] PositivelyDependentExclusive(IList<double> probabilities, int[,] indicators)
        {
            var result = new double[indicators.GetLength(0)];
            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                result[i] = PositivelyDependentExclusive(probabilities, indicators.GetRow(i));
            }
            return result;
        }

        /// <summary>
        /// Returns an array of exclusive probabilities of multiple events occurring assuming perfect positive dependence.
        /// </summary>
        /// <param name="probabilities">And array of probabilities for each event.</param>
        public static double[] PositivelyDependentExclusive(IList<double> probabilities)
        {
            int n = probabilities.Count;
            int f = (int)Math.Pow(2, n) - 1;
            var result = new double[f];
            int t = 0;
            for (int i = 1; i <= n; i++)
            {
                foreach (int[] combos in Factorial.FindCombinations(i, n))
                {
                    var indicators = new int[n];
                    for (int j = 0; j < combos.Length; j++)
                    {
                        indicators[combos[j]] = 1;
                    }
                    result[t] = PositivelyDependentExclusive(probabilities, indicators);
                    t++;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a list of exclusive probabilities of multiple events occurring assuming independence.
        /// </summary>
        /// <param name="probabilities">An array of probabilities for each event.</param>
        /// <param name="binomialCombinations">An array of binomial combinations.</param>
        /// <param name="indicators">An 2D array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        /// <param name="eventProbabilities">Output. A list of exclusive event probabilities.</param>
        /// <param name="eventIndicators">Output. A list of exclusive event indicators that were evaluated.</param>
        /// <param name="absoluteTol">The absolute tolerance for evaluation convergence of the inclusion-exclusion algorithm. Default = 1E-8.</param>
        /// <param name="relativeTol">The realtive tolerance for evaluation convergence of the inclusion-exclusion algorithm. Default = 1E-4.</param>
        public static void PositivelyDependentExclusive(IList<double> probabilities, int[] binomialCombinations, int[,] indicators, out List<double> eventProbabilities, out List<int[]> eventIndicators, double absoluteTol = 1E-8, double relativeTol = 1E-4)
        {
            eventProbabilities = new List<double>();
            eventIndicators = new List<int[]>();

            double union = 0;
            double s = 1;
            int j = 0;
            int c = binomialCombinations[j];
            double inc = double.NaN;
            double exc = double.NaN;

            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                if (i == c)
                {
                    if (j > 0 && s == 1)
                    {
                        inc = union;
                    }
                    else if (j > 0 && s == -1)
                    {
                        exc = union;
                    }

                    // Check for convergence
                    // Add the tolerance to the last joint event
                    double diff = Math.Abs(inc - exc);
                    double tol = absoluteTol + relativeTol * Math.Min(inc, exc);
                    if (j > 0 && j < binomialCombinations.Length && diff <= tol)
                    {
                        eventIndicators.Add(indicators.GetRow(indicators.GetLength(0) - 1));
                        eventProbabilities.Add(0.5 * diff);
                        return;
                    }

                    s *= -1;
                    j++;
                    if (j < binomialCombinations.Length)
                    {
                        c += binomialCombinations[j];
                    }

                }

                // Record indicators
                eventIndicators.Add(indicators.GetRow(i));
                // Compute the exclusive event probability
                eventProbabilities.Add(PositivelyDependentExclusive(probabilities, eventIndicators.Last()));
                // Compute the union
                if (i < probabilities.Count)
                {
                    union += s * probabilities[i];
                }
                else
                {
                    union += s * PositiveJointProbability(probabilities, eventIndicators.Last());
                }

            }

        }

        #endregion

        #region Any Dependency

        /// <summary>
        /// Returns an array of exclusive probabilities of multiple events using the inclusion-exclusion method. Dependence between events is captured with the multivariate normal distribution.
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution used to compute the joint probabilities.</param>
        public static double[] ExclusivePCM(IList<double> probabilities, MultivariateNormal multivariateNormal)
        {
            // Get number of unique combinations by subset
            int N = probabilities.Count;
            var binomialCombinations = new int[N];
            var cumCombos = new int[N - 1];
            binomialCombinations[0] = (int)Factorial.BinomialCoefficient(N, 1);
            cumCombos[0] = binomialCombinations[0];
            for (int i = 1; i < N - 1; i++)
            {
                binomialCombinations[i] = (int)Factorial.BinomialCoefficient(N, i + 1);
                cumCombos[i] = cumCombos[i - 1] + binomialCombinations[i];
            }

            // Get combination indicators
            var indicators = Factorial.AllCombinations(N);

            // Get joint probabilities
            var pVals = JointProbabilitiesPCM(probabilities, indicators, multivariateNormal);

            var result = new double[indicators.GetLength(0)];

            int j = 0;
            int c = binomialCombinations[j];

            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                if (i == c)
                {
                    j++;
                    if (j < binomialCombinations.Length) c += binomialCombinations[j];
                }

                result[i] = pVals[i];
                double s = 1;
                for (int k = j; k < cumCombos.Length; k++)
                {
                    s *= -1;
                    int c1 = cumCombos[k];
                    int c2 = k == cumCombos.Length - 1 ? cumCombos[k] + 1 : cumCombos[k + 1];
                    var sP = SumSearch(pVals, indicators.GetRow(i), indicators, c1, c2);
                    result[i] += s * sP;
                }
                // Due to floating point issues, this can sometimes be very small, but negative. 
                if (result[i] < 0d) result[i] = 0d;
            }

            return result;

        }

        /// <summary>
        /// Returns an array of exclusive probabilities of multiple events using the inclusion-exclusion method. Dependence between events is captured with the multivariate normal distribution.
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        /// <param name="binomialCombinations">An array of binomial combinations.</param>
        /// <param name="indicators">An 2D array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution used to compute the joint probabilities.</param>
        public static double[] ExclusivePCM(IList<double> probabilities, int[] binomialCombinations, int[,] indicators, MultivariateNormal multivariateNormal)
        {

            // Get number of unique combinations by subset
            int N = probabilities.Count;
            var cumCombos = new int[N - 1];
            cumCombos[0] = binomialCombinations[0];
            for (int i = 1; i < N - 1; i++)
            {
                cumCombos[i] = cumCombos[i - 1] + binomialCombinations[i];
            }

            // Get joint probabilities
            var pVals = JointProbabilitiesPCM(probabilities, indicators, multivariateNormal);

            var result = new double[indicators.GetLength(0)];

            int j = 0;
            int c = binomialCombinations[j];

            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                if (i == c)
                {
                    j++;
                    if (j < binomialCombinations.Length) c += binomialCombinations[j];
                }

                result[i] = pVals[i];
                double s = 1;
                for (int k = j; k < cumCombos.Length; k++)
                {
                    s *= -1;
                    int c1 = cumCombos[k];
                    int c2 = k == cumCombos.Length - 1 ? cumCombos[k] + 1 : cumCombos[k + 1];
                    var sP = SumSearch(pVals, indicators.GetRow(i), indicators, c1, c2);
                    result[i] += s * sP;
                }
                // Due to floating point issues, this can sometimes be very small, but negative. 
                if (result[i] < 0d) result[i] = 0d;
            }

            return result;
        }

        /// <summary>
        /// Returns an array of exclusive probabilities of multiple events using the inclusion-exclusion method. Dependence between events is captured with the multivariate normal distribution.
        /// </summary>
        /// <param name="probabilities">An array of probabilities for each event.</param>
        /// <param name="binomialCombinations">An array of binomial combinations.</param>
        /// <param name="indicators">An 2D array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution used to compute the joint probabilities.</param>
        /// <param name="eventProbabilities">Output. A list of exclusive event probabilities.</param>
        /// <param name="eventIndicators">Output. A list of exclusive event indicators that were evaluated.</param>
        /// <param name="absoluteTol">The absolute tolerance for evaluation convergence of the inclusion-exclusion algorithm. Default = 1E-8.</param>
        /// <param name="relativeTol">The realtive tolerance for evaluation convergence of the inclusion-exclusion algorithm. Default = 1E-4.</param>
        public static void ExclusivePCM(IList<double> probabilities, int[] binomialCombinations, int[,] indicators, MultivariateNormal multivariateNormal, out List<double> eventProbabilities, out List<int[]> eventIndicators, double absoluteTol = 1E-8, double relativeTol = 1E-4)
        {
            // Get number of unique combinations by subset
            int N = probabilities.Count;
            var cumCombos = new int[N - 1];
            cumCombos[0] = binomialCombinations[0];
            for (int i = 1; i < N - 1; i++)
            {
                cumCombos[i] = cumCombos[i - 1] + binomialCombinations[i];
            }

            var jointProbabilities = new List<double>();
            eventProbabilities = new List<double>();
            eventIndicators = new List<int[]>();

            double union = 0;
            double s = 1;
            int j = 0;
            int c = binomialCombinations[j];
            double inc = double.NaN;
            double exc = double.NaN;

            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                if (i == c)
                {
                    if (j > 0 && s == 1)
                    {
                        inc = union;
                    }
                    else if (j > 0 && s == -1)
                    {
                        exc = union;
                    }

                    // Check for convergence
                    // Add the tolerance to the last joint event
                    double diff = Math.Abs(inc - exc);
                    double tol = absoluteTol + relativeTol * Math.Min(inc, exc);
                    if (j > 0 && j < binomialCombinations.Length && diff <= tol)
                    {
                        eventIndicators.Add(indicators.GetRow(indicators.GetLength(0) - 1));
                        jointProbabilities.Add(0.5 * diff);
                        goto Exclusive;
                    }

                    s *= -1;
                    j++;
                    if (j < binomialCombinations.Length)
                    {
                        c += binomialCombinations[j];
                    }

                }

                // Record indicators
                eventIndicators.Add(indicators.GetRow(i));

                // Compute the union
                if (i < probabilities.Count)
                {
                    jointProbabilities.Add(probabilities[i]);
                    union += s * jointProbabilities.Last();
                }
                else
                {
                    jointProbabilities.Add(JointProbabilityPCM(probabilities, eventIndicators.Last(), multivariateNormal));
                    union += s * jointProbabilities.Last();
                }

            }

            Exclusive:

            j = 0;
            c = binomialCombinations[j];

            for (int i = 0; i < eventIndicators.Count; i++)
            {
                if (i == c)
                {
                    j++;
                    if (j < binomialCombinations.Length) c += binomialCombinations[j];
                }

                double prob = jointProbabilities[i];
                s = 1;
                for (int k = j; k < cumCombos.Length; k++)
                {
                    s *= -1;
                    int c1 = cumCombos[k];
                    int c2 = k == cumCombos.Length - 1 ? cumCombos[k] + 1 : cumCombos[k + 1];
                    if (c2 >= eventIndicators.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        var sP = SumSearch(jointProbabilities, eventIndicators[i], eventIndicators, c1, c2);
                        prob += s * sP;
                    }

                }
                // Due to floating point issues, this can sometimes be very small, but negative. 
                if (prob < 0d) prob = 0d;
                eventProbabilities.Add(prob);
            }
        }

        /// <summary>
        /// Returns an array of exclusive probabilities of multiple events using the inclusion-exclusion method. Dependence between events is captured with the multivariate normal distribution.
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution used to compute the joint probabilities.</param>
        public static double[] Exclusive(IList<double> probabilities, MultivariateNormal multivariateNormal)
        {
            // Get number of unique combinations by subset
            int N = probabilities.Count;
            var binomialCombinations = new int[N];
            var cumCombos = new int[N - 1];
            binomialCombinations[0] = (int)Factorial.BinomialCoefficient(N, 1);
            cumCombos[0] = binomialCombinations[0];
            for (int i = 1; i < N - 1; i++)
            {
                binomialCombinations[i] = (int)Factorial.BinomialCoefficient(N, i + 1);
                cumCombos[i] = cumCombos[i - 1] + binomialCombinations[i];
            }

            // Get combination indicators
            var indicators = Factorial.AllCombinations(N);

            // Get joint probabilities
            var pVals = JointProbabilities(probabilities, indicators, multivariateNormal);

            var result = new double[indicators.GetLength(0)];

            int j = 0;
            int c = binomialCombinations[j];

            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                if (i == c)
                {
                    j++;
                    if (j < binomialCombinations.Length) c += binomialCombinations[j];
                }

                result[i] = pVals[i];
                double s = 1;
                for (int k = j; k < cumCombos.Length; k++)
                {
                    s *= -1;
                    int c1 = cumCombos[k];
                    int c2 = k == cumCombos.Length - 1 ? cumCombos[k] + 1 : cumCombos[k + 1];
                    var sP = SumSearch(pVals, indicators.GetRow(i), indicators, c1, c2);
                    result[i] += s * sP;
                }
                // Due to floating point issues, this can sometimes be very small, but negative. 
                if (result[i] < 0d) result[i] = 0d;
            }

            return result;

        }

        /// <summary>
        /// Returns an array of exclusive probabilities of multiple events using the inclusion-exclusion method. Dependence between events is captured with the multivariate normal distribution.
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        /// <param name="binomialCombinations">An array of binomial combinations.</param>
        /// <param name="indicators">An 2D array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution used to compute the joint probabilities.</param>
        public static double[] Exclusive(IList<double> probabilities, int[] binomialCombinations, int[,] indicators, MultivariateNormal multivariateNormal)
        {

            // Get number of unique combinations by subset
            int N = probabilities.Count;
            var cumCombos = new int[N - 1];
            cumCombos[0] = binomialCombinations[0];
            for (int i = 1; i < N - 1; i++)
            {
                cumCombos[i] = cumCombos[i - 1] + binomialCombinations[i];
            }

            // Get joint probabilities
            var pVals = JointProbabilities(probabilities, indicators, multivariateNormal);

            var result = new double[indicators.GetLength(0)];

            int j = 0;
            int c = binomialCombinations[j];

            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                if (i == c)
                {
                    j++;
                    if (j < binomialCombinations.Length) c += binomialCombinations[j];
                }

                result[i] = pVals[i];
                double s = 1;
                for (int k = j; k < cumCombos.Length; k++)
                {
                    s *= -1;
                    int c1 = cumCombos[k];
                    int c2 = k == cumCombos.Length - 1 ? cumCombos[k] + 1 : cumCombos[k + 1];
                    var sP = SumSearch(pVals, indicators.GetRow(i), indicators, c1, c2);
                    result[i] += s * sP;
                }
                // Due to floating point issues, this can sometimes be very small, but negative. 
                if (result[i] < 0d) result[i] = 0d;
            }

            return result;
        }


        /// <summary>
        /// Returns an array of exclusive probabilities of multiple events using the inclusion-exclusion method. Dependence between events is captured with the multivariate normal distribution.
        /// </summary>
        /// <param name="probabilities">An array of probabilities for each event.</param>
        /// <param name="binomialCombinations">An array of binomial combinations.</param>
        /// <param name="indicators">An 2D array of indicators, 0 means the event did not occur, 1 means the event did occur.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution used to compute the joint probabilities.</param>
        /// <param name="eventProbabilities">Output. A list of exclusive event probabilities.</param>
        /// <param name="eventIndicators">Output. A list of exclusive event indicators that were evaluated.</param>
        /// <param name="absoluteTol">The absolute tolerance for evaluation convergence of the inclusion-exclusion algorithm. Default = 1E-8.</param>
        /// <param name="relativeTol">The realtive tolerance for evaluation convergence of the inclusion-exclusion algorithm. Default = 1E-4.</param>
        public static void Exclusive(IList<double> probabilities, int[] binomialCombinations, int[,] indicators, MultivariateNormal multivariateNormal, out List<double> eventProbabilities, out List<int[]> eventIndicators, double absoluteTol = 1E-8, double relativeTol = 1E-4)
        {


            // Get number of unique combinations by subset
            int N = probabilities.Count;
            var cumCombos = new int[N - 1];
            cumCombos[0] = binomialCombinations[0];
            for (int i = 1; i < N - 1; i++)
            {
                cumCombos[i] = cumCombos[i - 1] + binomialCombinations[i];
            }

            var jointProbabilities = new List<double>();
            eventProbabilities = new List<double>();
            eventIndicators = new List<int[]>();

            double union = 0;
            double s = 1;
            int j = 0;
            int c = binomialCombinations[j];
            double inc = double.NaN;
            double exc = double.NaN;

            for (int i = 0; i < indicators.GetLength(0); i++)
            {
                if (i == c)
                {
                    if (j > 0 && s == 1)
                    {
                        inc = union;
                    }
                    else if (j > 0 && s == -1)
                    {
                        exc = union;
                    }

                    // Check for convergence
                    // Add the tolerance to the last joint event
                    double diff = Math.Abs(inc - exc);
                    double tol = absoluteTol + relativeTol * Math.Min(inc, exc);
                    if (j > 0 && j < binomialCombinations.Length && diff <= tol)
                    {
                        eventIndicators.Add(indicators.GetRow(indicators.GetLength(0) - 1));
                        jointProbabilities.Add(0.5 * diff);
                        goto Exclusive;
                    }

                    s *= -1;
                    j++;
                    if (j < binomialCombinations.Length)
                    {
                        c += binomialCombinations[j];
                    }

                }

                // Record indicators
                eventIndicators.Add(indicators.GetRow(i));

                // Compute the union
                if (i < probabilities.Count)
                {
                    jointProbabilities.Add(probabilities[i]);
                    union += s * jointProbabilities.Last();
                }
                else
                {
                    jointProbabilities.Add(JointProbability(probabilities, eventIndicators.Last(), multivariateNormal));
                    union += s * jointProbabilities.Last();
                }

            }

        Exclusive:

            j = 0;
            c = binomialCombinations[j];

            for (int i = 0; i < eventIndicators.Count; i++)
            {
                if (i == c)
                {
                    j++;
                    if (j < binomialCombinations.Length) c += binomialCombinations[j];
                }

                double prob = jointProbabilities[i];
                s = 1;
                for (int k = j; k < cumCombos.Length; k++)
                {
                    s *= -1;
                    int c1 = cumCombos[k];
                    int c2 = k == cumCombos.Length - 1 ? cumCombos[k] + 1 : cumCombos[k + 1];
                    if (c2 >= eventIndicators.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        var sP = SumSearch(jointProbabilities, eventIndicators[i], eventIndicators, c1, c2);
                        prob += s * sP;
                    }

                }
                // Due to floating point issues, this can sometimes be very small, but negative. 
                if (prob < 0d) prob = 0d;
                eventProbabilities.Add(prob);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="probabilityValues"></param>
        /// <param name="indicatorValues"></param>
        /// <param name="indicators"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private static double SumSearch(double[] probabilityValues, int[] indicatorValues, int[,] indicators, int startIndex, int endIndex)
        {
            double result = 0;
            var indeces = new List<int>();
            for (int i = 0; i < indicatorValues.Length; i++)
            {
                if (indicatorValues[i] == 1)
                {
                    indeces.Add(i);
                }
            }
            for (int i = startIndex; i < endIndex; i++)
            {
                bool inclusive = true;
                for (int j = 0; j < indeces.Count; j++)
                {
                    if (indicators[i, indeces[j]] == 0)
                    {
                        inclusive = false;
                        break;
                    }
                }
                if (inclusive == true) result += probabilityValues[i];
            }
            return result;
        }

        private static double SumSearch(List<double> probabilityValues, int[] indicatorValues, List<int[]> indicators, int startIndex, int endIndex)
        {
            double result = 0;
            var indeces = new List<int>();
            for (int i = 0; i < indicatorValues.Length; i++)
            {
                if (indicatorValues[i] == 1)
                {
                    indeces.Add(i);
                }
            }
            for (int i = startIndex; i < endIndex; i++)
            {
                bool inclusive = true;
                for (int j = 0; j < indeces.Count; j++)
                {
                    if (indicators[i][indeces[j]] == 0)
                    {
                        inclusive = false;
                        break;
                    }
                }
                if (inclusive == true) result += probabilityValues[i];
            }
            return result;
        }

        #endregion

        #region Common Cause Adjustment

        /// <summary>
        /// Returns the common cause adjustment factor.
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        public static double CommonCauseAdjustment(IList<double> probabilities)
        {
            if (probabilities.Count == 1) return 1d;
            double numerator = 1d;
            double denominator = 0d;
            for (int i = 0; i < probabilities.Count; i++)
            {
                numerator *= 1d - probabilities[i];
                denominator += probabilities[i];
            }
            if (denominator == 0) return 1d;
            return (1d - numerator) / denominator;
        }

        /// <summary>
        /// Returns the common cause adjustment factor using inclusion-exclusion. 
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        /// <param name="multivariateNormal">The multivariate normal distribution used to compute the joint probabilities.</param>
        public static double CommonCauseAdjustment(IList<double> probabilities, MultivariateNormal multivariateNormal)
        {
            if (probabilities.Count <= 1) return 1d;
            double numerator = UnionPCM(probabilities, multivariateNormal);
            double denominator = Tools.Sum(probabilities);
            if (denominator == 0) return 1d;
            return numerator / denominator;
        }

        /// <summary>
        /// Returns the mutually exclusive adjustment factor.
        /// </summary>
        /// <param name="probabilities">List of probabilities.</param>
        public static double MutuallyExclusiveAdjustment(IList<double> probabilities)
        {
            if (probabilities.Count == 1) return 1d;
            double numerator = 1d;
            double denominator = 0d;
            for (int i = 0; i < probabilities.Count; i++)
                denominator += probabilities[i];
            if (denominator <= 1) return 1d;
            return numerator / denominator;
        }

        #endregion

        #endregion

    }
}
