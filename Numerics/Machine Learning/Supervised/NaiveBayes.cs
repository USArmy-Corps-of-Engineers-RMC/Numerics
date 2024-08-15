/**
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
* **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.MachineLearning
{
    /// <summary>
    /// Naive-Bayes Algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     Tiki Gonzalez, USACE Risk Management Center, julian.t.gonzalez@usace.army.mil
    /// </para>
    /// <para>
    /// <b> Description: </b>
    /// </para>
    /// <para>
    /// Naive Bayes is a simple probabilistic classifier that apply Bayes' theorem.
    /// Naive-Bayes classification is meant to predict a discrete value.
    /// </para>
    /// <para>
    /// <b> References: </b>
    /// <list type="bullet">
    /// <item> <see href="https://learn.microsoft.com/en-us/archive/msdn-magazine/2019/june/test-run-simplified-naive-bayes-classification-using-csharp"/></item>
    /// <item> <see href="https://visualstudiomagazine.com/articles/2019/11/12/naive-bayes-csharp.aspx"/></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class NaiveBayes
    {
        //private double NaivePredict(int predictors, int classes, double[][] data)
        //{
        //    int N = data.Length;
        //    int[] classCounts = new int[classes];
        //    //Computes counts of each class to predict
        //    for (int i = 0; i < N; ++i)
        //    {
        //        int c = (int)data[i][classes + 1];
        //        ++classCounts[c]; //holds the number of reference items of each class
        //    }

        //    //Means of each predictor variable for each class
        //    double[][] means = new double[classes][];
        //    for (int c = 0; c < classes; ++c)
        //        means[c] = new double[predictors];

        //    for (int i = 0; i < N; ++i)
        //    {
        //        int c = (int)data[i][predictors];
        //        for (int j = 0; j < predictors; ++j)
        //            means[c][j] += data[i][j];
        //    }

        //    for (int c = 0; c < classes; ++c)
        //    {
        //        for (int j = 0; j < predictors; ++j)
        //            means[c][j] /= classCounts[c];
        //    }

        //    //Sums squared differences for each predictor
        //    double[][] variances = new double[classes][];
        //    for (int c = 0; c < classes; ++c)
        //    {
        //        variances[c] = new double[predictors];
        //    }

        //    for (int i = 0; i < N; ++i)
        //    {
        //        int c = (int)data[i][predictors];
        //        for (int j = 0; j < predictors; ++j)
        //        {
        //            double x = data[i][j];
        //            double u = means[c][j];
        //            variances[c][j] += (x - u) * (x - u);
        //        }
        //    }

        //    //Get Sample variances
        //    for (int c = 0; c < 2; ++c)
        //    {
        //        for (int j = 0; j < 3; ++j)
        //            variances[c][j] /= classCounts[c] - 1;
        //    }

        //    //Computes conditional probabilities
        //    double[][] condProbs = new double[classes][];
        //    for (int c = 0; c < classes; ++c)
        //        condProbs[c] = new double[predictors];

        //}
        ////actual prediction
        //for (int c = 0; c< 2; ++c)  // each class
        //    {
        //        for (int j = 0; j< 3; ++j)  // each predictor
        //        {
        //            double u = means[c][j];
        //double v = variances[c][j];
        //double x = unk[j];
        //condProbs[c][j] = ProbDensFunc(u, v, x);
    }
}
 
