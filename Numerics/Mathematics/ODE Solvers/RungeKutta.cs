using System;

namespace Numerics.Mathematics.ODESolvers
{

    /// <summary>
    /// Runge-Kutta method for solving Ordinary Differential Equations (ODE).
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// In numerical analysis, the Runge–Kutta methods are a family of implicit and explicit iterative methods,
    /// which include the well-known routine called the Euler Method,
    /// used in temporal discretization for the approximate solutions of ordinary differential equations.
    /// </para>
    /// <para>
    /// References:
    /// </para>
    /// <para>
    /// <see href="https://en.wikipedia.org/wiki/Runge%E2%80%93Kutta_methods"/>
    /// </para>
    /// </remarks>
    public class RungeKutta
    {

        /// <summary>
        /// Second Order Runge-Kutta method.
        /// </summary>
        /// <param name="f">ODE function F(t,Y).</param>
        /// <param name="initialValue">Initial value of Y.</param>
        /// <param name="startTime">Start time.</param>
        /// <param name="endTime">End time.</param>
        /// <param name="timeSteps">The number of time steps between the start and end time.</param>
        /// <returns></returns>
        public static double[] SecondOrder(Func<double, double, double> f, double initialValue, double startTime, double endTime, int timeSteps)
        {
            double dt = (endTime - startTime) / (timeSteps - 1);
            double k1 = 0d;
            double k2 = 0d;
            double t = startTime;
            var y = new double[timeSteps];
            double y0 = initialValue;
            y[0] = y0;
            for (int i = 1; i < timeSteps; i++)
            {
                k1 = f(t, y0);
                k2 = f(t + dt, y0 + k1 * dt);
                y[i] = y0 + dt * 0.5d * (k1 + k2);
                t += dt;
                y0 = y[i];
            }

            return y;
        }

        /// <summary>
        /// Fourth Order Runge-Kutta method.
        /// </summary>
        /// <param name="f">ODE function F(t,Y).</param>
        /// <param name="initialValue">Initial value of Y.</param>
        /// <param name="startTime">Start time.</param>
        /// <param name="endTime">End time.</param>
        /// <param name="timeSteps">The number of time steps between the start and end time.</param>
        /// <returns></returns>
        public static double[] FourthOrder(Func<double, double, double> f, double initialValue, double startTime, double endTime, int timeSteps)
        {
            double dt = (endTime - startTime) / (timeSteps - 1);
            double k1 = 0d;
            double k2 = 0d;
            double k3 = 0d;
            double k4 = 0d;
            double t = startTime;
            var y = new double[timeSteps];
            double y0 = initialValue;
            y[0] = y0;
            for (int i = 1; i < timeSteps; i++)
            {
                k1 = f(t, y0);
                k2 = f(t + dt / 2d, y0 + k1 * dt / 2d);
                k3 = f(t + dt / 2d, y0 + k2 * dt / 2d);
                k4 = f(t + dt, y0 + k3 * dt);
                y[i] = y0 + dt / 6d * (k1 + 2d * k2 + 2d * k3 + k4);
                t += dt;
                y0 = y[i];
            }
            return y;
        }
    }
}