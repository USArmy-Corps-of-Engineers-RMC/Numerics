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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;
using Numerics.MachineLearning;
using Numerics.Mathematics.LinearAlgebra;
using Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Numerics.Data.Statistics;

namespace MachineLearning
{
    /// <summary>
    /// Unit tests for Decision Tree classification and regression.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     <list type="bullet"> 
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    ///     <item> Tiki Gonzalez, USACE Risk Management Center, julian.t.gonzalez@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    [TestClass]
    public class Test_DecisionTree
    {
        /// <summary>
        /// Decision Tree Classification is tested against the Iris dataset. 
        /// </summary>
        [TestMethod]
        public void Test_DecisionTree_Iris()
        {
            // Create Training data based on 70% split
            var sepalLengthTrain = new double[] { 5.1, 4.7, 5, 5.4, 5, 4.9, 5.4, 4.8, 5.8, 5.7, 5.1, 5.1, 5.4, 4.6, 4.8, 5, 5.2, 4.7, 4.8, 5.2, 4.9, 5, 4.9, 5.1, 5, 4.4, 5.1, 4.8, 4.6, 5, 7, 6.9, 6.5, 5.7, 4.9, 5.2, 5, 6, 5.6, 6.7, 5.8, 5.6, 5.9, 6.3, 6.4, 6.6, 6.7, 5.7, 5.5, 5.8, 5.4, 6, 6.3, 5.5, 5.5, 5.8, 5.6, 5.7, 6.2, 5.7, 6.3, 7.1, 6.5, 7.6, 7.3, 7.2, 6.5, 6.8, 5.8, 6.4, 7.7, 6, 6.9, 7.7, 6.7, 7.2, 6.1, 7.2, 7.4, 6.4, 6.1, 7.7, 6.4, 6.9, 6.7, 5.8, 6.7, 6.7, 6.5, 5.9 };
            var sepalWidthTrain = new double[] { 3.5, 3.2, 3.6, 3.9, 3.4, 3.1, 3.7, 3, 4, 4.4, 3.5, 3.8, 3.4, 3.6, 3.4, 3, 3.5, 3.2, 3.1, 4.1, 3.1, 3.2, 3.6, 3.4, 3.5, 3.2, 3.8, 3, 3.2, 3.3, 3.2, 3.1, 2.8, 2.8, 2.4, 2.7, 2, 2.2, 2.9, 3.1, 2.7, 2.5, 3.2, 2.5, 2.9, 3, 3, 2.6, 2.4, 2.7, 3, 3.4, 2.3, 2.5, 2.6, 2.6, 2.7, 3, 2.9, 2.8, 3.3, 3, 3, 3, 2.9, 3.6, 3.2, 3, 2.8, 3.2, 3.8, 2.2, 3.2, 2.8, 3.3, 3.2, 3, 3, 2.8, 2.8, 2.6, 3, 3.1, 3.1, 3.1, 2.7, 3.3, 3, 3, 3 };
            var petalLengthTrain = new double[] { 1.4, 1.3, 1.4, 1.7, 1.5, 1.5, 1.5, 1.4, 1.2, 1.5, 1.4, 1.5, 1.7, 1, 1.9, 1.6, 1.5, 1.6, 1.6, 1.5, 1.5, 1.2, 1.4, 1.5, 1.3, 1.3, 1.9, 1.4, 1.4, 1.4, 4.7, 4.9, 4.6, 4.5, 3.3, 3.9, 3.5, 4, 3.6, 4.4, 4.1, 3.9, 4.8, 4.9, 4.3, 4.4, 5, 3.5, 3.8, 3.9, 4.5, 4.5, 4.4, 4, 4.4, 4, 4.2, 4.2, 4.3, 4.1, 6, 5.9, 5.8, 6.6, 6.3, 6.1, 5.1, 5.5, 5.1, 5.3, 6.7, 5, 5.7, 6.7, 5.7, 6, 4.9, 5.8, 6.1, 5.6, 5.6, 6.1, 5.5, 5.4, 5.6, 5.1, 5.7, 5.2, 5.2, 5.1 };
            var petalWidthTrain = new double[] { 0.2, 0.2, 0.2, 0.4, 0.2, 0.1, 0.2, 0.1, 0.2, 0.4, 0.3, 0.3, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.1, 0.2, 0.2, 0.1, 0.2, 0.3, 0.2, 0.4, 0.3, 0.2, 0.2, 1.4, 1.5, 1.5, 1.3, 1, 1.4, 1, 1, 1.3, 1.4, 1, 1.1, 1.8, 1.5, 1.3, 1.4, 1.7, 1, 1.1, 1.2, 1.5, 1.6, 1.3, 1.3, 1.2, 1.2, 1.3, 1.2, 1.3, 1.3, 2.5, 2.1, 2.2, 2.1, 1.8, 2.5, 2, 2.1, 2.4, 2.3, 2.2, 1.5, 2.3, 2, 2.1, 1.8, 1.8, 1.6, 1.9, 2.2, 1.4, 2.3, 1.8, 2.1, 2.4, 1.9, 2.5, 2.3, 2, 1.8 };
            var speciesTrain = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

            var trainList = new List<double[]> { sepalLengthTrain, sepalWidthTrain, petalLengthTrain, petalWidthTrain };
            var Y_training = new Vector(speciesTrain) { Header = "Species" };
            var X_training = new Matrix(trainList) { Header = new string[] { "Sepal Length", "Sepal Width", "Petal Length", "Petal Width" } };

            // Create Decision Tree
            var tree = new DecisionTree(X_training, Y_training, 12345) { IsRegression = false, Features = 4 };
            tree.Train();

            // Create Test data based on 30% split
            var sepalLengthTest = new double[] { 4.9, 4.6, 4.6, 4.4, 4.8, 4.3, 5.4, 5.7, 5.1, 5.1, 5, 5.2, 5.4, 5.5, 5.5, 4.4, 4.5, 5, 5.1, 5.3, 6.4, 5.5, 6.3, 6.6, 5.9, 6.1, 5.6, 6.2, 6.1, 6.1, 6.8, 6, 5.5, 6, 6.7, 5.6, 6.1, 5, 5.7, 5.1, 5.8, 6.3, 4.9, 6.7, 6.4, 5.7, 6.5, 7.7, 5.6, 6.3, 6.2, 6.4, 7.9, 6.3, 6.3, 6, 6.9, 6.8, 6.3, 6.2 };
            var sepalWidthTest = new double[] { 3, 3.1, 3.4, 2.9, 3.4, 3, 3.9, 3.8, 3.7, 3.3, 3.4, 3.4, 3.4, 4.2, 3.5, 3, 2.3, 3.5, 3.8, 3.7, 3.2, 2.3, 3.3, 2.9, 3, 2.9, 3, 2.2, 2.8, 2.8, 2.8, 2.9, 2.4, 2.7, 3.1, 3, 3, 2.3, 2.9, 2.5, 2.7, 2.9, 2.5, 2.5, 2.7, 2.5, 3, 2.6, 2.8, 2.7, 2.8, 2.8, 3.8, 2.8, 3.4, 3, 3.1, 3.2, 2.5, 3.4 };
            var petalLengthTest = new double[] { 1.4, 1.5, 1.4, 1.4, 1.6, 1.1, 1.3, 1.7, 1.5, 1.7, 1.6, 1.4, 1.5, 1.4, 1.3, 1.3, 1.3, 1.6, 1.6, 1.5, 4.5, 4, 4.7, 4.6, 4.2, 4.7, 4.5, 4.5, 4, 4.7, 4.8, 4.5, 3.7, 5.1, 4.7, 4.1, 4.6, 3.3, 4.2, 3, 5.1, 5.6, 4.5, 5.8, 5.3, 5, 5.5, 6.9, 4.9, 4.9, 4.8, 5.6, 6.4, 5.1, 5.6, 4.8, 5.1, 5.9, 5, 5.4 };
            var petalWidthTest = new double[] { 0.2, 0.2, 0.3, 0.2, 0.2, 0.1, 0.4, 0.3, 0.4, 0.5, 0.4, 0.2, 0.4, 0.2, 0.2, 0.2, 0.3, 0.6, 0.2, 0.2, 1.5, 1.3, 1.6, 1.3, 1.5, 1.4, 1.5, 1.5, 1.3, 1.2, 1.4, 1.5, 1, 1.6, 1.5, 1.3, 1.4, 1, 1.3, 1.1, 1.9, 1.8, 1.7, 1.8, 1.9, 2, 1.8, 2.3, 2, 1.8, 1.8, 2.1, 2, 1.5, 2.4, 1.8, 2.3, 2.3, 1.9, 2.3 };
            var speciesTest = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

            var testList = new List<double[]> { sepalLengthTest, sepalWidthTest, petalLengthTest, petalWidthTest };
            var Y_test = new Vector(speciesTest) { Header = "Species" };
            var X_test = new Matrix(testList) { Header = new string[] { "Sepal Length", "Sepal Width", "Petal Length", "Petal Width" } };

            // Make predictions
            var prediction = tree.Predict(X_test);
            var accuracy = GoodnessOfFit.Accuracy(Y_test.Array, prediction);

            // Accuracy should be greater than or equal to 90%
            Assert.IsTrue(accuracy >= 90);

        }

