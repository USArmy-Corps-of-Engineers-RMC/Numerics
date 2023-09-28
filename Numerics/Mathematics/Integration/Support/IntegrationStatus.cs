using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.Mathematics
{
    /// <summary>
    /// Enumeration of integration statuses.
    /// </summary>
    public enum IntegrationStatus
    {
        /// <summary>
        /// Integration has not been performed yet. 
        /// </summary>
        None,

        /// <summary>
        /// The integration ended successfully.
        /// </summary>
        Success,

        /// <summary>
        /// The integration was stopped because the maximum number of iterations was reached. 
        /// </summary>
        MaximumIterationsReached,

        /// <summary>
        /// The integration was stopped because the maximum number of objective function evaluations was reached. 
        /// </summary>
        MaximumFunctionEvaluationsReached,

        /// <summary>
        /// The integration was stopped due to internal failure. 
        /// </summary>
        Failure
    }
}
