using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Mathematics.Optimization
{
    /// <summary>
    /// A class for storing an optimization trial parameter set.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    [Serializable]
    public struct ParameterSet
    {
        /// <summary>
        /// Constructs an empty parameter set.
        /// </summary>
        public ParameterSet() { }

        /// <summary>
        /// Constructs a parameter set.
        /// </summary>
        /// <param name="values">The parameter values.</param>
        /// <param name="fitness">The objective function result (or fitness) given the parameter set.</param>
        public ParameterSet(double[] values, double fitness)
        {
            Values = values.ToArray();
            Fitness = fitness;
        }

        /// <summary>
        /// Constructs a parameter set.
        /// </summary>
        /// <param name="values">The parameter values.</param>
        /// <param name="fitness">The objective function result (or fitness) given the parameter set.</param>
        /// <param name="weight">The weight given to the parameter set.</param>
        public ParameterSet(double[] values, double fitness, double weight)
        {
            Values = values.ToArray();
            Fitness = fitness;
            Weight = weight;
        }

        /// <summary>
        /// The trial parameter set values.
        /// </summary>
        public double[] Values;

        /// <summary>
        /// The objective function result (or fitness) given the trial parameter set. 
        /// </summary>
        public double Fitness;

        /// <summary>
        /// An optional weight given to the parameter set values.
        /// </summary>
        public double Weight;

        /// <summary>
        /// Returns a clone of the point.
        /// </summary>
        public ParameterSet Clone()
        {
            return new ParameterSet(Values, Fitness, Weight);
        }
    }
}
