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
    public class ParameterSet
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
        public ParameterSet(IList<double> values, double fitness)
        {
            if (values != null) Values = values.ToArray();
            Fitness = fitness;
        }

        /// <summary>
        /// The trial parameter set values.
        /// </summary>
        public double[] Values { get; set; }

        /// <summary>
        /// The objective function result (or fitness) given the trial parameter set. 
        /// </summary>
        public double Fitness { get; set; }

        /// <summary>
        /// Returns a clone of the point.
        /// </summary>
        public ParameterSet Clone()
        {
            return new ParameterSet(Values, Fitness);
        }
    }
}
