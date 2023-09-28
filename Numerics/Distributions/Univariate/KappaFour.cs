using System;
using System.Collections.Generic;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Mathematics.LinearAlgebra;
using Numerics.Mathematics.Optimization;
using Numerics.Mathematics.RootFinding;
using Numerics.Mathematics.SpecialFunctions;

namespace Numerics.Distributions
{

    /// <summary>
    /// The Kappa-4 distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://ieeexplore.ieee.org/document/5389569" />
    /// <see href = "https://rdrr.io/cran/nsRFA/src/R/KAPPA.R" />
    /// </para>
    /// <para>
    /// <list type="bullet">
    /// <item><description>
    /// If h = -1 , then the Kappa-4 is the Generalized Logistic distribution.
    /// </description></item>
    /// <item><description>
    /// If h = 0 , then the Kappa-4 is the Generalized Extreme Value distribution.
    /// </description></item>
    /// <item><description>
    /// If h = 1 , then the Kappa-4 is the Generalized Pareto distribution.
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public class KappaFour : UnivariateDistributionBase, IColesTawn, IEstimation, IMaximumLikelihoodEstimation, ILinearMomentEstimation, IBootstrappable
    {

        /// <summary>
        /// Constructs a Kappa-4 distribution.
        /// </summary>
        public KappaFour()
        {
            SetParameters(new[] { 100d, 10d, 0d, 0d });
        }

        /// <summary>
        /// Constructs a Kappa-4 distribution with the given parameters ξ, α, κ, and h.
        /// </summary>
        /// <param name="location">The location parameter ξ (Xi).</param>
        /// <param name="scale">The scale parameter α (alpha).</param>
        /// <param name="shape">The shape parameter κ (kappa).</param>
        /// <param name="shape2">The shape parameter h (hondo).</param>
        public KappaFour(double location, double scale, double shape, double shape2)
        {
            SetParameters(new[] { location, scale, shape, shape2 });
        }
    
        private bool _parametersValid = true;
        private double _alpha;
        private bool _momentsComputed = false;
        private double[] u = new double[] { double.NaN, double.NaN, double.NaN, double.NaN };

        /// <summary>
        /// Gets and sets the location parameter ξ (Xi).
        /// </summary>
        public double Xi { get; set; }

        /// <summary>
        /// Gets and sets the scale parameter α (alpha).
        /// </summary>
        public double Alpha
        {
            get { return _alpha; }
            set
            {
                _parametersValid = ValidateParameters(new[] { Xi, value, Kappa, Hondo }, false) is null;
                _alpha = value;
            }
        }

        /// <summary>
        /// Gets and sets the shape parameter κ (kappa).
        /// </summary>
        public double Kappa { get; set; }

        /// <summary>
        /// Gets and sets the shape parameter h (hondo).
        /// </summary>
        public double Hondo { get; set; }

        /// <summary>
        /// Returns the number of distribution parameters.
        /// </summary>
        public override int NumberOfParameters
        {
            get { return 4; }
        }

        /// <summary>
        /// Returns the univariate distribution type.
        /// </summary>
        public override UnivariateDistributionType Type
        {
            get { return UnivariateDistributionType.KappaFour; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Kappa-4"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "K4"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[4, 2];
                parmString[0, 0] = "Location (ξ)";
                parmString[1, 0] = "Scale (α)";
                parmString[2, 0] = "Shape (κ)";
                parmString[3, 0] = "Shape (h)";
                parmString[0, 1] = Xi.ToString();
                parmString[1, 1] = Alpha.ToString();
                parmString[2, 1] = Kappa.ToString();
                parmString[3, 1] = Hondo.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get { return new[] { "ξ", "α", "κ", "h" }; }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Xi), nameof(Alpha), nameof(Kappa), nameof(Hondo) }; }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters
        {
            get { return new[] { Xi, Alpha, Kappa, Hondo }; }
        }

        /// <summary>
        /// Determines whether the parameters are valid or not.
        /// </summary>
        public override bool ParametersValid
        {
            get
            { return _parametersValid; }
        }

