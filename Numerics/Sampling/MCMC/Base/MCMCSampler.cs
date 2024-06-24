using Numerics.Distributions;
using Numerics.Mathematics.LinearAlgebra;
using Numerics.Mathematics;
using Numerics.Mathematics.Optimization;
using Numerics.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Numerics.Sampling.MCMC
{

    /// <summary>
    /// The log-likelihood function to evaluate.
    /// </summary>
    /// <param name="parameters">The list of parameters to evaluate.</param>
    /// <returns>The log-Likelihood given the parameter set.</returns>
    /// <remarks>
    /// This function should account for the data likelihood 
    /// as well as the prior likelihood of the sampler parameters.
    /// </remarks>
    [Serializable]
    public delegate double LogLikelihood(IList<double> parameters);


    /// <summary>
    /// A base class for all Markov Chain Monte Carlo (MCMC) samplers.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class MCMCSampler
    {

        #region Inputs
       
        /// <summary>
        /// The master pseudo random number generator (PRNG).
        /// </summary>
        protected MersenneTwister _masterPRNG;

        /// <summary>
        /// The PRNG for each Markov Chain.
        /// </summary>
        protected MersenneTwister[] _chainPRNGs;

        /// <summary>
        /// The number of simulations that have been run with this instance of the sampler. 
        /// </summary>
        protected int _simulations = 0;

        /// <summary>
        /// The current states of each chain. 
        /// </summary>
        protected ParameterSet[] _chainStates;

        /// <summary>
        /// Gets and sets the pseudo random number generator (PRNG) seed.
        /// </summary>
        public int PRNGSeed { get; set; } = 12345;

        /// <summary>
        /// Gets and sets the number of MCMC iterations to simulate.
        /// </summary>
        public int Iterations { get; set; } = 3000;

        /// <summary>
        /// Gets and sets the number of warm up MCMC iterations to discard at the beginning of the simulation.
        /// </summary>
        public int WarmupIterations { get; set; } = 1500;

        /// <summary>
        /// Gets and sets the number of Markov Chains.
        /// </summary>
        public int NumberOfChains { get; set; } = 4;

        /// <summary>
        /// Gets and sets the thinning interval. This determines how often the MCMC iterations will be recorded and evaluated.
        /// </summary>
        public int ThinningInterval { get; set; } = 20;

        /// <summary>
        /// Gets and sets the list of prior distributions for the model parameters.
        /// </summary>
        public List<IUnivariateDistribution> PriorDistributions { get; protected set; }

        /// <summary>
        /// Gets the number of parameters to evaluate.
        /// </summary>
        public int NumberOfParameters => PriorDistributions.Count;

        /// <summary>
        /// Determines the length of the initial population vector. It is recommended that the initial population be at least 10 x number of parameters in length.
        /// </summary>

        public int InitialPopulationLength { get; set; } = 10;

        /// <summary>
        /// Determines whether to update the population matrix when the chain states are recorded.
        /// </summary>
        public bool IsPopulationSampler { get; set; } = false;

        /// <summary>
        /// Determines if the chains should be sampled in parallel. Default = true.
        /// </summary>
        public bool ParallelizeChains { get; set; } = true;

        /// <summary>
        /// Determines if the MCMC simulation should be resumed. Default = false.
        /// </summary>
        public bool ResumeSimulation { get; set; } = true;

        /// <summary>
        /// Determines whether to initialize the chains using the Maximum a Posteriori (MAP) estimate and covariance matrix.
        /// </summary>
        public bool InitializeWithMAP { get; set; } = false;

        /// <summary>
        /// Determines if the Maximum a Posteriori (MAP) estimate was successful.
        /// </summary>
        protected bool _MAPsuccessful = false;

        /// <summary>
        /// The Multivariate Normal proposal distribution set from the MAP estimate.
        /// </summary>
        protected MultivariateNormal _mvn;

        /// <summary>
        /// The Log-Likelihood function to evaluate. 
        /// </summary>
        public LogLikelihood LogLikelihoodFunction { get; protected set; }

        /// <summary>
        /// Event is raised when the simulation progress changes.
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Event is raised when the simulation progress changes.
        /// </summary>
        /// <param name="percentComplete">The percent complete as decimal between 0 and 1; e.g. 10% complete is passed through the event as 0.1.</param>
        /// <param name="progressText"></param>
        public delegate void ProgressChangedEventHandler(double percentComplete, string progressText);

        /// <summary>
        /// Get and set the progress changed rate. The default is to update progress with every 1% (0.01) change in progress. 
        /// </summary>
        public double ProgressChangedRate { get; set; } = 0.01;

        /// <summary>
        /// Cancellation token source.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }

        #endregion

        #region Outputs

        /// <summary>
        /// Gets the population matrix used for population-based samplers.
        /// </summary>
        public List<ParameterSet> PopulationMatrix { get; protected set; }

        /// <summary>
        /// Gets the list of sampled Markov Chains.
        /// </summary>
        public List<ParameterSet>[] MarkovChains { get; protected set; }

        /// <summary>
        /// Keeps track of the number of accepted samples per chain.
        /// </summary>
        public int[] AcceptCount { get; protected set; }

        /// <summary>
        /// Keeps track of the number of calls to the proposal sampler per chain.
        /// </summary>
        public int[] SampleCount { get; protected set; }

        /// <summary>
        /// The acceptance rate per chain.
        /// </summary>
        public double[] AcceptanceRates
        {
            get
            {
                var ar = new double[NumberOfChains];
                for (int i = 0; i < NumberOfChains; i++)
                    ar[i] = (double)AcceptCount[i] / (double)SampleCount[i];
                return ar;
            }
        }

        /// <summary>
        /// The average log-likelihood across each chain for each iteration.
        /// </summary>
        public List<double>  MeanLogLikelihood { get; protected set; }

        /// <summary>
        /// Gets and sets the number of posterior parameter sets to output.
        /// </summary>
        public int OutputLength { get; set; } = 10000;

        /// <summary>
        /// Output posterior parameter sets. These are recorded after the iterations have been completed. 
        /// </summary>
        public List<ParameterSet>[] Output { get; protected set; }

        /// <summary>
        /// The output parameter set that produced the maximum likelihood. 
        /// This is referred to as the maximum a posteriori (MAP). 
        /// </summary>
        public ParameterSet MAP { get; protected set; }

        #endregion

        #region Simulation Methods

        /// <summary>
        /// Validate the sampler settings.
        /// </summary>
        protected virtual void ValidateSettings()
        {
            if (NumberOfChains < 1) throw new ArgumentException(nameof(NumberOfChains), "There must be at least 1 chain.");
            if (Iterations < 100) throw new ArgumentException(nameof(Iterations), "The number of iterations cannot be less than 100.");
            if (WarmupIterations < 1) throw new ArgumentException(nameof(WarmupIterations), "The number of warm up iterations cannot be less than 1.");
            if (WarmupIterations > (int)(0.5 * Iterations)) throw new ArgumentException(nameof(WarmupIterations), "The number of warm up iterations cannot be greater than half the number of iterations.");
            if (ThinningInterval < 1) throw new ArgumentException(nameof(ThinningInterval), "The thinning interval cannot be less than 1.");
            if (InitialPopulationLength < NumberOfChains) throw new ArgumentException(nameof(InitialPopulationLength), "The initial population cannot be less than the number of chains.");
            if (OutputLength < 100) throw new ArgumentException(nameof(OutputLength), "The output length must be at least 100.");
            ValidateCustomSettings();
        }

        /// <summary>
        /// Validate any custom MCMC sampler settings. 
        /// </summary>
        protected virtual void ValidateCustomSettings() { }

        /// <summary>
        /// Initialize any custom MCMC sampler settings.
        /// </summary>
        protected virtual void InitializeCustomSettings() { }


        /// <summary>
        /// Initialize the Markov Chains.
        /// </summary>
        protected virtual ParameterSet[] InitializeChains()
        {
            var prng = new MersenneTwister(PRNGSeed);
            var rnds = LatinHypercube.Random(InitialPopulationLength, NumberOfParameters, prng.Next());
            var parameters = new double[NumberOfParameters];
            var tempPopulation = new List<ParameterSet>();       
            var initials = new ParameterSet[NumberOfChains];
            double logLH = 0;

            if (InitializeWithMAP == true)
            {

                // Use differential evolution to find a global optimum
                var lowerBounds = PriorDistributions.Select(x => x.Minimum).ToList();
                var upperBounds = PriorDistributions.Select(x => x.Maximum).ToList();
                var DE = new DifferentialEvolution((x) => { return LogLikelihoodFunction(x); }, NumberOfParameters, lowerBounds, upperBounds);
                DE.ReportFailure = false;
                DE.Maximize();
                if (DE.Status == OptimizationStatus.Success)
                {
                    try
                    {
                        _MAPsuccessful = true;
                        MAP = DE.BestParameterSet.Clone();

                        // Get Fisher Information Matrix (or Hessian)
                        Matrix hessian = new Matrix(NumericalDerivative.Hessian((x) => { return LogLikelihoodFunction(x); }, MAP.Values));
                        Matrix fisher = hessian * -1d;
                        // Invert it to get the covariance matrix
                        var B = new Matrix(fisher.NumberOfRows, 0);
                        GaussJordanElimination.Solve(ref fisher, ref B);
                        // Scale it to give wider coverage
                        fisher = fisher * 1.2;

                        // Set up proposal distribution
                        _mvn = new MultivariateNormal(MAP.Values, fisher.ToArray());
                        // Then randomly sample from the proposal
                        for (int i = 0; i < InitialPopulationLength; i++)
                        {
                            parameters = _mvn.InverseCDF(rnds.GetRow(i));
                            logLH = LogLikelihoodFunction(parameters);
                            if (IsPopulationSampler) PopulationMatrix.Add(new ParameterSet(parameters, logLH));
                            tempPopulation.Add(new ParameterSet(parameters, logLH));
                        }

                        // Set the initial vectors randomly from the MVN proposal
                        for (int i = 0; i < NumberOfChains; i++)
                            initials[i] = tempPopulation[i].Clone();

                        return initials;

                    }
                    catch (Exception ex) 
                    {
                        // if this fails go to naive initialization below
                        InitializeWithMAP = false;
                    }
                }
            }


            // *** If not using MAP or if MAP fails, then use naive initialization *** //

            // First add the mean of the priors
            for (int j = 0; j < NumberOfParameters; j++)
                parameters[j] = PriorDistributions[j].Mean;
            logLH = LogLikelihoodFunction(parameters);
            if (IsPopulationSampler) PopulationMatrix.Add(new ParameterSet(parameters, logLH));
            tempPopulation.Add(new ParameterSet(parameters, logLH));

            // If the initial population and the number of chains is 1, 
            // then just take the mean of the priors
            if (InitialPopulationLength == 1 && NumberOfChains == 1)
            {
                initials[0] = tempPopulation.First().Clone();
                return initials;
            }

            // Then randomly sample from the priors
            for (int i = 1; i < InitialPopulationLength; i++)
            {
                for (int j = 0; j < NumberOfParameters; j++)
                    parameters[j] = PriorDistributions[j].InverseCDF(rnds[i, j]);
                logLH = LogLikelihoodFunction(parameters);
                if (IsPopulationSampler) PopulationMatrix.Add(new ParameterSet(parameters, logLH));
                tempPopulation.Add(new ParameterSet(parameters, logLH));
            }
            
            // Sort temp population by log-likelihood in descending order
            tempPopulation.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));

            // Set the initial vectors to the best performing parameter sets
            for (int i = 0; i < NumberOfChains; i++)
                initials[i] = tempPopulation[i].Clone();

            return initials;
        }

        /// <summary>
        /// Sample the Markov Chain. 
        /// </summary>
        /// <param name="index">The Markov Chain zero-based index</param>
        /// <param name="state">The initial state.</param>
        protected virtual ParameterSet SampleChain(int index, ParameterSet state)
        {
            // Sample until thinning interval, then return the last state.
            for (int j = 1; j <= ThinningInterval; j++)
            {
                state = ChainIteration(index, state).Clone();
            }
            return state;
        }

        /// <summary>
        /// Update the population matrix with a new chain state.
        /// </summary>
        /// <param name="state">The chain state.</param>
        protected virtual void UpdatePopulationMatrix(ParameterSet state)
        {
            PopulationMatrix.Add(state.Clone());
        }


        /// <summary>
        /// Returns a proposed MCMC parameter set and its fitness. 
        /// </summary>
        /// <param name="index">The Markov Chain zero-based index.</param>
        /// <param name="state">The current chain state to compare against.</param>
        protected abstract ParameterSet ChainIteration(int index, ParameterSet state);

        /// <summary>
        /// Sample the Markov Chains.
        /// </summary>
        public virtual void Sample()
        {
            // Validate the input settings
            ValidateSettings();

            CancellationTokenSource = new CancellationTokenSource();

            // Setup the sampler
            if (ResumeSimulation = false || _simulations < 1)
            {
                // Create inputs for the chains
                _masterPRNG = new MersenneTwister(PRNGSeed);
                _chainPRNGs = new MersenneTwister[NumberOfChains];
                PopulationMatrix = new List<ParameterSet>();
                MarkovChains = new List<ParameterSet>[NumberOfChains];
                for (int i = 0; i < NumberOfChains; i++)
                {
                    _chainPRNGs[i] = new MersenneTwister(_masterPRNG.Next());
                    MarkovChains[i] = new List<ParameterSet>();
                }

                // Create sample & accept counts
                AcceptCount = new int[NumberOfChains];
                SampleCount = new int[NumberOfChains];

                // Create mean log-likelihood list
                MeanLogLikelihood = new List<double>();

                // Keeps track of best parameter set
                MAP = new ParameterSet(new double[] { }, double.MinValue);

                // Initialize the chains
                _chainStates = InitializeChains();

                // Initialize custom settings
                InitializeCustomSettings();

            }

            // Output settings
            int outputIterations = (int)Math.Ceiling(OutputLength / (double)NumberOfChains);
            int totalIterations = Iterations + outputIterations;
            Output = new List<ParameterSet>[NumberOfChains];
            for (int i = 0; i < NumberOfChains; i++)
                Output[i] = new List<ParameterSet>();

            // progress counter
            int progress = 0;

            // Sample chains
            for (int i = 1; i <= totalIterations; i++)
            {

                if (ParallelizeChains)
                {
                    Parallel.For(0, NumberOfChains, (j) => { _chainStates[j] = SampleChain(j, _chainStates[j]); });
                }
                else
                {
                    for (int j = 0; j < NumberOfChains; j++)
                        _chainStates[j] = SampleChain(j, _chainStates[j]);
                }

                // Save chain states
                if (i <= Iterations)
                    MeanLogLikelihood.Add(0);

                // Record output
                for (int j = 0; j < NumberOfChains; j++)
                {
                    // Update population
                    if (IsPopulationSampler == true)
                        UpdatePopulationMatrix(_chainStates[j].Clone());

                    if (i <= Iterations)
                    {
                        // Record mean log-likelihood
                        MeanLogLikelihood[i - 1] += _chainStates[j].Fitness / NumberOfChains;

                        // Save chain state
                        MarkovChains[j].Add(_chainStates[j].Clone());
                    }
                    else if (i > Iterations)
                    {
                        Output[j].Add(_chainStates[j].Clone());
                        if ((InitializeWithMAP == false || _MAPsuccessful == false) && _chainStates[j].Fitness > MAP.Fitness)
                            MAP = _chainStates[j].Clone();
                    }
                }

                // Check for cancellation
                if (CancellationTokenSource.Token.IsCancellationRequested)
                    break;

                // Update progress
                progress += 1;
                if (progress % (int)(totalIterations * ProgressChangedRate) == 0)
                {
                    ReportProgress((double)progress / totalIterations);
                }

            }

            _simulations += 1;
        }

        /// <summary>
        /// Cancel the MCMC simulation.
        /// </summary>
        public void CancelSimulation()
        {
            if (CancellationTokenSource != null && CancellationTokenSource.Token != null  && CancellationTokenSource.Token.CanBeCanceled == true)
            {
                CancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Report the simulation progress.
        /// </summary>
        /// <param name="percentComplete">The percent complete as decimal between 0 and 1; e.g. 10% complete is passed through the event as 0.1.</param>
        public void ReportProgress(double percentComplete)
        {
            ProgressChanged?.Invoke(percentComplete, (percentComplete * 100) + "%");
        }

        #endregion

    }
}
