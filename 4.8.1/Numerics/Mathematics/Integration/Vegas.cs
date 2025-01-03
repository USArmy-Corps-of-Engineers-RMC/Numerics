﻿/*
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
*/

using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Mathematics.Integration
{
    /// <summary>
    /// A class for adaptive Monte Carlo integration for multidimensional integration.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <b> Description: </b>
    /// This method aims to reduce error in Monte Carlo simulations by using a probability distribution function to concentrate the search
    /// in those areas of the integrand that make the greatest contribution. 
    /// </para>
    /// <b> References: </b>
    /// <list type="bullet">
    /// <item><description>
    /// "Numerical Recipes, Routines and Examples in Basic", J.C. Sprott, Cambridge University Press, 1991.
    /// </description></item>
    /// <item><description>
    /// "Numerical Recipes: The art of Scientific Computing, Third Edition. Press et al. 2017.
    /// </description></item>
    /// <item><description>
    /// <see href="https://en.wikipedia.org/wiki/VEGAS_algorithm"/>
    /// </description></item>
    /// </list>
    /// </remarks>
    [Serializable]
    public class Vegas : Integrator
    {
        /// <summary>
        /// Creates a new Vegas class for adaptive Monte Carlo integration for multidimensional integration.
        /// </summary>
        /// <param name="function">The multidimensional function to integrate.</param>
        /// <param name="dimensions">The number of dimensions in the function to evaluate.</param>
        /// <param name="min">The minimum values under which the integral must be computed.</param>
        /// <param name="max">The maximum values under which the integral must be computed.</param>
        public Vegas(Func<double[], double, double> function, int dimensions, IList<double> min, IList<double> max)
        {
            if (dimensions < 1) throw new ArgumentOutOfRangeException(nameof(dimensions), "There must be at least 1 dimension to evaluate.");

            // Check if the length of the min and max values equal the number of dimensions
            if (min.Count != dimensions || max.Count != dimensions)
            {
                throw new ArgumentOutOfRangeException(nameof(min), "The minimum and maximum values must be the same length as the number of dimensions.");
            }

            if (dimensions > MXDIM)
                throw new ArgumentOutOfRangeException(nameof(dimensions), "The maximum number of dimensions is 20.");

            // Check if the minimum values are less than the maximum values
            for (int i = 0; i < min.Count; i++)
            {
                if (max[i] <= min[i])
                {
                    throw new ArgumentOutOfRangeException(nameof(max), "The maximum values cannot be less than or equal to the minimum values.");
                }
            }

            Function = function ?? throw new ArgumentNullException(nameof(function), "The function cannot be null.");
            Dimensions = dimensions;
            Min = min.ToArray();
            Max = max.ToArray();
            Random = new MersenneTwister();
            _sobol = new SobolSequence(Dimensions);

            RelativeTolerance = 1E-3;
            InitializeParameters();

        }

        private double[] _region;
        private int _bins = 50;
        private double _standardError;
        private double _chiSquared;
        private SobolSequence _sobol;

        // Make everything static allowing restarts
        private int MXDIM = 20;
        private double TINY = 1.0e-30;
        private int i, it, j, k, mds = 1, nd, ndo = 1, ng, npg;
        private double calls, dv2g, dxg, f, f2, f2b, fb, rc, ti;
        private double tsi, wgt, xjac, xn, xnd, xo, schi, si, swgt;
        private int[] ia;
        private int[] kg;
        private double[] dt;
        private double[] dx;
        private double[] r;
        private double[] x;
        private double[] xin;
        private double[,] d;
        private double[,] di;
        private double[,] xi;


        /// <summary>
        /// The multidimensional function to integrate.
        /// </summary>
        public Func<double[], double, double> Function { get; }

        /// <summary>
        /// The number of dimensions in the function to evaluate./>.
        /// </summary>
        public int Dimensions { get; }

        /// <summary>
        /// The minimum values under which the integral must be computed.
        /// </summary>
        public double[] Min { get; }

        /// <summary>
        /// The maximum values under which the integral must be computed. 
        /// </summary>
        public double[] Max { get; }

        /// <summary>
        /// Gets and sets the random number generator to be used within the Monte Carlo integration.
        /// </summary>
        public Random Random { get; set; }

        /// <summary>
        /// Determines whether to use a Sobol sequence or a pseudo-Random number generator. 
        /// </summary>
        public bool UseSobolSequence { get; set; } = true;

        /// <summary>
        /// Determines whether to check convergence and exit when integrating.
        /// </summary>
        public bool CheckConvergence { get; set; } = true;

        /// <summary>
        /// Determines how to initialize the Vegas routine. 
        /// </summary>
        /// <remarks>      
        /// If 0, then Vegas enters on a cold start. If Initialize 1, then inherit the grid from a previous call, but not its answers. If 2, then inherit the previous grid and its answers.
        /// </remarks>
        public int Initialize { get; set; } = 0;

        /// <summary>
        /// Gets and sets the number of statistically independent evaluations of the integral, per iteration. 
        /// </summary>
        public int IndependentEvaluations { get; set; } = 1000;

        /// <summary>
        /// Gets and sets the number of function evaluations within each independent evaluation. Default = 10,000.
        /// </summary>
        public int FunctionCalls { get; set; } = 10000;

        /// <summary>
        /// The damping parameter used to refine the grid. The default = 1.5.
        /// </summary>
        public double Alpha { get; set; } = 1.5;

        /// <summary>
        /// Gets and sets the number of stratification bins for each dimension. The default = 50.
        /// </summary>
        public int Bins 
        { 
            get { return _bins; }
            set 
            {
                _bins = value;
                InitializeParameters();
            }
        }

        /// <summary>
        /// Gets the stratification grid. 
        /// </summary>
        public double[,] Grid
        {
            get { return xi; }
            private set { xi = value; }
        }

        /// <summary>
        /// Gets integration standard error. 
        /// </summary>
        public double StandardError 
        { 
            get { return _standardError; } 
        }

        /// <summary>
        /// Gets the Chi-Squared statistic
        /// </summary>
        public double ChiSquared 
        { 
            get { return _chiSquared; } 
        }

        /// <summary>
        /// Initialize the parameter arrays.
        /// </summary>
        private void InitializeParameters()
        {
            ia = new int[Dimensions];
            kg = new int[Dimensions];
            dt = new double[Dimensions];
            dx = new double[Dimensions];
            r = new double[Bins];
            x = new double[Dimensions];
            xin = new double[Bins];
            d = new double[Bins, Dimensions];
            di = new double[Bins, Dimensions];
            xi = new double[Dimensions, Bins];

            // Create region array
            _region = new double[2 * Dimensions];
            for (int i = 0; i < Dimensions; i++)
                _region[i] = Min[i];
            for (int i = Dimensions; i < 2 * Dimensions; i++)
                _region[i] = Max[i - Dimensions];

            Iterations = 0;
        }

        /// <inheritdoc/>
        public override void Integrate()
        {
            Validate();

            try
            {
                // Compute
                vegas(Function, _region, Initialize, FunctionCalls, IndependentEvaluations, ref _result, ref _standardError, ref _chiSquared);

                // Update status
                if (FunctionEvaluations >= MaxFunctionEvaluations)
                {
                    Status = IntegrationStatus.MaximumFunctionEvaluationsReached;
                }
                else
                {
                    Status = IntegrationStatus.Success;
                }

            }
            catch (Exception ex)
            {
                Status = IntegrationStatus.Failure;
                if (ReportFailure) throw ex;
            }

        }

        /// <summary>
        /// Helper function for Integrate(), the actual Vegas algorithm
        /// </summary>
        /// <param name="fxn"> The function being evaluated </param>
        /// <param name="regn"> A vector consisting of ndim “lower left”coordinates of the region followed by ndim “upper right”
        /// coordinates. Specifies the rectangular volume by regn[0..2 * ndim - 1] </param>
        /// <param name="init"> The input flag that signals whether this call is a new start or a subsequent call for
        /// additional iterations </param>
        /// <param name="ncall">The approximate number of integrand evaluations per iteration.</param>
        /// <param name="itmx">The maximum number of iterations.</param>
        /// <param name="tgral">Output. The integral result.</param>
        /// <param name="sd">Output. The standard deviation of the estimate of the integral.</param>
        /// <param name="chi2a">The chi-squared per degree of freedom for all iterations up to that point.</param>
        private void vegas(Func<double[], double, double> fxn, double[] regn, int init, int ncall, int itmx, ref double tgral, ref double sd, ref double chi2a)
        {
            // Performs Monte Carlo integration of a user - supplied ndim - dimensional function fxn over a
            // rectangular volume specified by regn[0..2 * ndim - 1], a vector consisting of ndim “lower left”
            // coordinates of the region followed by ndim “upper right” coordinates. The integration consists
            // of itmx iterations, each with approximately ncall calls to the function. After each iteration
            // the grid is refined; more than 5 or 10 iterations are rarely useful. The input flag init signals
            // whether this call is a new start or a subsequent call for additional iterations (see comments in the
            // code). The input flag nprn (normally 0) controls the amount of diagnostic output. Returned
            // answers are tgral (the best estimate of the integral), sd(its standard deviation), and chi2a
            // (Chi2 per degree of freedom, an indicator of whether consistent results are being obtained). See
            // text for further details.

            int ndim = regn.Length / 2;
            if (init <= 0)
            {
                // Normal entry. Enter here on a cold start. 
                // Change mds = 0 to disable stratified sampling, i.e. use importance sampling only. 
                mds = ndo = 1;
                for (j = 0; j < ndim; j++) xi[j, 0] = 1.0;
            }
            if (init <= 1)
            {
                // Enter here to inherit the grid from a previous call, but not its answers.
                si = swgt = schi = 0.0;
            }
            if (init <= 2)
            {
                // Enter here to inherit the previous grid and its answers.
                nd = Bins;
                ng = 1;
                // Set up for stratification
                if (mds != 0)
                {
                    ng = (int)Math.Pow(ncall / 2.0 + 0.25, 1.0 / ndim);
                    mds = 1;
                    if ((2 * ng - Bins) >= 0)
                    {
                        mds = -1;
                        npg = ng / Bins + 1;
                        nd = ng / npg;
                        ng = npg * nd;
                    }
                }
                for (k = 1, i = 0; i < ndim; i++) k *= ng;
                npg = Math.Max((int)(ncall / k), 2);
                calls = (double)(npg) * (double)(k);
                dxg = 1.0 / ng;
                for (dv2g = 1, i = 0; i < ndim; i++) dv2g *= dxg;
                dv2g = Tools.Sqr(calls * dv2g) / npg / npg / (npg - 1.0);
                xnd = nd;
                dxg *= xnd;
                xjac = 1.0 / calls;
                for (j = 0; j < ndim; j++)
                {
                    dx[j] = regn[j + ndim] - regn[j];
                    xjac *= dx[j];
                }
                // Do binning if necessary
                if (nd != ndo)
                {
                    for (i = 0; i < Math.Max(nd, ndo); i++) r[i] = 1.0;
                    for (j = 0; j < ndim; j++)
                        rebin(ndo / xnd, nd, r, xin, xi, j);
                    ndo = nd;
                }

            }
            // Main iteration loop. Can enter here (init >= 3) to do an additional itmx iteration with all other parameters unchanged.
            for (it = 0; it < itmx; it++)
            {
                Iterations++;

                ti = tsi = 0.0;
                for (j = 0; j < ndim; j++)
                {
                    kg[j] = 1;
                    for (i = 0; i < nd; i++) d[i, j] = di[i, j] = 0.0;
                }
                for (; ; )
                {
                    fb = f2b = 0.0;
                    for (k = 0; k < npg; k++)
                    {
                        wgt = xjac;

                        // Sobol Sequence quasi-random numbers
                        double[] rnd = null;
                        if (UseSobolSequence)
                            rnd = _sobol.NextDouble();

                        for (j = 0; j < ndim; j++)
                        {
                            xn = (kg[j] - (UseSobolSequence ? rnd[j] : Random.NextDouble())) * dxg + 1.0;
                            ia[j] = Math.Max(Math.Min((int)xn, Bins), 1);
                            if (ia[j] > 1)
                            {
                                xo = xi[j, ia[j] - 1] - xi[j, ia[j] - 2];
                                rc = xi[j, ia[j] - 2] + (xn - ia[j]) * xo;
                            }
                            else
                            {
                                xo = xi[j, ia[j] - 1];
                                rc = (xn - ia[j]) * xo;
                            }
                            x[j] = regn[j] + rc * dx[j];
                            wgt *= xo * xnd;
                        }
                        f = wgt * fxn(x, wgt);
                        // Keep track of function evaluations
                        FunctionEvaluations++;                  
                        f2 = f * f;
                        fb += f;
                        f2b += f2;
                        for (j = 0; j < ndim; j++)
                        {
                            di[ia[j] - 1, j] += f;
                            if (mds >= 0) d[ia[j] - 1, j] += f2;
                        }
                    }
                    f2b = Math.Sqrt(f2b * npg);
                    f2b = (f2b - fb) * (f2b + fb);
                    if (f2b <= 0.0) f2b = TINY;
                    ti += fb;
                    tsi += f2b;
                    if (mds < 0)
                    {
                        // Use stratified sampling
                        for (j = 0; j < ndim; j++) d[ia[j] - 1, j] += f2b;
                    }
                    for (k = ndim - 1; k >= 0; k--)
                    {
                        kg[k] %= ng;
                        if (++kg[k] != 1) break;
                    }
                    if (k < 0) break;
                }
                // Compute final results for this iteration
                tsi *= dv2g;
                wgt = 1.0 / tsi;
                si += wgt * ti;
                schi += wgt * ti * ti;
                swgt += wgt;
                tgral = si / swgt;
                
                chi2a = (schi - si * tgral) / (it + 0.0001);
                if (chi2a < 0.0) chi2a = 0.0;
                sd = Math.Sqrt(1.0 / swgt);
                tsi = Math.Sqrt(tsi);

                // check convergence
                if ((CheckConvergence && Iterations > 1 && Math.Abs(sd/tgral) < RelativeTolerance) || FunctionEvaluations >= MaxFunctionEvaluations)
                {
                    break;
                }

                // Refine the grid. Consult references to understand the subtlety of this procedure. The refinement is damped.
                // to avoid rapid, destabilizing changes, and also compressed in range by the exponent ALPH.
                for (j = 0; j < ndim; j++)
                {
                    xo = d[0, j];
                    xn = d[1, j];
                    d[0, j] = (xo + xn) / 2.0;
                    dt[j] = d[0, j];
                    for (i = 2; i < nd; i++)
                    {
                        rc = xo + xn;
                        xo = xn;
                        xn = d[i, j];
                        d[i - 1, j] = (rc + xn) / 3.0;
                        dt[j] += d[i - 1, j];
                    }
                    d[nd - 1, j] = (xo + xn) / 2.0;
                    dt[j] += d[nd - 1, j];
                }
                for (j = 0; j < ndim; j++)
                {
                    rc = 0.0;
                    for (i = 0; i < nd; i++)
                    {
                        if (d[i, j] < TINY) d[i, j] = TINY;
                        r[i] = Math.Pow((1.0 - d[i, j] / dt[j]) /
                            (Math.Log(dt[j]) - Math.Log(d[i, j])), Alpha);
                        rc += r[i];
                    }
                    rebin(rc / xnd, nd, r, xin, xi, j);
                }

            }

        }

        /// <summary>
        /// Utility routine used by Vegas to rebin a vector of densities contained in row j of xi into new bins defined by a vector r.
        /// </summary>
        /// <remarks>
        /// This method refine the grid. Consult references to understand the subtlety of this procedure. The refinement is damped.
        /// to avoid rapid, destabilizing changes, and also compressed in range by the exponent ALPH.
        /// </remarks>
        private void rebin(double rc, int nd, double[] r, double[] xin, double[,] xi, int j)
        {
            int i, k = 0;
            double dr = 0.0, xn = 0.0, xo = 0.0;
            for (i = 0; i < nd - 1; i++)
            {
                while (rc > dr)
                    dr += r[(++k) - 1];
                if (k > 1) xo = xi[j, k - 2];
                xn = xi[j, k - 1];
                dr -= rc;
                xin[i] = xn - (xn - xo) * dr / r[k - 1];
            }
            for (i = 0; i < nd - 1; i++) xi[j, i] = xin[i];
            xi[j, nd - 1] = 1.0;
        }

    }
}