        /// <summary>
        /// Gets the mean of the distribution.
        /// </summary>
        public override double Mean
        {
            get
            {
                if (!_momentsComputed)
                {
                    u = CentralMoments(1000);
                    _momentsComputed = true;
                }
                return u[0];
            }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public override double Median
        {
            get { return InverseCDF(0.5d); }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get { return double.NaN; }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get
            {
                if (!_momentsComputed)
                {
                    u = CentralMoments(1000);
                    _momentsComputed = true;
                }
                return u[1];
            }
        }

        /// <summary>
        /// Gets the skew of the distribution.
        /// </summary>
        public override double Skew
        {
            get
            {
                if (!_momentsComputed)
                {
                    u = CentralMoments(1000);
                    _momentsComputed = true;
                }
                return u[2];
            }
        }

        /// <summary>
        /// Gets the kurtosis of the distribution.
        /// </summary>
        public override double Kurtosis
        {
            get
            {
                if (!_momentsComputed)
                {
                    u = CentralMoments(1000);
                    _momentsComputed = true;
                }
                return u[3];
            }
        }

        /// <summary>
        /// Gets the minimum of the distribution.
        /// </summary>
        public override double Minimum
        {
            get
            {
                if (Hondo > 0d)
                {
                    return Xi + Alpha / Kappa * (1d - Math.Pow(Hondo, -Kappa));
                }
                if (Hondo <= 0d && Kappa < -NearZero)
                {
                    return Xi + Alpha / Kappa;
                }
                if (Hondo <= 0d && Kappa >= -NearZero)
                {
                    return double.NegativeInfinity;
                }
                return double.NaN;
            }
        }

        /// <summary>
        /// Gets the maximum of the distribution.
        /// </summary>
        public override double Maximum
        {
            get
            {
                if (Kappa <= NearZero)
                {
                    return double.PositiveInfinity;
                }
                else
                {
                    return Xi + Alpha / Kappa;
                }
            }
        }

        /// <summary>
        /// Gets the minimum values allowable for each parameter.
        /// </summary>
        public override double[] MinimumOfParameters
        {
            get { return new[] { double.NegativeInfinity, 0.0d, double.NegativeInfinity, double.NegativeInfinity }; }
        }

        /// <summary>
        /// Gets the maximum values allowable for each parameter.
        /// </summary>
        public override double[] MaximumOfParameters
        {
            get { return new[] { double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity }; }
        }

        /// <summary>
        /// Estimates the parameters of the underlying distribution given a sample of observations.
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        /// <param name="estimationMethod">The parameter estimation method.</param>
        public void Estimate(IList<double> sample, ParameterEstimationMethod estimationMethod)
        {
            if (estimationMethod == ParameterEstimationMethod.MethodOfMoments)
            {
                throw new NotImplementedException();
            }
            else if (estimationMethod == ParameterEstimationMethod.MethodOfLinearMoments)
            {
                SetParameters(ParametersFromLinearMoments(Statistics.LinearMoments(sample)));
            }
            else if (estimationMethod == ParameterEstimationMethod.MaximumLikelihood)
            {
                SetParameters(MLE(sample));
            }
        }

        /// <summary>
        /// Bootstrap the distribution based on a sample size and parameter estimation method.
        /// </summary>
        /// <param name="estimationMethod">The parameter estimation method.</param>
        /// <param name="sampleSize">Size of the random sample to generate.</param>
        /// <param name="seed">Optional. Seed for random number generator. Default = 12345.</param>
        /// <returns>
        /// Returns a bootstrapped distribution.
        /// </returns>
        public IUnivariateDistribution Bootstrap(ParameterEstimationMethod estimationMethod, int sampleSize, int seed = 12345)
        {
            var newDistribution = new KappaFour(Xi, Alpha, Kappa, Hondo);
            var sample = newDistribution.GenerateRandomValues(seed, sampleSize);
            newDistribution.Estimate(sample, estimationMethod);
            return newDistribution;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        public override void SetParameters(IList<double> parameters)
        {
            // Set parameters
            Xi = parameters[0];
            Alpha = parameters[1];
            Kappa = parameters[2];
            Hondo = parameters[3];
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="parameters">A list of moments of the log transformed data.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public override ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            if (parameters[1] <= 0.0d)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Alpha), "The scale parameter α (alpha) must be positive.");
                return new ArgumentOutOfRangeException(nameof(Alpha), "The scale parameter α (alpha) must be positive.");
            }
            return null;
        }

        /// <summary>
        /// Returns an array of distribution parameters given the linear moments of the sample.
        /// </summary>
        /// <param name="moments">The array of sample linear moments.</param>
        public double[] ParametersFromLinearMoments(IList<double> moments)
        {
            // This routine is taken and converted directly from Fortran code
            //***********************************************************************
            //*                                                                     *
            //* FORTRAN CODE WRITTEN FOR INCLUSION IN IBM RESEARCH REPORT RC20525,  *
            //* 'FORTRAN ROUTINES FOR USE WITH THE METHOD OF L-MOMENTS, VERSION 3'  *
            //*                                                                     *
            //* J.R.M.HOSKING                                                       *
            //* IBM RESEARCH DIVISION                                               *
            //* T.J.WATSON RESEARCH CENTER                                          *
            //* YORKTOWN HEIGHTS                                                    *
            //* NEW YORK 10598, U.S.A.                                              *
            //*                                                                     *
            //* VERSION 3     AUGUST 1996                                           *
            //*                                                                     *
            //***********************************************************************

            double L1 = moments[0];
            double L2 = moments[1];
            double T3 = moments[2];
            double T4 = moments[3];
            double eps = 1E-6;
            int maxit = 20, maxsr = 10;
            double xi = 0, alpha = 0, kappa = 0, hondo = 0;

            //  TEST FOR FEASIBILITY

            if (L2 <= 0d) throw new ArgumentException("L-moments invalid.");
            if (Math.Abs(T3) >= 1d || Math.Abs(T4) >= 1d) throw new ArgumentException("L-moments invalid.");
            if (T4 <= (5d * T3 * T3 - 1d) / 4d) throw new ArgumentException("L-moments invalid.");
            if (T4 >= (5d * T3 * T3 + 1d) / 6d) throw new ArgumentException("(TAU-3, TAU-4) lies above the Generalized Logistic (suggests that L-moments are not constant with any Kappa distribution with hondo > -1).");

            //  SET STARTING VALUES FOR N-R ITERATION:
            //  G IS CHOSEN TO GIVE THE CORRECT VALUE OF TAU - 3 ON THE
            //  ASSUMPTION THAT H = 1(I.E.A GENERALIZED PARETO FIT) -
            //  BUT H IS ACTUALLY SET TO 1.001 TO AVOID NUMERICAL
            //  DIFFICULTIES WHICH CAN SOMETIMES ARISE WHEN H = 1 EXACTLY

            double G = (1d - 3d * T3) / (1d + T3);
            double H = 1.001d;
            double Z = G + H * 0.725d;
            double XDIST = 10d, DIST = 0;
            double U1 = 0, U2 = 0, U3 = 0, U4 = 0;
            double ALAM2 = 0, ALAM3, ALAM4;
            double TAU3 = 0, TAU4 = 0;
            double E1 = 0, E2 = 0;
            double DEL1 = 0, DEL2 = 0;
            double XG = 0, XH = 0, XZ, RHH;
            double U1G, U2G, U3G, U4G, U1H, U2H, U3H, U4H;
            double DL2G, DL2H, DL3G, DL3H, DL4G, DL4H;
            double D11, D12, D21, D22, DET, H11, H12, H21, H22;
            double FACTOR;

            //  START OF NEWTON-RAPHSON ITERATION

            for (int i = 1; i < maxit; i++)
            {

                //  REDUCE STEPLENGTH UNTIL WE ARE NEARER TO THE REQUIRED
                //  VALUES OF TAU-3 AND TAU-4 THAN WE WERE AT THE PREVIOUS STEP

                for (int j = 1; j <= maxsr; j++)
                {
                    if (G > 53d) throw new Exception("Iteration encountered numerical difficulties - overflow would have been likely to occur.");

                    if (H <= 0)
                    {
                        U1 = Math.Exp(Gamma.LogGamma(-1d / H - G) - Gamma.LogGamma(-1d / H + 1d));
                        U2 = Math.Exp(Gamma.LogGamma(-2d / H - G) - Gamma.LogGamma(-2d / H + 1d));
                        U3 = Math.Exp(Gamma.LogGamma(-3d / H - G) - Gamma.LogGamma(-3d / H + 1d));
                        U4 = Math.Exp(Gamma.LogGamma(-4d / H - G) - Gamma.LogGamma(-4d / H + 1d));
                    }
                    else
                    {
                        U1 = Math.Exp(Gamma.LogGamma(1d / H) - Gamma.LogGamma(1d / H + 1d + G));
                        U2 = Math.Exp(Gamma.LogGamma(2d / H) - Gamma.LogGamma(2d / H + 1d + G));
                        U3 = Math.Exp(Gamma.LogGamma(3d / H) - Gamma.LogGamma(3d / H + 1d + G));
                        U4 = Math.Exp(Gamma.LogGamma(4d / H) - Gamma.LogGamma(4d / H + 1d + G));
                    }
                    ALAM2 = U1 - 2d * U2;
                    ALAM3 = -U1 + 6d * U2 - 6d * U3;
                    ALAM4 = U1 - 12d * U2 + 30d * U3 - 20d * U4;               
                    if (ALAM2 == 0d) throw new Exception("Iteration encountered numerical difficulties - overflow would have been likely to occur.");
                    TAU3 = ALAM3 / ALAM2;
                    TAU4 = ALAM4 / ALAM2;
                    E1 = TAU3 - T3;
                    E2 = TAU4 - T4;

                    // IF NEARER THAN BEFORE, EXIT THIS LOOP
                    DIST = Math.Max(Math.Abs(E1), Math.Abs(E2));
                    if (DIST < XDIST) break;

                    // OTHERWISE, HALVE THE STEPLENGTH AND TRY AGAIN
                    DEL1 *= 0.5;
                    DEL2 *= 0.5;
                    G = XG - DEL1;
                    H = XH - DEL2;

                    // TOO MANY STEPLENGTH REDUCTIONS
                    if (j == maxsr) throw new Exception("Iteration encountered numerical difficulties - overflow would have been likely to occur.");
                }

                //  TEST FOR CONVERGENCE
                if (DIST < eps) break;

                //  NOT CONVERGED: CALCULATE NEXT STEP
                //  NOTATION:
                //  U1G  - DERIVATIVE OF U1 W.R.T.G
                //  DL2G - DERIVATIVE OF ALAM2 W.R.T.G
                //  D..  - MATRIX OF DERIVATIVES OF TAU-3 AND TAU-4 W.R.T.G AND H
                //  H..  - INVERSE OF DERIVATIVE MATRIX
                //  DEL. - STEPLENGTH

                XG = G;
                XH = H;
                XZ = Z;
                XDIST = DIST;
                RHH = 1d / (H * H);

                if (H > 0)
                {
                    U1G = -U1 * Gamma.Digamma(1d / H + 1d + G);
                    U2G = -U2 * Gamma.Digamma(2d / H + 1d + G);
                    U3G = -U3 * Gamma.Digamma(3d / H + 1d + G);
                    U4G = -U4 * Gamma.Digamma(4d / H + 1d + G);
                    U1H = RHH * (-U1G - U1 * Gamma.Digamma(1d / H));
                    U2H = 2d * RHH * (-U2G - U2 * Gamma.Digamma(2d / H));
                    U3H = 3d * RHH * (-U3G - U3 * Gamma.Digamma(3d / H));
                    U4H = 4d * RHH * (-U4G - U4 * Gamma.Digamma(4d / H));
                }
                else
                {
                    U1G = -U1 * Gamma.Digamma(-1d / H - G);
                    U2G = -U2 * Gamma.Digamma(-2d / H - G);
                    U3G = -U3 * Gamma.Digamma(-3d / H - G);
                    U4G = -U4 * Gamma.Digamma(-4d/ H - G);
                    U1H = RHH * (-U1G - U1 * Gamma.Digamma(-1d / H + 1d));
                    U2H = 2d * RHH * (-U2G - U2 * Gamma.Digamma(-2d / H + 1d));
                    U3H = 3d * RHH * (-U3G - U3 * Gamma.Digamma(-3d / H + 1d));
                    U4H = 4d * RHH * (-U4G - U4 * Gamma.Digamma(-4d / H + 1d));
                }

                DL2G = U1G - 2d * U2G;
                DL2H = U1H - 2d * U2H;
                DL3G = -U1G + 6d * U2G - 6d * U3G;
                DL3H = -U1H + 6d * U2H - 6d * U3H;
                DL4G = U1G - 12d * U2G + 30d * U3G - 20d * U4G;
                DL4H = U1H - 12d * U2H + 30d * U3H - 20d * U4H;
                D11 = (DL3G - TAU3 * DL2G) / ALAM2;
                D12 = (DL3H - TAU3 * DL2H) / ALAM2;
                D21 = (DL4G - TAU4 * DL2G) / ALAM2;
                D22 = (DL4H - TAU4 * DL2H) / ALAM2;
                DET = D11 * D22 - D12 * D21;
                H11 = D22 / DET;
                H12 = -D12 / DET;
                H21 = -D21 / DET;
                H22 = D11 / DET;
                DEL1 = E1 * H11 + E2 * H12;
                DEL2 = E1 * H21 + E2 * H22;

                //  TAKE NEXT N-R STEP

                G = XG - DEL1;
                H = XH - DEL2;
                Z = G + H * 0.725;

                //  REDUCE STEP IF G AND H ARE OUTSIDE THE PARAMETER SPACE
                FACTOR = 1d;
                if (G <= -1d) FACTOR = 0.8 * (XG + 1d) / DEL1;
                if (H <= -1d) FACTOR = Math.Min(FACTOR, 0.8 * (XH + 1d) / DEL2);
                if (Z <= -1d) FACTOR = Math.Min(FACTOR, 0.8 * (XZ + 1d) / (XZ - Z));
                if (H <= 0 && G * H <= -1d) FACTOR = Math.Min(FACTOR, 0.8 * (XG * XH + 1d) / (XG * XH - G * H));
                if (FACTOR != 1d)
                {
                    DEL1 = DEL1 * FACTOR;
                    DEL2 = DEL2 * FACTOR;
                    G = XG - DEL1;
                    H = XH - DEL2;
                    Z = G + H * 0.725;
                }

                // NOT CONVERGED
                if (i == maxit) throw new Exception("Iterations failed to converge.");
            }

            hondo = H;
            kappa = G;
            var TEMP = Gamma.LogGamma(1d + G);
            if (TEMP > 170d) throw new Exception("Iteration for hondo and kappa converged, but overflow would have occurred when calculating xi and alpha.");
            var GAM = Math.Exp(TEMP);
            TEMP = (1d + G) * Math.Log(Math.Abs(H));
            if (TEMP > 170d) throw new Exception("Iteration for hondo and kappa converged, but overflow would have occurred when calculating xi and alpha.");
            var HH = Math.Exp(TEMP);
            alpha = L2 * G * HH / (ALAM2 * GAM);
            xi = L1 - alpha / G * (1d - GAM * U1 / HH);

            return new[] { xi, alpha, kappa, hondo };

        }

        /// <summary>
        /// Returns an array of linear moments given the distribution parameters.
        /// </summary>
        /// <param name="parameters">The list of distribution parameters.</param>
        public double[] LinearMomentsFromParameters(IList<double> parameters)
        {
            double xi = parameters[0];
            double alpha = parameters[1];
            double kappa = parameters[2];
            double hondo = parameters[3];
            if (((kappa < -1) && (hondo >= 0)) || ((hondo < 0) && ((kappa <= -1) || (kappa >= -1 / hondo))))
            {
                throw new ArgumentOutOfRangeException(nameof(parameters), "L-moments can only be defined for hondo (h) >= 0 and kappa (k) > -1, or if h < 0 and -1 < k < -1/h.");
            }

            double L1;
            double L2;
            double T3;
            double T4;
            if (kappa == 0.0d) kappa = Math.Pow(10d, -100);
            if (hondo == 0.0d)
            {
                L1 = xi + alpha * (1.0d - Gamma.Function(1.0d + kappa)) / kappa;
                L2 = alpha * (1.0d - Math.Pow(2.0d, -kappa)) * Gamma.Function(1.0d + kappa) / kappa;
                T3 = 2.0d * (1.0d - Math.Pow(3.0d, -kappa)) / (1.0d - Math.Pow(2.0d, -kappa)) - 3.0d;
                T4 = (5.0d * (1.0d - Math.Pow(4.0d, -kappa)) - 10.0d * (1.0d - Math.Pow(3.0d, -kappa)) + 6.0d * (1.0d - Math.Pow(2.0d, -kappa))) / (1.0d - Math.Pow(2.0d, -kappa));
            }
            else
            {
                var g = new double[4];
                for (int r = 1; r <= 4; r++)
                {
                    if (hondo > 0.0d)
                    {
                        g[r - 1] = r * Gamma.Function(1d + kappa) * Gamma.Function(r / hondo) / (Math.Pow(hondo, 1d + kappa) * Gamma.Function(1d + kappa + r / hondo));
                    }
                    else
                    {
                        g[r - 1] = r * Gamma.Function(1d + kappa) * Gamma.Function(-kappa - r / hondo) / (Math.Pow(-hondo, 1d + kappa) * Gamma.Function(1d - r / hondo));
                    }
                }

                L1 = xi + alpha * (1d - g[0]) / kappa;
                L2 = alpha * (g[0] - g[1]) / kappa;
                T3 = (-g[0] + 3d * g[1] - 2d * g[2]) / (g[0] - g[1]);
                T4 = -(-g[0] + 6d * g[1] - 10d * g[2] + 5d * g[3]) / (g[0] - g[1]);
            }
            // 
            return new[] { L1, L2, T3, T4 };
        }

           
        /// <summary>
        /// Get the initial, lower, and upper values for the distribution parameters for constrained optimization.
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        /// <returns>Returns a Tuple of initial, lower, and upper values.</returns>
        public Tuple<double[], double[], double[]> GetParameterConstraints(IList<double> sample)
        {
            var initialVals = new double[NumberOfParameters];
            var lowerVals = new double[NumberOfParameters];
            var upperVals = new double[NumberOfParameters];

            // Get initial values
            try
            {
                initialVals = ParametersFromLinearMoments(Statistics.LinearMoments(sample));

                // Get bounds of location
                if (initialVals[0] == 0d) initialVals[0] = Tools.DoubleMachineEpsilon;
                lowerVals[0] = -Math.Pow(10d, Math.Ceiling(Math.Log10(Math.Abs(initialVals[0])) + 1d));
                upperVals[0] = Math.Pow(10d, Math.Ceiling(Math.Log10(Math.Abs(initialVals[0])) + 1d));

                // Get bounds of scale
                lowerVals[1] = Tools.DoubleMachineEpsilon;
                upperVals[1] = Math.Pow(10d, Math.Ceiling(Math.Log10(Math.Abs(initialVals[1])) + 1d));

                // Get bounds of shape
                lowerVals[2] = -10d;
                upperVals[2] = 10d;

                // Get bounds of shape 2
                lowerVals[3] = -5d;
                upperVals[3] = 5d;

                // Correct initial value of kappa and hondo if necessary
                if (initialVals[2] <= lowerVals[2] || initialVals[2] >= upperVals[2])
                {
                    initialVals[2] = 0d;
                }

                if (initialVals[3] <= lowerVals[3] || initialVals[3] >= upperVals[3])
                {
                    initialVals[3] = 0d;
                }

            }
            catch
            {

                // Get constraints from GEV
                var gev = new GeneralizedExtremeValue();
                var parms = gev.GetParameterConstraints(sample);
                for (int i = 0; i < 3; i++)
                {
                    initialVals[i] = parms.Item1[i];
                    lowerVals[i] = parms.Item2[i];
                    upperVals[i] = parms.Item3[i];

                    // Get bounds of shape 2
                    initialVals[3] = 0;
                    lowerVals[3] = -1d;
                    upperVals[3] = 1d;
                }

            }


            // 
            return new Tuple<double[], double[], double[]>(initialVals, lowerVals, upperVals);
        }

        /// <summary>
        /// Estimate the distribution parameters using the method of maximum likelihood estimation.
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        public double[] MLE(IList<double> sample)
        {
            // Set constraints
            var tuple = GetParameterConstraints(sample);
            var Initials = tuple.Item1;
            var Lowers = tuple.Item2;
            var Uppers = tuple.Item3;

            // Solve using Powell
            double logLH(double[] x)
            {
                var K4 = new KappaFour();
                K4.SetParameters(x);
                return K4.LogLikelihood(sample);
            }
            var solver = new NelderMead(logLH, NumberOfParameters, Initials, Lowers, Uppers);
            solver.Maximize();
            return solver.BestParameterSet.Values;

        }

        /// <summary>
        /// Gets the Probability Density Function (PDF) of the distribution evaluated at a point x.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double PDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false) ValidateParameters(new[] { Xi, Alpha, Kappa, Hondo }, true);
            if (x < Minimum || x > Maximum) return 0.0d;
            double k = Kappa;
            if (k == 0.0d) k = Math.Pow(10d, -100);
            if (1d - k * (x - Xi) / Alpha < 0d) return 0.0d;
            if (Hondo == 0.0d)
            {
                double y = (x - Xi) / Alpha;
                if (Math.Abs(k) > NearZero) y = -Math.Log(1d - k * y) / k;
                return Math.Exp(-(1d - k) * y - Math.Exp(-y)) / Alpha;
            }
            else
            {
                return 1d / Alpha * Math.Pow(1d - k * (x - Xi) / Alpha, 1d / k - 1d) * Math.Pow(CDF(x), 1d - Hondo);
            }
        }

        /// <summary>
        /// Gets the Cumulative Distribution Function (CDF) for the distribution evaluated at a point x.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double CDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false) ValidateParameters(new[] { Xi, Alpha, Kappa, Hondo }, true);
            if (x <= Minimum) return 0d;
            if (x >= Maximum) return 1d;
            double k = Kappa;
            if (k == 0.0d) k = Math.Pow(10d, -100);
            if (Hondo == 0.0d)
            {
                double y = (x - Xi) / Alpha;
                if (Math.Abs(Kappa) > NearZero) y = -Math.Log(1d - Kappa * y) / Kappa;
                return Math.Exp(-Math.Exp(-y));
            }
            else
            {
                return Math.Pow(1d - Hondo * Math.Pow(1d - k * (x - Xi) / Alpha, 1d / k), 1d / Hondo);
            }
        }

        /// <summary>
        /// Gets the Inverse Cumulative Distribution Function (ICFD) of the distribution evaluated at a probability.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        public override double InverseCDF(double probability)
        {
            // Validate probability
            if (probability < 0.0d || probability > 1.0d)
                throw new ArgumentOutOfRangeException("probability", "Probability must be between 0 and 1.");
            if (probability == 0.0d)
                return Minimum;
            if (probability == 1.0d)
                return Maximum;
            // Validate parameters
            if (_parametersValid == false) ValidateParameters(new[] { Xi, Alpha, Kappa, Hondo }, true);
            double k = Kappa;
            if (k == 0.0d) k = Math.Pow(10d, -100);
            if (Hondo == 0.0d)
            {
                if (Math.Abs(k) <= NearZero)
                {
                    return Xi - Alpha * Math.Log(-Math.Log(probability));
                }
                else
                {
                    return Xi + Alpha / Kappa * (1d - Math.Pow(-Math.Log(probability), Kappa));
                }
            }
            else
            {
                return Xi + Alpha / k * (1d - Math.Pow((1d - Math.Pow(probability, Hondo)) / Hondo, k));
            }
        }

        /// <summary>
        /// Returns a list of partial derivatives of X given probability with respect to each parameter.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        public double[] PartialDerivatives(double probability)
        {
            double a = Alpha, k = Kappa, h = Hondo, F = probability;

            double dxi = 1d;
            double da = (1d - Math.Pow((1d - Math.Pow(F, h)) / h, k)) / k;
            double dk = a * (-(1d - Math.Pow((1d - Math.Pow(F, h)) / h, k)) / (k * k) - Math.Pow((1d - Math.Pow(F, h)) / h, k) * Math.Log((1d - Math.Pow(F, h)) / h) / k);
            double x = 1d - Math.Pow(F, h);
            double dh = -(a * (Math.Pow(F, h) - h * Math.Log(F) * Math.Pow(F, h) - 1d) * Math.Sign(x) * Math.Pow(Math.Abs(x), k - 1d)) / (Math.Sign(h) * Math.Pow(Math.Abs(h), k + 1d));

            return new double[] { dxi, da, dk, dh };
        }

        /// <summary>
        /// Returns the determinant of the Jacobian.
        /// </summary>
        /// <param name="probabilities">List of probabilities, must be the same length as the number of distribution parameters.</param>
        public double Jacobian(IList<double> probabilities)
        {
            if (probabilities.Count != NumberOfParameters)
            {
                throw new ArgumentOutOfRangeException(nameof(Jacobian), "The number of probabilities must be the same length as the number of distribution parameters.");
            }
            // |a b c d|
            // |e f g h|
            // |i j k l|
            // |m n o p| 
            var matrix = new Matrix(4);
            for (int i = 0; i <= 3; i++)
            {
                // Get the partial derivatives for each probability
                var dFdx = PartialDerivatives(probabilities[i]);
                // Populate the Jacobian matrix
                for (int j = 0; j <= 3; j++)
                    matrix[i, j] = dFdx[j];
            }
            // Solve determinant with LU decomposition
            var LU = new LUDecomposition(matrix);
            return LU.Determinant();
        }

        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public override UnivariateDistributionBase Clone()
        {
            return new KappaFour(Xi, Alpha, Kappa, Hondo);
        }
   
    }
}