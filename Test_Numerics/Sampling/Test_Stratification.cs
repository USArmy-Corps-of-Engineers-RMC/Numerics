﻿/**
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
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;
using Numerics.Distributions;
using Numerics.Sampling;

namespace Sampling
{
    [TestClass]
    public class Test_Stratification
    {
        [TestMethod()]
        public void Test_XValues()
        {
            var options = new StratificationOptions(100d, 200d, 100);
            var bins = Stratify.XValues(options);
            var true_midpoint = new double[] { 100.5d, 101.5d, 102.5d, 103.5d, 104.5d, 105.5d, 106.5d, 107.5d, 108.5d, 109.5d, 110.5d, 111.5d, 112.5d, 113.5d, 114.5d, 115.5d, 116.5d, 117.5d, 118.5d, 119.5d, 120.5d, 121.5d, 122.5d, 123.5d, 124.5d, 125.5d, 126.5d, 127.5d, 128.5d, 129.5d, 130.5d, 131.5d, 132.5d, 133.5d, 134.5d, 135.5d, 136.5d, 137.5d, 138.5d, 139.5d, 140.5d, 141.5d, 142.5d, 143.5d, 144.5d, 145.5d, 146.5d, 147.5d, 148.5d, 149.5d, 150.5d, 151.5d, 152.5d, 153.5d, 154.5d, 155.5d, 156.5d, 157.5d, 158.5d, 159.5d, 160.5d, 161.5d, 162.5d, 163.5d, 164.5d, 165.5d, 166.5d, 167.5d, 168.5d, 169.5d, 170.5d, 171.5d, 172.5d, 173.5d, 174.5d, 175.5d, 176.5d, 177.5d, 178.5d, 179.5d, 180.5d, 181.5d, 182.5d, 183.5d, 184.5d, 185.5d, 186.5d, 187.5d, 188.5d, 189.5d, 190.5d, 191.5d, 192.5d, 193.5d, 194.5d, 195.5d, 196.5d, 197.5d, 198.5d, 199.5d };
            for (int i = 0; i < bins.Count; i++)
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
        }

        [TestMethod()]
        public void Test_XValues_Multi()
        {
            var options = new List<StratificationOptions>();
            options.Add(new StratificationOptions(100d, 125d, 10));
            options.Add(new StratificationOptions(125d, 130d, 20));
            options.Add(new StratificationOptions(130d, 200d, 70));
            var bins = Stratify.XValues(options);
            var true_midpoint = new double[] { 101.25d, 103.75d, 106.25d, 108.75d, 111.25d, 113.75d, 116.25d, 118.75d, 121.25d, 123.75d, 125.125d, 125.375d, 125.625d, 125.875d, 126.125d, 126.375d, 126.625d, 126.875d, 127.125d, 127.375d, 127.625d, 127.875d, 128.125d, 128.375d, 128.625d, 128.875d, 129.125d, 129.375d, 129.625d, 129.875d, 130.5d, 131.5d, 132.5d, 133.5d, 134.5d, 135.5d, 136.5d, 137.5d, 138.5d, 139.5d, 140.5d, 141.5d, 142.5d, 143.5d, 144.5d, 145.5d, 146.5d, 147.5d, 148.5d, 149.5d, 150.5d, 151.5d, 152.5d, 153.5d, 154.5d, 155.5d, 156.5d, 157.5d, 158.5d, 159.5d, 160.5d, 161.5d, 162.5d, 163.5d, 164.5d, 165.5d, 166.5d, 167.5d, 168.5d, 169.5d, 170.5d, 171.5d, 172.5d, 173.5d, 174.5d, 175.5d, 176.5d, 177.5d, 178.5d, 179.5d, 180.5d, 181.5d, 182.5d, 183.5d, 184.5d, 185.5d, 186.5d, 187.5d, 188.5d, 189.5d, 190.5d, 191.5d, 192.5d, 193.5d, 194.5d, 195.5d, 196.5d, 197.5d, 198.5d, 199.5d };
            for (int i = 0; i < bins.Count; i++)
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
        }

        [TestMethod()]
        public void Test_XValues_Log10()
        {
            var options = new StratificationOptions(100d, 200d, 100);
            var bins = Stratify.XValues(options, true);
            var true_midpoint = new double[] { 100.347777502836d, 101.045751492337d, 101.748580274861d, 102.456297618163d, 103.168937524872d, 103.886534234125d, 104.60912222321d, 105.336736209223d, 106.069411150737d, 106.807182249483d, 107.550084952036d, 108.298154951525d, 109.05142818934d, 109.809940856868d, 110.573729397223d, 111.342830507004d, 112.117281138052d, 112.897118499231d, 113.682380058212d, 114.473103543273d, 115.269326945117d, 116.071088518688d, 116.878426785017d, 117.691380533071d, 118.509988821613d, 119.334290981083d, 120.164326615485d, 121.000135604291d, 121.841758104357d, 122.68923455185d, 123.542605664196d, 124.401912442031d, 125.267196171173d, 126.138498424606d, 127.015861064478d, 127.899326244109d, 128.788936410021d, 129.684734303973d, 130.586762965016d, 131.495065731564d, 132.409686243472d, 133.330668444133d, 134.258056582592d, 135.191895215669d, 136.132229210105d, 137.079103744708d, 138.032564312535d, 138.992656723068d, 139.959427104422d, 140.932921905556d, 141.91318789851d, 142.900272180647d, 143.894222176918d, 144.895085642142d, 145.902910663299d, 146.917745661838d, 147.93963939601d, 148.968640963201d, 150.004799802302d, 151.048165696075d, 152.09878877355d, 153.156719512431d, 154.222008741522d, 155.294707643172d, 156.374867755728d, 157.462540976015d, 158.557779561832d, 159.660636134457d, 160.771163681178d, 161.889415557839d, 163.015445491405d, 164.149307582538d, 165.291056308204d, 166.440746524284d, 167.598433468212d, 168.764172761627d, 169.938020413052d, 171.120032820574d, 172.310266774565d, 173.508779460402d, 174.715628461219d, 175.930871760673d, 177.154567745726d, 178.386775209458d, 179.627553353884d, 180.876961792803d, 182.13506055466d, 183.401910085432d, 184.677571251529d, 185.962105342722d, 187.255574075086d, 188.558039593965d, 189.869564476958d, 191.190211736925d, 192.520044825016d, 193.859127633718d, 195.207524499926d, 196.565300208033d, 197.932519993044d, 199.309249543709d };
            for (int i = 0; i < bins.Count; i++)
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
        }

        [TestMethod()]
        public void Test_XToProbability()
        {
            var options = new StratificationOptions(230408d, 1152038d, 100);
            var bins = Stratify.XValues(options);
            var x = new double[] { 230408d, 288010d, 345611d, 403213d, 460815d, 518417d, 576019d, 633621d, 691223d, 748825d, 806427d, 864029d, 921631d, 1036834d, 1152038d };
            var p = new double[] { 0.998826d, 0.99963644d, 0.99986387d, 0.999930195d, 0.999957544d, 0.999975831d, 0.999985218d, 0.9999904152d, 0.9999934545d, 0.9999954208d, 0.9999967533d, 0.9999976163d, 0.9999982153d, 0.999998962d, 0.99999933555d };
            var probs = Stratify.XToProbability(bins, (z) =>
            {
                var linear = new Linear(x, p);
                return linear.Interpolate(z);
            });
            var true_midpoint = new double[] { 0.998890835059304d, 0.99902050517791d, 0.999150175296518d, 0.999279845415124d, 0.999409515533731d, 0.999539185652338d, 0.999633876116337d, 0.999681926197418d, 0.99971831555019d, 0.999754704902962d, 0.999791094255733d, 0.999827483608505d, 0.999857427568477d, 0.999874482840548d, 0.999885094817519d, 0.999895706794491d, 0.999906318771462d, 0.999916930748433d, 0.999926762995645d, 0.999933477169623d, 0.999937853000128d, 0.999942228830632d, 0.999946604661136d, 0.99995098049164d, 0.999955356282814d, 0.999959007115561d, 0.999961933029212d, 0.999964858942862d, 0.999967784856513d, 0.999970710770163d, 0.999973636683814d, 0.999976028569653d, 0.999977708457037d, 0.999979210373778d, 0.999980712290519d, 0.999982214207259d, 0.999983716124d, 0.999985050440012d, 0.999986049572752d, 0.999986881122948d, 0.999987712673143d, 0.999988544223338d, 0.999989375773534d, 0.999990164163482d, 0.999990779921804d, 0.999991266208749d, 0.999991752495693d, 0.999992238782638d, 0.999992725069583d, 0.999993211356528d, 0.999993611803659d, 0.999993926410976d, 0.999994241018293d, 0.99999455562561d, 0.999994870232928d, 0.999995184840245d, 0.999995461420333d, 0.99999568729653d, 0.999995900496067d, 0.999996113695605d, 0.999996326895142d, 0.99999654009468d, 0.999996734515276d, 0.999996891375955d, 0.999997029455655d, 0.999997167535356d, 0.999997305615056d, 0.999997443694756d, 0.999997576495327d, 0.999997688175944d, 0.999997784015736d, 0.999997879855528d, 0.99999797569532d, 0.999998071535112d, 0.999998167374905d, 0.999998245163974d, 0.999998304901343d, 0.999998364637732d, 0.99999842437412d, 0.999998484110509d, 0.999998543846898d, 0.999998603583287d, 0.999998663319676d, 0.999998723056065d, 0.999998782792454d, 0.999998842528843d, 0.999998902265232d, 0.999998954538102d, 0.999998991884746d, 0.999999021768681d, 0.999999051652616d, 0.999999081536551d, 0.999999111420486d, 0.999999141304422d, 0.999999171188357d, 0.999999201072292d, 0.999999230956227d, 0.999999260840162d, 0.999999290724097d, 0.999999320608032d };
            double weights = 0d;
            for (int i = 0; i < bins.Count; i++)
            {
                Assert.AreEqual(probs[i].Midpoint, true_midpoint[i], 0.000001d);
                weights += probs[i].Weight;
            }

            Assert.AreEqual(weights, 1.0d, 0.000000001d);
        }

        [TestMethod()]
        public void Test_XToExceedanceProbability()
        {
            var options = new StratificationOptions(230408d, 1152038d, 100);
            var bins = Stratify.XValues(options);
            var x = new double[] { 230408d, 288010d, 345611d, 403213d, 460815d, 518417d, 576019d, 633621d, 691223d, 748825d, 806427d, 864029d, 921631d, 1036834d, 1152038d };
            var p = new double[] { 0.001174d, 0.00036356d, 0.00013613d, 0.000069805d, 0.000042456d, 0.000024169d, 0.000014782d, 0.0000095848d, 0.0000065455d, 0.0000045792d, 0.0000032467d, 0.0000023837d, 0.0000017847d, 0.000001038d, 0.00000066445d };
            var probs = Stratify.XToExceedanceProbability(bins, (z) =>
            {
                var linear = new Linear(x, p);
                return linear.Interpolate(z);
            });
            var true_midpoint = new double[] { 0.0011091649406965d, 0.000979494822089511d, 0.000849824703482518d, 0.000720154584875526d, 0.000590484466268533d, 0.00046081434766154d, 0.000366123883662712d, 0.000318073802581553d, 0.0002816844498099d, 0.000245295097038246d, 0.000208905744266593d, 0.00017251639149494d, 0.000142572431523343d, 0.000125517159451929d, 0.000114905182480643d, 0.000104293205509358d, 0.0000936812285380718d, 0.0000830692515667861d, 0.0000732370043548837d, 0.0000665228303765495d, 0.0000621469998724004d, 0.0000577711693682512d, 0.0000533953388641021d, 0.0000490195083599529d, 0.0000446437171860353d, 0.0000409928844389085d, 0.0000380669707883408d, 0.0000351410571377731d, 0.0000322151434872054d, 0.0000292892298366377d, 0.00002636331618607d, 0.0000239714303470366d, 0.000022291542962918d, 0.0000207896262221798d, 0.0000192877094814417d, 0.0000177857927407035d, 0.0000162838759999653d, 0.0000149495599876741d, 0.0000139504272480122d, 0.0000131188770525329d, 0.0000122873268570536d, 0.0000114557766615742d, 0.0000106242264660949d, 0.00000983583651835005d, 0.00000922007819598277d, 0.00000873379125125862d, 0.00000824750430653448d, 0.00000776121736181034d, 0.00000727493041708619d, 0.00000678864347236205d, 0.00000638819634135966d, 0.00000607358902407901d, 0.00000575898170679836d, 0.0000054443743895177d, 0.00000512976707223705d, 0.0000048151597549564d, 0.00000453857966745944d, 0.00000431270346993158d, 0.00000409950393258913d, 0.00000388630439524667d, 0.00000367310485790422d, 0.00000345990532056176d, 0.00000326548472362069d, 0.00000310862404517203d, 0.0000029705443448144d, 0.00000283246464445677d, 0.00000269438494409914d, 0.00000255630524374152d, 0.0000024235046725808d, 0.00000231182405558833d, 0.00000221598426356722d, 0.00000212014447154611d, 0.000002024304679525d, 0.00000192846488750389d, 0.00000183262509548278d, 0.00000175483602568856d, 0.00000169509865745683d, 0.0000016353622685607d, 0.00000157562587966458d, 0.00000151588949076846d, 0.00000145615310187233d, 0.00000139641671297621d, 0.00000133668032408009d, 0.00000127694393518397d, 0.00000121720754628784d, 0.00000115747115739172d, 0.0000010977347684956d, 0.00000104546189792269d, 0.00000100811525422294d, 0.000000978231319073115d, 0.000000948347383923294d, 0.000000918463448773472d, 0.000000888579513623651d, 0.00000085869557847383d, 0.000000828811643324008d, 0.000000798927708174187d, 0.000000769043773024366d, 0.000000739159837874544d, 0.000000709275902724723d, 0.000000679391967574906d };
            double weights = 0d;
            for (int i = 0; i < bins.Count; i++)
            {
                Assert.AreEqual(probs[i].Midpoint, true_midpoint[i], 0.000001d);
                weights += probs[i].Weight;
            }

            Assert.AreEqual(weights, 1.0d, 0.000000001d);

        }

        [TestMethod()]
        public void Test_ProbabilityToX()
        {
            var options = new StratificationOptions(0.998826d, 0.99999933555d, 100, true);
            var probs = Stratify.Probabilities(options);
            var x = new double[] { 230408d, 288010d, 345611d, 403213d, 460815d, 518417d, 576019d, 633621d, 691223d, 748825d, 806427d, 864029d, 921631d, 1036834d, 1152038d };
            var p = new double[] { 0.998826d, 0.99963644d, 0.99986387d, 0.999930195d, 0.999957544d, 0.999975831d, 0.999985218d, 0.9999904152d, 0.9999934545d, 0.9999954208d, 0.9999967533d, 0.9999976163d, 0.9999982153d, 0.999998962d, 0.99999933555d };
            var bins = Stratify.ProbabilityToX(probs, (z) =>
            {
                var linear = new Linear(p, x);
                return linear.Interpolate(z);
            });
            var true_midpoint = new double[] { 230824.973954586d, 231658.921863759d, 232492.869772931d, 233326.817682103d, 234160.765591276d, 234994.713500448d, 235828.661409621d, 236662.609318793d, 237496.557227965d, 238330.505137138d, 239164.45304631d, 239998.400955483d, 240832.348864655d, 241666.296773827d, 242500.244683d, 243334.192592172d, 244168.140501345d, 245002.088410517d, 245836.036319689d, 246669.984228862d, 247503.932138034d, 248337.880047207d, 249171.827956379d, 250005.775865551d, 250839.723774724d, 251673.671683896d, 252507.619593068d, 253341.567502241d, 254175.515411413d, 255009.463320586d, 255843.411229758d, 256677.35913893d, 257511.307048103d, 258345.254957275d, 259179.202866448d, 260013.15077562d, 260847.098684792d, 261681.046593965d, 262514.994503137d, 263348.94241231d, 264182.890321482d, 265016.838230654d, 265850.786139827d, 266684.734048999d, 267518.681958172d, 268352.629867344d, 269186.577776516d, 270020.525685689d, 270854.473594861d, 271688.421504034d, 272522.369413206d, 273356.317322378d, 274190.265231551d, 275024.213140723d, 275858.161049896d, 276692.108959068d, 277526.05686824d, 278360.004777413d, 279193.952686585d, 280027.900595758d, 280861.84850493d, 281695.796414102d, 282529.744323275d, 283363.692232447d, 284197.64014162d, 285031.588050792d, 285865.535959964d, 286699.483869137d, 287533.431778309d, 289359.871931198d, 292255.186544736d, 295226.883375207d, 298198.580205678d, 301170.27703615d, 304141.973866621d, 307113.670697092d, 310085.367527563d, 313057.064358034d, 316028.761188506d, 319000.458018977d, 321972.154849448d, 324943.851679919d, 327915.54851039d, 330887.245340862d, 333858.942171333d, 336830.639001804d, 339802.335832275d, 342774.032662746d, 347713.988841734d, 356263.194558817d, 366453.390465478d, 376643.586372139d, 386833.7822788d, 397023.978185462d, 413695.87907665d, 437628.979201049d, 465781.355245109d, 500113.313884502d, 560546.439196073d, 877240.842003369d };
            for (int i = 0; i < probs.Count; i++)
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
        }

        [TestMethod()]
        public void Test_ExceedanceProbabilityToX()
        {
            var options = new StratificationOptions(0.00000066445d, 0.001174d, 100, true);
            var probs = Stratify.ExceedanceProbabilities(options);
            var x = new double[] { 230408d, 288010d, 345611d, 403213d, 460815d, 518417d, 576019d, 633621d, 691223d, 748825d, 806427d, 864029d, 921631d, 1036834d, 1152038d };
            var p = new double[] { 0.001174d, 0.00036356d, 0.00013613d, 0.000069805d, 0.000042456d, 0.000024169d, 0.000014782d, 0.0000095848d, 0.0000065455d, 0.0000045792d, 0.0000032467d, 0.0000023837d, 0.0000017847d, 0.000001038d, 0.00000066445d };
            var bins = Stratify.ExceedanceProbabilityToX(probs, (z) =>
            {
                var linear = new Linear(p, x, SortOrder.Descending);
                return linear.Interpolate(z);
            });
            var true_midpoint = new double[] { 230824.973954586d, 231658.921863759d, 232492.869772935d, 233326.817682111d, 234160.765591284d, 234994.713500456d, 235828.661409632d, 236662.609318809d, 237496.557227981d, 238330.505137154d, 239164.453046326d, 239998.400955502d, 240832.348864679d, 241666.296773851d, 242500.244683023d, 243334.192592196d, 244168.140501372d, 245002.088410548d, 245836.036319721d, 246669.984228893d, 247503.93213807d, 248337.880047246d, 249171.827956418d, 250005.775865591d, 250839.723774763d, 251673.67168394d, 252507.619593116d, 253341.567502288d, 254175.515411461d, 255009.463320633d, 255843.411229809d, 256677.359138986d, 257511.307048158d, 258345.254957331d, 259179.202866503d, 260013.150775679d, 260847.098684856d, 261681.046594028d, 262514.9945032d, 263348.942412377d, 264182.890321553d, 265016.838230725d, 265850.786139898d, 266684.73404907d, 267518.681958247d, 268352.629867423d, 269186.577776595d, 270020.525685768d, 270854.47359494d, 271688.421504117d, 272522.369413293d, 273356.317322465d, 274190.265231638d, 275024.21314081d, 275858.161049986d, 276692.108959163d, 277526.056868335d, 278360.004777507d, 279193.952686684d, 280027.90059586d, 280861.848505033d, 281695.796414205d, 282529.744323377d, 283363.692232554d, 284197.64014173d, 285031.588050902d, 285865.535960075d, 286699.483869247d, 287533.431778424d, 289359.871931468d, 292255.186545158d, 295226.883375629d, 298198.5802061d, 301170.277036585d, 304141.973867071d, 307113.670697542d, 310085.367528013d, 313057.064358498d, 316028.761188984d, 319000.458019455d, 321972.154849926d, 324943.851680397d, 327915.548510882d, 330887.245341368d, 333858.942171839d, 336830.63900231d, 339802.335832781d, 342774.032663267d, 347713.988842917d, 356263.194560649d, 366453.39046731d, 376643.586373971d, 386833.782280681d, 397023.97818739d, 413695.879079953d, 437628.979205726d, 465781.35525112d, 500113.313895328d, 560546.439216146d, 877240.842375805d };
            for (int i = 0; i < probs.Count; i++)
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
        }

        [TestMethod()]
        public void Test_Probabilities()
        {
            var options = new StratificationOptions(0.0d, 1.0d, 100, true);
            var bins = Stratify.Probabilities(options);
            var true_midpoint = new double[] { 0.005d, 0.015d, 0.025d, 0.035d, 0.045d, 0.055d, 0.065d, 0.075d, 0.085d, 0.095d, 0.105d, 0.115d, 0.125d, 0.135d, 0.145d, 0.155d, 0.165d, 0.175d, 0.185d, 0.195d, 0.205d, 0.215d, 0.225d, 0.235d, 0.245d, 0.255d, 0.265d, 0.275d, 0.285d, 0.295d, 0.305d, 0.315d, 0.325d, 0.335d, 0.345d, 0.355d, 0.365d, 0.375d, 0.385d, 0.395d, 0.405d, 0.415d, 0.425d, 0.435d, 0.445d, 0.455d, 0.465d, 0.475d, 0.485d, 0.495d, 0.505d, 0.515d, 0.525d, 0.535d, 0.545d, 0.555d, 0.565d, 0.575d, 0.585d, 0.595d, 0.605d, 0.615d, 0.625d, 0.635d, 0.645d, 0.655d, 0.665d, 0.675d, 0.685d, 0.695d, 0.705d, 0.715d, 0.725d, 0.735d, 0.745d, 0.755d, 0.765d, 0.775d, 0.785d, 0.795d, 0.805d, 0.815000000000001d, 0.825000000000001d, 0.835000000000001d, 0.845000000000001d, 0.855000000000001d, 0.865000000000001d, 0.875000000000001d, 0.885000000000001d, 0.895000000000001d, 0.905000000000001d, 0.915000000000001d, 0.925000000000001d, 0.935000000000001d, 0.945000000000001d, 0.955000000000001d, 0.965000000000001d, 0.975000000000001d, 0.985000000000001d, 0.995000000000001d };
            double weights = 0d;
            for (int i = 0; i < bins.Count; i++)
            {
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
                weights += bins[i].Weight;
            }

            Assert.AreEqual(weights, 1.0d, 0.000000001d);
        }

        [TestMethod()]
        public void Test_Probabilities_Multi()
        {
            var options = new List<StratificationOptions>();
            options.Add(new StratificationOptions(0.0d, 0.25d, 10, true));
            options.Add(new StratificationOptions(0.25d, 0.3d, 20, true));
            options.Add(new StratificationOptions(0.3d, 1.0d, 70, true));
            var bins = Stratify.Probabilities(options);
            var true_midpoint = new double[] { 0.0125d, 0.0375d, 0.0625d, 0.0875d, 0.1125d, 0.1375d, 0.1625d, 0.1875d, 0.2125d, 0.2375d, 0.25125d, 0.25375d, 0.25625d, 0.25875d, 0.26125d, 0.26375d, 0.26625d, 0.26875d, 0.27125d, 0.27375d, 0.27625d, 0.27875d, 0.28125d, 0.28375d, 0.28625d, 0.28875d, 0.29125d, 0.29375d, 0.29625d, 0.29875d, 0.305d, 0.315d, 0.325d, 0.335d, 0.345d, 0.355d, 0.365d, 0.375d, 0.385d, 0.395d, 0.405d, 0.415d, 0.425d, 0.435d, 0.445d, 0.455d, 0.465d, 0.475d, 0.485d, 0.495d, 0.505d, 0.515d, 0.525d, 0.535d, 0.545d, 0.555d, 0.565d, 0.575d, 0.585d, 0.595d, 0.605d, 0.615d, 0.625d, 0.635d, 0.645d, 0.655d, 0.665d, 0.675d, 0.685d, 0.695d, 0.705d, 0.715d, 0.725d, 0.735d, 0.745d, 0.755d, 0.765d, 0.775d, 0.785d, 0.795d, 0.805d, 0.815d, 0.825d, 0.835d, 0.845d, 0.855d, 0.865d, 0.875d, 0.885d, 0.895d, 0.905d, 0.915d, 0.925d, 0.935d, 0.945000000000001d, 0.955000000000001d, 0.965000000000001d, 0.975000000000001d, 0.985000000000001d, 0.995000000000001d };
            double weights = 0d;
            for (int i = 0; i < bins.Count; i++)
            {
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
                weights += bins[i].Weight;
            }

            Assert.AreEqual(weights, 1.0d, 0.000000001d);
        }

        [TestMethod()]
        public void Test_Probabilities_Log10()
        {
            var options = new StratificationOptions(0.0d, 1.0d, 100, true);
            var bins = Stratify.Probabilities(options, Stratify.ImportanceDistribution.Log10Uniform);
            var true_midpoint = new double[] { 0.00347777502835944d, 0.0104575149233741d, 0.0174858027486113d, 0.0245629761816299d, 0.0316893752487221d, 0.0388653423412495d, 0.0460912222320944d, 0.0533673620922239d, 0.0606941115073703d, 0.0680718224948267d, 0.0755008495203604d, 0.0829815495152429d, 0.0905142818933992d, 0.0980994085686755d, 0.105737293972228d, 0.113428305070033d, 0.121172811380515d, 0.128971184992305d, 0.136823800582112d, 0.14473103543273d, 0.152693269451162d, 0.160710885186872d, 0.168784267850168d, 0.176913805330706d, 0.185099888216127d, 0.193342909810826d, 0.201643266154846d, 0.210001356042905d, 0.218417581043559d, 0.226892345518494d, 0.235426056641953d, 0.244019124420301d, 0.252671961711722d, 0.261384984246056d, 0.270158610644773d, 0.278993262441085d, 0.2878893641002d, 0.296847343039714d, 0.30586762965015d, 0.31495065731563d, 0.324096862434705d, 0.333306684441314d, 0.342580565825903d, 0.351918952156683d, 0.361322292101036d, 0.370791037447072d, 0.380325643125337d, 0.389926567230667d, 0.399594271044204d, 0.409329219055549d, 0.419131878985086d, 0.429002721806452d, 0.438942221769164d, 0.448950856421406d, 0.459029106632971d, 0.469177456618366d, 0.479396393960078d, 0.489686409631997d, 0.500047998023006d, 0.510481656960736d, 0.520987887735482d, 0.531567195124289d, 0.542220087415205d, 0.552947076431699d, 0.563748677557254d, 0.574625409760132d, 0.5855777956183d, 0.596606361344545d, 0.607711636811753d, 0.618894155578366d, 0.630154454914021d, 0.641493075825359d, 0.65291056308202d, 0.664407465242816d, 0.675984334682089d, 0.687641727616247d, 0.699380204130489d, 0.711200328205716d, 0.723102667745624d, 0.735087794603995d, 0.747156284612165d, 0.759308717606696d, 0.771545677457231d, 0.783867752094549d, 0.796275533538809d, 0.808769617927998d, 0.821350605546568d, 0.834019100854283d, 0.846775712515253d, 0.859621053427186d, 0.872555740750827d, 0.885580395939615d, 0.898695644769541d, 0.91190211736921d, 0.925200448250118d, 0.93859127633714d, 0.952075244999221d, 0.965653002080295d, 0.979325199930404d, 0.993092495437045d };
            double weights = 0d;
            for (int i = 0; i < bins.Count; i++)
            {
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
                weights += bins[i].Weight;
            }

            Assert.AreEqual(weights, 1.0d, 0.000000001d);
        }

        [TestMethod()]
        public void Test_Probabilities_Normal()
        {

            var options = new StratificationOptions(0.001d, 0.99d, 100, true);
            var bins = Stratify.Probabilities(options, Stratify.ImportanceDistribution.Normal);
            var true_midpoint = new double[] { 0.00109921622778001d, 0.00131537846594742d, 0.00156976481908554d, 0.00186825829138565d, 0.00221748135247476d, 0.00262485979154455d, 0.00309868723758612d, 0.00364818943744279d, 0.00428358723696714d, 0.00501615706334221d, 0.0058582875621811d, 0.00682353090543272d, 0.00792664715992155d, 0.00918363999643927d, 0.0106117819308723d, 0.0122296272272356d, 0.0140570105630521d, 0.0161150295654837d, 0.0184260093768648d, 0.021013447505168d, 0.023901937362071d, 0.0271170690913643d, 0.0306853065449946d, 0.0346338395732814d, 0.0389904111584927d, 0.0437831193341145d, 0.0490401942911559d, 0.0547897515713072d, 0.0610595227765424d, 0.0678765657759823d, 0.0752669569520987d, 0.0832554685868742d, 0.0918652350304632d, 0.101117411805583d, 0.111030832265268d, 0.1216216668247d, 0.132903090115122d, 0.144884961645646d, 0.157573525695161d, 0.170971136181053d, 0.185076012156496d, 0.199882029368242d, 0.215378552960328d, 0.231550315936998d, 0.248377347405152d, 0.265834953911073d, 0.283893756379449d, 0.302519784269907d, 0.321674627604951d, 0.341315646513853d, 0.361396236901842d, 0.381866149816733d, 0.402671861070378d, 0.423756986704568d, 0.445062738994208d, 0.466528416877191d, 0.488091924010908d, 0.509690307097436d, 0.531260306707634d, 0.552738912579433d, 0.574063915274116d, 0.595174446148864d, 0.616011497842233d, 0.636518417865389d, 0.656641368435367d, 0.676329746363418d, 0.69553655760407d, 0.714218741958761d, 0.732337444389461d, 0.749858230408729d, 0.766751244048732d, 0.782991307948224d, 0.798557966109581d, 0.813435470845184d, 0.827612716333183d, 0.841083122018952d, 0.853844469815099d, 0.865898699657903d, 0.877251668462816d, 0.887912877881238d, 0.897895176493533d, 0.907214442180892d, 0.915889250406042d, 0.923940534007516d, 0.931391239884224d, 0.938265987628421d, 0.944590734769141d, 0.950392452829453d, 0.955698817894198d, 0.960537918845501d, 0.96493798586586d, 0.968927141247009d, 0.972533173989963d, 0.975783339149176d, 0.978704182371667d, 0.981321389618559d, 0.983659661638403d, 0.985742612393637d, 0.987592690326722d, 0.989231121092326d };
            double weights = 0d;
            for (int i = 0; i < bins.Count; i++)
            {
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
                weights += bins[i].Weight;
            }

            Assert.AreEqual(weights, 1.0d);


        }

        [TestMethod()]
        public void Test_ExceedanceProbabilities()
        {
            var options = new StratificationOptions(0.0d, 1.0d, 100, true);
            var bins = Stratify.ExceedanceProbabilities(options);
            var true_midpoint = new double[] { 0.995d, 0.985d, 0.975d, 0.965d, 0.955d, 0.945d, 0.935d, 0.925d, 0.915d, 0.905d, 0.895d, 0.885d, 0.875d, 0.865d, 0.855d, 0.845d, 0.835d, 0.825d, 0.815d, 0.805d, 0.795d, 0.785d, 0.775d, 0.765d, 0.755d, 0.745d, 0.735d, 0.725d, 0.715d, 0.705d, 0.695d, 0.685d, 0.675d, 0.665d, 0.655d, 0.645d, 0.635d, 0.625d, 0.615d, 0.605d, 0.595d, 0.585d, 0.575d, 0.565d, 0.555d, 0.545d, 0.535d, 0.525d, 0.515d, 0.505d, 0.495d, 0.485d, 0.475d, 0.465d, 0.455d, 0.445d, 0.434999999999999d, 0.424999999999999d, 0.414999999999999d, 0.404999999999999d, 0.394999999999999d, 0.384999999999999d, 0.374999999999999d, 0.364999999999999d, 0.354999999999999d, 0.344999999999999d, 0.334999999999999d, 0.324999999999999d, 0.314999999999999d, 0.304999999999999d, 0.294999999999999d, 0.284999999999999d, 0.274999999999999d, 0.264999999999999d, 0.254999999999999d, 0.244999999999999d, 0.234999999999999d, 0.224999999999999d, 0.214999999999999d, 0.204999999999999d, 0.194999999999999d, 0.184999999999999d, 0.174999999999999d, 0.164999999999999d, 0.154999999999999d, 0.144999999999999d, 0.134999999999999d, 0.124999999999999d, 0.114999999999999d, 0.104999999999999d, 0.0949999999999993d, 0.0849999999999992d, 0.0749999999999993d, 0.0649999999999993d, 0.0549999999999993d, 0.0449999999999992d, 0.0349999999999993d, 0.0249999999999992d, 0.0149999999999992d, 0.00499999999999925d };
            double weights = 0d;
            for (int i = 0; i < bins.Count; i++)
            {
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
                weights += bins[i].Weight;
            }

            Assert.AreEqual(weights, 1.0d, 0.000000001d);
        }

        [TestMethod()]
        public void Test_ExceedanceProbabilities_Multi()
        {
            var options = new List<StratificationOptions>();
            options.Add(new StratificationOptions(0.0d, 0.25d, 10, true));
            options.Add(new StratificationOptions(0.25d, 0.3d, 20, true));
            options.Add(new StratificationOptions(0.3d, 1.0d, 70, true));
            var bins = Stratify.ExceedanceProbabilities(options);
            var true_midpoint = new double[] { 0.995d, 0.985d, 0.975d, 0.965d, 0.955d, 0.945d, 0.935d, 0.925d, 0.915d, 0.905d, 0.895d, 0.885d, 0.875d, 0.865d, 0.855d, 0.845d, 0.835d, 0.825d, 0.815d, 0.805d, 0.795d, 0.785d, 0.775d, 0.765d, 0.755d, 0.745d, 0.735d, 0.725d, 0.715d, 0.705d, 0.695d, 0.685d, 0.675d, 0.665d, 0.655d, 0.645d, 0.635d, 0.625d, 0.615d, 0.605d, 0.595d, 0.585d, 0.575d, 0.565d, 0.555d, 0.545d, 0.535d, 0.525d, 0.515d, 0.505d, 0.495d, 0.485d, 0.475d, 0.465d, 0.455d, 0.445d, 0.434999999999999d, 0.424999999999999d, 0.414999999999999d, 0.404999999999999d, 0.394999999999999d, 0.384999999999999d, 0.374999999999999d, 0.364999999999999d, 0.354999999999999d, 0.344999999999999d, 0.334999999999999d, 0.324999999999999d, 0.314999999999999d, 0.304999999999999d, 0.29875d, 0.29625d, 0.29375d, 0.29125d, 0.28875d, 0.28625d, 0.28375d, 0.28125d, 0.27875d, 0.27625d, 0.27375d, 0.27125d, 0.26875d, 0.26625d, 0.26375d, 0.26125d, 0.25875d, 0.25625d, 0.25375d, 0.25125d, 0.2375d, 0.2125d, 0.1875d, 0.1625d, 0.1375d, 0.1125d, 0.0875d, 0.0625d, 0.0375d, 0.0125d };
            double weights = 0d;
            for (int i = 0; i < bins.Count; i++)
            {
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
                weights += bins[i].Weight;
            }

            Assert.AreEqual(weights, 1.0d, 0.000000001d);
        }

        [TestMethod()]
        public void Test_ExceedanceProbabilities_Log10()
        {
            var options = new StratificationOptions(0.0d, 1.0d, 100, true);
            var bins = Stratify.ExceedanceProbabilities(options, Stratify.ImportanceDistribution.Log10Uniform);
            var true_midpoint = new double[] { 0.993092495437036d, 0.979325199930395d, 0.965653002080286d, 0.952075244999213d, 0.938591276337131d, 0.92520044825011d, 0.911902117369202d, 0.898695644769534d, 0.885580395939608d, 0.87255574075082d, 0.859621053427179d, 0.846775712515247d, 0.834019100854277d, 0.821350605546563d, 0.808769617927992d, 0.796275533538804d, 0.783867752094544d, 0.771545677457226d, 0.759308717606691d, 0.747156284612161d, 0.735087794603991d, 0.72310266774562d, 0.711200328205712d, 0.699380204130485d, 0.687641727616243d, 0.675984334682085d, 0.664407465242813d, 0.652910563082017d, 0.641493075825356d, 0.630154454914018d, 0.618894155578363d, 0.60771163681175d, 0.596606361344542d, 0.585577795618298d, 0.57462540976013d, 0.563748677557252d, 0.552947076431697d, 0.542220087415203d, 0.531567195124288d, 0.520987887735481d, 0.510481656960735d, 0.500047998023005d, 0.489686409631996d, 0.479396393960077d, 0.469177456618365d, 0.45902910663297d, 0.448950856421405d, 0.438942221769164d, 0.429002721806452d, 0.419131878985086d, 0.409329219055548d, 0.399594271044203d, 0.389926567230667d, 0.380325643125336d, 0.370791037447072d, 0.361322292101036d, 0.351918952156683d, 0.342580565825903d, 0.333306684441314d, 0.324096862434705d, 0.314950657315631d, 0.30586762965015d, 0.296847343039715d, 0.2878893641002d, 0.278993262441085d, 0.270158610644773d, 0.261384984246056d, 0.252671961711722d, 0.244019124420301d, 0.235426056641953d, 0.226892345518494d, 0.21841758104356d, 0.210001356042905d, 0.201643266154846d, 0.193342909810827d, 0.185099888216127d, 0.176913805330706d, 0.168784267850169d, 0.160710885186873d, 0.152693269451163d, 0.144731035432731d, 0.136823800582113d, 0.128971184992305d, 0.121172811380516d, 0.113428305070034d, 0.105737293972229d, 0.0980994085686764d, 0.0905142818934001d, 0.0829815495152438d, 0.0755008495203613d, 0.0680718224948276d, 0.0606941115073711d, 0.0533673620922248d, 0.0460912222320953d, 0.0388653423412504d, 0.031689375248723d, 0.0245629761816308d, 0.0174858027486122d, 0.010457514923375d, 0.00347777502836033d };
            double weights = 0d;
            for (int i = 0; i < bins.Count; i++)
            {
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
                weights += bins[i].Weight;
            }

            Assert.AreEqual(weights, 1.0d, 0.000000001d);
        }

        [TestMethod()]
        public void Test_ExceedanceProbabilities_Normal()
        {
            var options = new StratificationOptions(0.001d, 0.99d, 100, true);
            var bins = Stratify.ExceedanceProbabilities(options, Stratify.ImportanceDistribution.Normal);
            var true_midpoint = new double[] { 0.989231121092325d, 0.987592690326722d, 0.985742612393637d, 0.983659661638403d, 0.981321389618559d, 0.978704182371667d, 0.975783339149176d, 0.972533173989962d, 0.968927141247008d, 0.964937985865859d, 0.9605379188455d, 0.955698817894197d, 0.950392452829452d, 0.944590734769139d, 0.938265987628419d, 0.931391239884223d, 0.923940534007514d, 0.91588925040604d, 0.90721444218089d, 0.897895176493531d, 0.887912877881236d, 0.877251668462814d, 0.865898699657901d, 0.853844469815097d, 0.84108312201895d, 0.827612716333181d, 0.813435470845181d, 0.798557966109578d, 0.78299130794822d, 0.766751244048729d, 0.749858230408726d, 0.732337444389458d, 0.714218741958758d, 0.695536557604067d, 0.676329746363415d, 0.656641368435363d, 0.636518417865386d, 0.61601149784223d, 0.595174446148861d, 0.574063915274113d, 0.55273891257943d, 0.531260306707631d, 0.509690307097433d, 0.488091924010905d, 0.466528416877188d, 0.445062738994205d, 0.423756986704565d, 0.402671861070375d, 0.38186614981673d, 0.361396236901839d, 0.34131564651385d, 0.321674627604948d, 0.302519784269904d, 0.283893756379446d, 0.26583495391107d, 0.248377347405149d, 0.231550315936995d, 0.215378552960326d, 0.199882029368239d, 0.185076012156494d, 0.170971136181051d, 0.157573525695159d, 0.144884961645645d, 0.132903090115121d, 0.121621666824699d, 0.111030832265267d, 0.101117411805583d, 0.0918652350304623d, 0.0832554685868733d, 0.075266956952098d, 0.0678765657759818d, 0.0610595227765419d, 0.0547897515713067d, 0.0490401942911555d, 0.0437831193341142d, 0.0389904111584924d, 0.0346338395732811d, 0.0306853065449944d, 0.0271170690913641d, 0.0239019373620709d, 0.0210134475051679d, 0.0184260093768647d, 0.0161150295654836d, 0.014057010563052d, 0.0122296272272355d, 0.0106117819308722d, 0.00918363999643917d, 0.00792664715992146d, 0.00682353090543265d, 0.00585828756218104d, 0.00501615706334217d, 0.0042835872369671d, 0.00364818943744276d, 0.0030986872375861d, 0.00262485979154453d, 0.00221748135247475d, 0.00186825829138564d, 0.00156976481908553d, 0.00131537846594741d, 0.00109921622778d };
            double weights = 0d;
            for (int i = 0; i < bins.Count; i++)
            {
                Assert.AreEqual(bins[i].Midpoint, true_midpoint[i], 0.000001d);
                weights += bins[i].Weight;
            }

            Assert.AreEqual(weights, 1.0d, 0.000000001d);
        }
    }
}
