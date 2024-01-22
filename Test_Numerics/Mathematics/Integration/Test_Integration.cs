using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using Numerics.Data;
using Numerics.Distributions;
using Numerics.Sampling;

namespace Mathematics.Integration
{
    /// <summary>
    /// Test integration methods.
    /// </summary>
    [TestClass()]
    public class Test_Integration
    {


        /// <summary>
        /// Gauss-Legendre.
        /// </summary>
        [TestMethod()]
        public void Test_GaussLegendre()
        {
            double e = Numerics.Mathematics.Integration.Integration.GaussLegendre(Integrands.FX3, 0d, 1d);
            double val = 0.25d;
            Assert.AreEqual(e, val, 1E-3);

        }

        /// <summary>
        /// Trapezoidal Rule Method. 
        /// </summary>
        [TestMethod()]
        public void Test_TrapezoidalRule()
        {
            double e = Numerics.Mathematics.Integration.Integration.TrapezoidalRule(Integrands.FX3, 0d, 1d, 1000);
            double val = 0.25d;
            Assert.AreEqual(e, val, 1E-3);

        }


        /// <summary>
        /// Simpsons Rule Method. 
        /// </summary>
        [TestMethod()]
        public void Test_SimpsonsRule()
        {
            double e = Numerics.Mathematics.Integration.Integration.SimpsonsRule(Integrands.FX3, 0d, 1d, 1000);
            double val = 0.25d;
            Assert.AreEqual(e, val, 1E-3);

        }

        /// <summary>
        /// Midpoint Method. 
        /// </summary>
        [TestMethod()]
        public void Test_MidPoint()
        {
            double e = Numerics.Mathematics.Integration.Integration.Midpoint(Integrands.FX3, 0d, 1d, 1000);
            double val = 0.25d;
            Assert.AreEqual(e, val, 1E-3);

        }


    }
}