        /// <summary>
        /// Testing the Decision Tree (DT) regression method. This test is mainly meant for demonstration. 
        /// I compare the DT regression against linear regression, and show the DT has worse performance against the observed data. 
        /// Use a Random Forest to get better performance.
        /// <para>
        /// <see cref="https://otexts.com/fpp3/least-squares.html"/>
        /// </para>
        /// </summary>
        [TestMethod]
        public void Test_DecisionTree_Regression()
        {
            var consumption = new double[] { 0.61598622, 0.46037569, 0.87679142, -0.27424514, 1.89737076, 0.91199291, 0.79453885, 1.64858747, 1.31372218, 1.89147495, 1.530714, 2.31829471, 1.81073916, -0.04173996, 0.35423556, -0.29163216, -0.87702794, 0.35113555, 0.4095977, -1.47580863, 0.83225762, 1.65583461, 1.41942029, 1.05437932, 1.97998024, 0.91391607, 1.05532326, 1.29889825, 1.13637586, 0.54994073, 0.94985262, 1.49599724, 0.57549599, 2.1112096, 0.41796279, 0.7979271, 0.50584598, -0.05775339, 0.9773001, 0.26826982, -0.15391875, -2.27411019, 1.07188123, 1.31644941, 0.5247277, -0.01728203, 0.4016515, -0.7528762, 0.65938376, 0.36854173, 0.76954464, 1.80876006, 0.96802954, 1.95946831, 1.73949442, 1.56389332, 0.84526442, 1.41504495, 0.76546608, 1.31380062, 1.6865532, 0.9343699, 1.90256675, 0.25656565, 0.84304279, 1.1117739, 1.79499406, 0.63768446, 0.01569397, 1.37731686, 1.15225712, 0.21016439, 1.76316026, 0.73053714, 0.85083233, 1.13789838, 0.46064152, 0.46937808, 0.98950145, 0.43942767, 0.85543417, 0.31230451, 0.40261313, -0.75910716, -0.34535008, 0.83564224, 0.48439843, -0.02626579, 1.85996999, 0.68354371, 1.07661214, 1.18372396, 0.37817936, 0.89392729, 1.09813766, 0.88122025, 1.14064791, 0.77176225, 0.77214364, 1.07014805, 0.26420505, 0.89311141, 0.91264702, 0.70025425, 0.92360967, 1.07997887, 0.60055799, 0.78298122, 1.04949253, 0.45219855, 1.69654264, 1.18062797, 1.02693626, 1.75069399, 1.30596977, 1.45888615, 0.94821191, 1.46971415, 1.12921436, 1.45748895, 1.51106759, 0.95508878, 0.96797647, 0.88629738, 0.42159086, 0.25689982, 0.36381084, 1.51630321, 0.29958257, 0.50899032, 0.69667241, 0.53634306, 0.43826169, 1.10719086, 1.46377882, 0.77334046, 0.96768535, 0.64760607, 0.95117167, 1.02041702, 0.76172556, 1.08136588, 0.77186494, 0.37591485, 1.11522822, 0.53100554, 0.58208747, 1.01434389, 0.52486184, 0.33874119, 0.44391875, 0.12505584, -0.20652548, 0.16783443, -0.72499446, -1.21068558, -0.3435437, -0.45174364, 0.60491332, -0.01115014, 0.5348174, 0.81040406, 0.64501881, 1.01833874, 0.50041315, 0.20141978, 0.43372599, 0.33593895, 0.60108995, 0.16942956, 0.26416034, 0.27877186, 0.46861292, 0.20545802, 0.46641787, 0.83917367, 0.47345118, 0.93375698, 0.91687178, 1.1253325, 0.59624005, 0.70814389, 0.66496956, 0.56167978, 0.40468216, 1.04770741, 0.72959779 };
            var income = new double[] { 0.97226104, 1.16908472, 1.55327055, -0.25527238, 1.98715363, 1.44733417, 0.53181193, 1.16012514, 0.4570115, 1.01662441, 1.90410126, 3.89025866, 0.70825266, 0.79430954, 0.43381827, 1.09380979, -1.66168482, -0.93835321, 0.09448779, -0.12259599, -0.16369546, 4.53650956, -1.46376532, 0.76166351, 1.16825761, 0.51729906, 0.73370026, 0.59458339, -0.03108003, 1.23808955, 1.51880293, 1.9145624, 0.70266687, 0.98314132, 0.7199262, 0.78553605, 1.05755946, -0.86765105, 0.4710034, 0.44037974, 0.33827686, -1.46388507, 1.21301507, 1.94243865, -0.26813406, -0.02363025, 2.02680183, 0.19560628, 0.11969888, 0.57548997, 0.5348441, 0.44938311, 0.85588425, 0.70632719, 1.49810999, 2.13138911, 2.02348788, 1.64921136, 1.36163845, 0.81927319, -0.23895759, 1.90677905, -0.33536283, 1.14181151, 1.2395111, 1.31938549, 0.7047715, 0.17977925, 0.81973366, -0.97505791, 1.80185055, 1.32743427, 1.44861875, 1.02084894, 0.95820336, 0.96207024, 1.22693023, -0.29489091, 0.67822897, 0.80025832, 0.83939484, 0.59572848, 0.03740765, -0.79479735, 0.2118329, 0.69043356, 0.36205181, 0.85100324, 2.12421067, 1.04095059, 0.43562041, 0.34210852, 0.55877186, 0.17627103, 0.05868803, 0.65496353, 0.69846579, 1.05367166, 0.59247377, 1.38110661, 0.94873528, 0.22780635, 0.88957006, 0.57591998, 0.95255663, 0.95161791, 0.79369738, 0.52035746, 0.99858552, 0.85103564, 1.18352222, 1.42325742, 2.10753052, 1.38767133, 1.01464427, 0.80893032, 0.89173174, 0.24722185, 0.66729226, 1.46092242, 1.95061335, 1.03174349, 1.16178668, 0.33725343, 0.84865826, -0.08818148, 2.3367892, -1.24443353, 2.40331419, 0.50559877, -0.12828194, 0.47941927, 0.27834026, 1.43729445, 1.62544947, 0.40353864, 0.72653162, 0.98056746, 0.52450113, 1.24238706, -0.96827007, 0.78835467, 0.51136949, 0.82191843, 2.25904474, 0.14987813, 0.28490722, 1.30059162, 0.65373993, 0.1926087, 0.26238732, 0.08392938, 0.71926565, 2.08693775, -2.3261186, 0.64019534, -0.18888849, 0.70899368, -1.1034318, -0.13213193, 0.10094986, 1.29229259, 0.49678098, 0.69495229, 1.21571502, -0.15658108, 0.52891255, 0.06074719, 1.62204885, 0.76689543, -0.05071452, 2.59106697, -4.26525047, 0.58146541, 0.58328912, 0.21494896, 1.10369487, 1.29390492, 0.99853396, 1.04641801, 0.4904068, 0.95495949, 0.80166267, 0.7400626, 0.5190254, 0.72372078, 0.64470081 };
            var production = new double[] { -2.45270031, -0.55152509, -0.35870786, -2.18545486, 1.90973412, 0.90153584, 0.30801942, 2.29130441, 4.14957387, 1.89062398, 1.2733529, 3.43689207, 2.79907636, 0.81768862, 0.86899693, 1.47296187, -0.88248358, 0.07427919, -0.41314971, -4.06411893, -6.85103912, -1.33129558, 2.42435972, 2.16904208, 3.02720471, 1.27881101, 1.30386487, 1.77537765, 2.05516067, 3.05838507, 1.10308888, 0.6334685, -0.29339056, 3.94815264, 0.87114701, 1.78447991, 0.42594327, -0.20491944, -0.29723637, 0.33560928, 0.41056141, -4.30076832, -1.64181977, 3.7804552, 0.24627687, 0.30977573, 0.91707444, -2.25457797, -2.07131293, -1.24766384, -1.4005043, -1.90375664, 1.1465572, 2.17942248, 3.36771897, 2.58168445, 2.89709545, 1.53821324, 0.7212874, 0.04115557, 0.32353159, 0.07020996, -0.14046924, 0.57978813, 0.58132135, -0.57641778, 0.37249329, 1.13734778, 1.30758228, 1.75000563, 1.843662, 2.40645058, 0.92013121, 0.87316353, 0.38103668, 0.70292025, 0.43372685, -0.36675732, -0.62142121, 0.42443392, 0.68265169, 0.77446547, 0.419448, -1.57345296, -1.91422028, 0.59131506, 1.36255645, 0.21710308, -0.13365365, 1.76874773, 0.76167388, 1.05024577, 0.87901471, 0.21755108, 0.40135891, 1.49618275, 1.22213656, 1.78250275, 1.267181, 2.04370404, 1.02552601, 0.33785685, 0.90043887, 0.87467273, 0.69285195, 2.11134752, 1.2441868, 1.3539689, 1.867147, 1.48763922, 2.28632066, 2.48091341, 1.10343775, 0.65122238, 0.72551955, 1.44421674, 1.10341663, 0.98574261, 0.90279881, 1.75533234, 0.99682019, 1.23293805, -0.10225268, -0.20388383, -1.35143911, -1.25954437, -1.44101744, -1.06013675, 0.70916406, 1.54280957, 0.59478143, -0.05776556, 0.53922789, -0.69876172, 0.60727351, 1.00599126, 0.65792806, 0.5746178, 0.5633003, 1.38522763, 1.39435718, 0.50586367, -0.50305848, 0.9336501, 0.95057853, 0.5963601, 0.33552773, 0.25603401, 0.91794957, 1.19594247, 0.22356909, 0.16424632, -0.42872571, -1.41297022, -3.26349945, -4.35417741, -5.75045075, -3.00372447, 1.39880419, 1.54400617, 1.88006931, 2.05402479, 1.42683671, 0.37927209, 0.5017404, 0.21878696, 1.01113866, 0.85151692, 0.88651817, 0.62923586, 0.07880166, 0.63305509, 0.67713243, 0.30744961, 0.23440888, 0.79208722, 0.54709166, 1.33801074, 0.62352731, 0.90355427, -0.46710878, -0.69702162, 0.3806061, -0.84554638, -0.41793048, -0.20331883, 0.47491844 };
            var savings = new double[] { 4.8103115, 7.28799234, 7.28901306, 0.98522964, 3.65777061, 6.0513418, -0.44583221, -1.53087186, -4.35859438, -5.05452579, 5.80995904, 16.04471706, -5.34886849, 8.42603436, 2.75879565, 11.14642986, -2.53351449, -6.59264464, 0.51717884, 11.3433954, -5.47619069, 24.30960536, -17.65616104, 0.64809041, -2.95006644, -1.47455755, -0.06754475, -3.57672239, -9.16055658, 9.09050404, 7.94495719, 6.69627648, 2.92296383, -6.81114259, 4.79207162, 2.371184, 7.77418337, -5.28634896, -1.84549644, 4.0495981, 5.86168864, 8.24322919, 5.70775044, 9.15098787, -5.68139002, 0.88183993, 15.99035721, 7.8055065, -3.34243955, 2.19400166, 0.03499563, -9.57651468, 0.3459546, -10.17004699, 0.21217916, 8.21600068, 13.8691815, 4.38900229, 6.51686089, -2.87544931, -18.71008389, 11.8287195, -23.57393474, 11.36628338, 5.86126836, 3.27551734, -10.09044542, -4.82920131, 12.46424452, -29.52866718, 12.32810406, 16.63076101, -0.96896505, 5.67776867, 3.64649867, -0.19730358, 10.01461545, -8.15576525, -2.48622554, 5.44681102, 2.87544931, 5.10951644, -3.17767248, -0.17953326, 6.49315257, -0.30920615, -0.14086493, 11.3419301, 7.2326515, 5.46708666, -5.9364609, -5.88618856, 2.63464703, -6.91664675, -11.99337844, -1.8370887, -5.18600629, 5.15609751, -2.42215898, 6.32351898, 10.11514398, -10.60541172, -0.11570727, -2.90726686, 2.55933958, -0.75802112, 3.33843952, -3.33843952, 0.61269338, 6.17532322, -7.22796452, 5.43456565, 19.35335228, -4.81709478, -3.12983982, -9.14923404, 1.88735718, -23.49652903, -9.86264835, 2.35825225, 12.2868408, 1.28001748, 2.57390229, -13.16296208, 13.22491995, -6.89043916, 41.66826457, -56.75209674, 50.75796205, 0.87861837, -14.70397426, 1.58733492, 0.49744834, 7.00891625, 6.1841315, -6.89274778, -2.9615204, 8.30885627, -8.99318286, 6.23585017, -42.28191228, -18.27592893, -7.87665229, 20.37236078, 37.40653542, -12.34810568, -10.5527614, 6.0310008, 6.60516929, -7.23648452, -9.00674555, 2.32887238, 29.83728599, 46.43989041, -32.53252494, 36.3124049, 0.9230602, 16.09059408, -24.49229966, 0.8482922, -5.54399051, 11.65612884, -0.35208609, -3.27335958, 14.33860193, -4.07705131, 2.722504, -3.45447712, 17.6253051, 8.9694971, -3.04922177, 29.04670355, -68.78826698, 7.81647729, 3.49400682, -11.2766145, 13.52020248, 8.2440477, 2.46195256, -1.51305022, -0.75840017, 5.02391773, 3.18092976, 3.48278601, 2.23653405, -2.72150106, -0.57285793 };
            var unemployment = new double[] { 0.9, 0.5, 0.5, 0.7, -0.1, -0.1, 0.1, 0, -0.2, -0.1, -0.2, -0.3, -0.3, 0, -0.1, 0.1, 0.2, 0.3, 0.5, 1.3, 1.4, 0.2, -0.4, -0.2, -0.6, 0, 0, 0.2, -0.4, -0.2, -0.4, -0.4, -0.1, -0.4, 0.1, 0, -0.2, -0.1, 0.2, 0.1, 0.3, 1.3, -0.1, -0.3, 0.2, 0.1, 0.1, 0.9, 0.5, 0.6, 0.5, 0.7, -0.5, -0.2, -0.9, -0.9, -0.5, -0.6, 0.1, 0, -0.1, 0.2, -0.3, -0.1, 0.2, 0, -0.2, -0.4, 0, -0.4, -0.3, -0.2, 0, -0.3, 0, -0.1, -0.3, 0.3, 0, 0.1, -0.2, 0, 0.7, 0.4, 0.5, 0.1, 0, 0.4, 0.1, 0.4, -0.2, -0.2, -0.4, 0, -0.3, -0.2, 0, -0.4, -0.2, -0.4, -0.1, 0.2, 0, 0, -0.1, -0.2, -0.1, 0.2, -0.2, -0.2, -0.1, -0.2, 0, -0.2, 0.1, -0.2, -0.2, 0.1, -0.1, -0.2, 0, 0, -0.1, 0, 0.4, 0.2, 0.5, 0.7, 0, 0.1, -0.1, 0.3, -0.1, 0.4, -0.2, -0.4, 0.1, -0.2, -0.2, 0, -0.2, -0.2, 0, -0.1, -0.2, -0.1, -0.1, -0.1, 0, 0.2, 0.1, 0.3, 0.1, 0.5, 0.5, 1.2, 1.4, 0.8, 0.3, 0.1, 0, -0.5, 0.1, -0.2, -0.3, 0.1, -0.1, -0.5, -0.3, 0, -0.4, 0.1, -0.4, 0, -0.3, -0.5, 0, -0.6, -0.2, -0.3, -0.2, -0.1, -0.3, 0, 0, -0.1, 0 };

            // Create training data
            int tIdx = 118;
            var list = new List<double[]> { income.Subset(0, tIdx), production.Subset(0, tIdx), savings.Subset(0, tIdx), unemployment.Subset(0, tIdx) };
            var Y_training = new Vector(consumption.Subset(0, tIdx)) { Header = "Consumption" };
            var X_training = new Matrix(list) { Header = new string[] { "Income", "Production", "Savings", "Unemployment" } };

            // Create test data
            list = new List<double[]> { income.Subset(tIdx + 1), production.Subset(tIdx + 1), savings.Subset(tIdx + 1), unemployment.Subset(tIdx + 1) };
            var Y_test = new Vector(consumption.Subset(tIdx + 1)) { Header = "Consumption" };
            var X_test = new Matrix(list) { Header = new string[] { "Income", "Production", "Savings", "Unemployment" } };

            var tree = new DecisionTree(X_training, Y_training, 12345) { Features = 4 };
            tree.Train();
            var treePredict = tree.Predict(X_test);

            // Create linear regression
            var lm = new LinearRegression(X_training, Y_training);
            var lmPredict = lm.Predict(X_test);

            // Get R-Squared of predictions
            var treeR2 = GoodnessOfFit.RSquared(Y_test.Array, treePredict);
            var lmR2 = GoodnessOfFit.RSquared(Y_test.Array, lmPredict);

            // Linear regress is better
            Assert.IsTrue(treeR2 < lmR2);

        }

    }
}
