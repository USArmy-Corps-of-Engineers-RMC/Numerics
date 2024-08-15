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
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_KernelDensity
    {
        private double[] sample = new[] { 0.357932851455981d, 0.368948807016266d, 0.373962229617301d, 0.379318305574296d, 0.38434453703916d, 0.387228248578908d, 0.390085555386997d, 0.393317484754538d, 0.395410868476734d, 0.397381871394637d, 0.399511065720379d, 0.401427427939919d, 0.403112311720983d, 0.404938826331122d, 0.407144825499472d, 0.408745923542778d, 0.409897886396549d, 0.410749025674218d, 0.411697279442521d, 0.412690484005292d, 0.41382343386408d, 0.414769950543834d, 0.415905030522348d, 0.417050818649857d, 0.418222495710055d, 0.419246471342475d, 0.42015720749269d, 0.421323484455772d, 0.421931675665859d, 0.422591066734144d, 0.423306188331332d, 0.424101959637244d, 0.424736854954905d, 0.425868469845642d, 0.426700208784377d, 0.427358168565253d, 0.428182503252972d, 0.428619791009092d, 0.429485691231328d, 0.430037239867958d, 0.430615595130734d, 0.431450677154348d, 0.432169304891522d, 0.432945420448891d, 0.433975290092854d, 0.435006968587196d, 0.435539227567724d, 0.436271971034046d, 0.437036192573982d, 0.437440879778596d, 0.438090652650346d, 0.438642009527982d, 0.439188531643533d, 0.439716300565575d, 0.440255946403899d, 0.440919482885319d, 0.441611253490266d, 0.442188379664095d, 0.442889794528083d, 0.443305170329407d, 0.443769751948566d, 0.444172806941239d, 0.44472994086087d, 0.445243701939361d, 0.445893487381949d, 0.446557969818754d, 0.447047816919488d, 0.447438957069806d, 0.448012426772711d, 0.448542157694805d, 0.448949537166497d, 0.449346307308396d, 0.449973173027215d, 0.450503904953925d, 0.451055440096683d, 0.451414536272356d, 0.45198832572423d, 0.45237389043366d, 0.452824769884884d, 0.453226299551805d, 0.453797988497987d, 0.454381273056395d, 0.454727419765082d, 0.455313104043655d, 0.45568995108398d, 0.456420428178374d, 0.456747797966957d, 0.456999777791706d, 0.457551219539732d, 0.45800937152529d, 0.458432039959744d, 0.458625364154576d, 0.459103987106328d, 0.459674868289844d, 0.459877924228767d, 0.460067302646035d, 0.460279326527753d, 0.460624256638952d, 0.461012945505167d, 0.461299946707763d, 0.46174116674788d, 0.462037187185278d, 0.462372141243975d, 0.462594345046901d, 0.462926951290873d, 0.463178271982942d, 0.463610590101798d, 0.464035590630473d, 0.464366260033415d, 0.464604493812931d, 0.465027226203695d, 0.465491361314703d, 0.465770038956713d, 0.466133251907044d, 0.466385674633631d, 0.466707464482428d, 0.467034148663573d, 0.467242001447695d, 0.467520491316101d, 0.4678174754479d, 0.468090447691619d, 0.468503139053751d, 0.468771090307302d, 0.469010717057089d, 0.469431835401057d, 0.469637560114204d, 0.46990617493413d, 0.470176658231462d, 0.47055152151506d, 0.470859175264129d, 0.47143617945266d, 0.471805684770016d, 0.47219184350729d, 0.472436749766708d, 0.472667574678227d, 0.472859609421128d, 0.473033529226905d, 0.473367747759521d, 0.473769846930052d, 0.474111177666985d, 0.47443990489548d, 0.474730365193495d, 0.475129881991894d, 0.475446830894165d, 0.475699795990655d, 0.476134180945901d, 0.476322841852358d, 0.476550774511286d, 0.476811324109711d, 0.477022049979698d, 0.477189610664264d, 0.477583604689749d, 0.47777949769102d, 0.478123320899105d, 0.478250729880376d, 0.478561709455616d, 0.478923003748283d, 0.479266252873269d, 0.479584621233688d, 0.480056620602381d, 0.480188821650029d, 0.480456334145961d, 0.480967066987566d, 0.481179599727012d, 0.481436963625403d, 0.481722868958078d, 0.481913690991207d, 0.482331008331044d, 0.48255018013314d, 0.482742419938873d, 0.482984612437867d, 0.483211919510751d, 0.483558385254043d, 0.483828544315926d, 0.484154420469535d, 0.484375967872593d, 0.484573555240561d, 0.48485341483039d, 0.485283926484392d, 0.485545948755625d, 0.485865037251699d, 0.486050649839917d, 0.486202876693764d, 0.486375957005436d, 0.486674262475666d, 0.486921476127103d, 0.487192243118736d, 0.487393176767704d, 0.487572974979477d, 0.487844257811349d, 0.488205720823558d, 0.488471209513875d, 0.488699106652307d, 0.488853221025028d, 0.489022324111439d, 0.489305397591608d, 0.489486508054378d, 0.489733470956927d, 0.48998657329896d, 0.490166025478052d, 0.490443949929921d, 0.490706458657161d, 0.49094791802778d, 0.491122465301314d, 0.49129175588537d, 0.491521372921132d, 0.491716445741452d, 0.492021547857135d, 0.492192859486774d, 0.492438673496821d, 0.492754804890501d, 0.492991618064369d, 0.493165251602238d, 0.493445672930715d, 0.49371019446637d, 0.493970472108481d, 0.49421593406304d, 0.494514636384487d, 0.494744201410701d, 0.494915409296949d, 0.495059753490437d, 0.495291711322706d, 0.495504797247499d, 0.495622337792298d, 0.495830135093666d, 0.496048244283459d, 0.496416428288534d, 0.496604318801613d, 0.496784848543595d, 0.496960342707792d, 0.497229646417074d, 0.497344292436681d, 0.497647830533079d, 0.497940150766602d, 0.498130787696352d, 0.498314924593077d, 0.498511733449021d, 0.498775621543438d, 0.499088405608138d, 0.499359460959369d, 0.499557542743818d, 0.499749454552308d, 0.500028280422914d, 0.500231666447087d, 0.500540592214391d, 0.500675363599465d, 0.500902681070098d, 0.501148978603241d, 0.501349140955706d, 0.501547845872758d, 0.501681397434725d, 0.501873200060106d, 0.502060368861233d, 0.502410337460708d, 0.502594315758879d, 0.502821408192713d, 0.503081255113683d, 0.503391031526869d, 0.503647365309343d, 0.503838068660686d, 0.504120094847082d, 0.504286777887873d, 0.504511042701362d, 0.504753740056877d, 0.504872801947779d, 0.505163542172874d, 0.50539234103349d, 0.505597810603626d, 0.505694403455748d, 0.505840743858089d, 0.505967975386022d, 0.506338402095846d, 0.506488692492729d, 0.506665402035415d, 0.506764407842813d, 0.507000680088082d, 0.5071435043015d, 0.507301634127684d, 0.50742515259818d, 0.507741300761445d, 0.507964478599116d, 0.508052938725535d, 0.508285535659501d, 0.508424223652186d, 0.508559045208856d, 0.508754672872859d, 0.508956353017671d, 0.509152617771015d, 0.509331909692214d, 0.509572270491488d, 0.509786632529936d, 0.509956310105458d, 0.510209469124321d, 0.510374723652778d, 0.510493761281469d, 0.510625767142479d, 0.510803617746952d, 0.510978520367258d, 0.511197754866215d, 0.511409128368436d, 0.511635279296399d, 0.511723637763848d, 0.511903926887769d, 0.51225222263786d, 0.512437448291208d, 0.512629194817915d, 0.512853306090911d, 0.513020307044057d, 0.513246441232488d, 0.513427038232063d, 0.513589986390347d, 0.513853309195917d, 0.514046167779799d, 0.514182144860427d, 0.514405982118844d, 0.514529199829593d, 0.514861056155329d, 0.515203767442397d, 0.515433918413103d, 0.515822808849059d, 0.516033443705647d, 0.516205276451482d, 0.51640858790799d, 0.516561437262387d, 0.516725328413038d, 0.517023469200101d, 0.517182799308451d, 0.517337928758511d, 0.517510875366531d, 0.517711859744276d, 0.517866097217946d, 0.518084388841149d, 0.518346857988642d, 0.518694906796687d, 0.518948808410563d, 0.519187409713025d, 0.519368611470936d, 0.519482425759247d, 0.519689639392298d, 0.519851361242559d, 0.519999498390457d, 0.520449788348141d, 0.520653492650199d, 0.52089022552436d, 0.521033686086513d, 0.521182284801113d, 0.521322311954658d, 0.521520455789677d, 0.521747659443066d, 0.522035454843992d, 0.522155370286437d, 0.522523690480364d, 0.522670120621593d, 0.52279456060662d, 0.522978564633698d, 0.523171530370259d, 0.523329286357737d, 0.523410937348462d, 0.523568331089552d, 0.523835148523902d, 0.523939362329061d, 0.524091536648327d, 0.524211384909836d, 0.524340037206614d, 0.524512995157029d, 0.524596027008418d, 0.52477059485318d, 0.524883761312879d, 0.525029195359992d, 0.52522127650087d, 0.525433569487678d, 0.52572055734829d, 0.525940344555749d, 0.526147067538031d, 0.526271566526186d, 0.526405037863916d, 0.526623002560913d, 0.526905641528106d, 0.527080874252482d, 0.527286469524377d, 0.527494670721984d, 0.52765403286594d, 0.527832771199238d, 0.527977999603694d, 0.52809066787573d, 0.528353810005532d, 0.528536554048781d, 0.528687641112066d, 0.528925470303398d, 0.529029949326147d, 0.529147985488496d, 0.529305617414574d, 0.529492742007821d, 0.529648199324414d, 0.529904700204488d, 0.530016346859502d, 0.530177882323556d, 0.530419656518347d, 0.530708551423162d, 0.530826975666553d, 0.530948837598136d, 0.531106188626899d, 0.531202130610555d, 0.531468796264078d, 0.531717262993393d, 0.532006403638871d, 0.532148304489665d, 0.532382714511325d, 0.532591521394367d, 0.532719213752926d, 0.532995875165783d, 0.533179606234775d, 0.533315119734113d, 0.533502041403358d, 0.533667257419322d, 0.533812588822857d, 0.533980750865879d, 0.534130795032283d, 0.534269406773903d, 0.534471505935245d, 0.534569878745491d, 0.534716221984779d, 0.535020072366498d, 0.535191085346413d, 0.535470374037102d, 0.53558453415955d, 0.535781211347541d, 0.53588845368192d, 0.536019177770273d, 0.536141675407071d, 0.536302149363576d, 0.536457219304036d, 0.53669582752176d, 0.536919884860447d, 0.53711362265424d, 0.537289612953742d, 0.537457805394096d, 0.537590562179805d, 0.537699354271309d, 0.537997543795632d, 0.538258412557993d, 0.538409584302808d, 0.538648000440647d, 0.538883778336479d, 0.539048066496468d, 0.539257545194512d, 0.539361196554242d, 0.539476078112841d, 0.539598184592571d, 0.539702227105189d, 0.539855782157248d, 0.539957334831052d, 0.540112492160871d, 0.540232297208447d, 0.540305261445813d, 0.540444522424809d, 0.540574673321588d, 0.540760423277756d, 0.540987112864045d, 0.541315730204942d, 0.541522143579065d, 0.541688210871198d, 0.541830406734728d, 0.541926159820389d, 0.542089690297115d, 0.542294311379219d, 0.542457356153235d, 0.542712685122499d, 0.54281958639368d, 0.543046302849784d, 0.543218644047102d, 0.543435865879204d, 0.54357076586622d, 0.543686838474886d, 0.543803457090162d, 0.544070283861329d, 0.544220333515718d, 0.544325570275796d, 0.544634285224589d, 0.544887720145575d, 0.545017502212145d, 0.545240502402653d, 0.545454251191217d, 0.545608509124333d, 0.545789771062582d, 0.546028770706247d, 0.546197635977779d, 0.546377857319678d, 0.546497892591421d, 0.546622711963728d, 0.546783454979021d, 0.546902415476614d, 0.547281365888601d, 0.547464843402103d, 0.547605311751916d, 0.547888626254675d, 0.5481220532926d, 0.548252108350289d, 0.548599683463398d, 0.548745850998698d, 0.548898624731666d, 0.549226947786356d, 0.54951010880496d, 0.549649941692215d, 0.549836239705369d, 0.549912081059807d, 0.550004546017492d, 0.550176529544853d, 0.550409178490764d, 0.550653526696725d, 0.550775508684865d, 0.550915485043754d, 0.551018546543951d, 0.551180544322035d, 0.551293197677335d, 0.551539778426516d, 0.551718002806977d, 0.551826352565836d, 0.551944973256009d, 0.55206395235229d, 0.55228487231632d, 0.552531580209478d, 0.552810699643509d, 0.552980413092795d, 0.553074454274142d, 0.553302165565811d, 0.553469565369506d, 0.553579664104685d, 0.553859073003016d, 0.554182693120687d, 0.554351372450629d, 0.55451911025696d, 0.554757130968491d, 0.554932051313564d, 0.555151218472887d, 0.555287396966528d, 0.555484993923653d, 0.555705108165664d, 0.555920587115005d, 0.556068230443718d, 0.556243862926666d, 0.556379661826417d, 0.556547369496995d, 0.556673337218709d, 0.556803415837542d, 0.556971338951256d, 0.557315835075695d, 0.557539980630951d, 0.557707012886526d, 0.557899277900587d, 0.558122312953255d, 0.558198838749836d, 0.558296864027876d, 0.558493203817931d, 0.55876718469702d, 0.558908980257417d, 0.559051742012135d, 0.559206780078566d, 0.559397474240808d, 0.559611981855557d, 0.559801770054735d, 0.559961002542543d, 0.560094117660063d, 0.560273821778114d, 0.560496915375753d, 0.560661177717509d, 0.56091076611965d, 0.561063728929181d, 0.561167075941509d, 0.561324552765178d, 0.561468303200557d, 0.561692646907815d, 0.56182176793528d, 0.562168161286205d, 0.562392642281386d, 0.56255267703583d, 0.562656430458789d, 0.562806606970591d, 0.562976520463695d, 0.56315966573431d, 0.56332949511267d, 0.563462456300523d, 0.563709824460034d, 0.563873687173772d, 0.564094610339029d, 0.564214927235548d, 0.564407757569051d, 0.564612953973404d, 0.5648123323707d, 0.565124934070681d, 0.565282988644789d, 0.565467462969976d, 0.565605447925733d, 0.56571242050921d, 0.56597063621443d, 0.566138484009549d, 0.566340746068467d, 0.566518505558535d, 0.566751790894057d, 0.566941324204996d, 0.5671005313132d, 0.567237732623848d, 0.567411811007821d, 0.567573670400573d, 0.567791054038394d, 0.568005396411818d, 0.568162823708971d, 0.568471099451782d, 0.568765095603899d, 0.568947082100006d, 0.569245382719007d, 0.569323452291886d, 0.569483952213881d, 0.569577379439465d, 0.569782111632943d, 0.569989475423519d, 0.570154522699721d, 0.570448159397391d, 0.570680301138729d, 0.570880008483562d, 0.571028774351509d, 0.571256123506424d, 0.57148461866558d, 0.571686242313411d, 0.571841687669214d, 0.572092933995306d, 0.572328257977289d, 0.572543749058984d, 0.572776174739154d, 0.573065279216609d, 0.573340349658718d, 0.573492861440692d, 0.57369419073193d, 0.573849317384554d, 0.57404514016907d, 0.574256552877341d, 0.574446166336529d, 0.574628543404421d, 0.574780114080613d, 0.574933430599581d, 0.575147267064929d, 0.57538446661678d, 0.575645820152001d, 0.575827165482761d, 0.576168663549811d, 0.576422756370803d, 0.576553149644436d, 0.576646620546129d, 0.576863979521664d, 0.577059945733557d, 0.57725394221959d, 0.577398654631977d, 0.577584751681287d, 0.577753495892009d, 0.578004822420746d, 0.578161476944476d, 0.578332143552103d, 0.578473328921511d, 0.578652717045327d, 0.578939883833229d, 0.579165119301231d, 0.579355454666486d, 0.579520626962431d, 0.57981188717424d, 0.579946446428783d, 0.580061700623265d, 0.580249953630247d, 0.5805601182239d, 0.58084529531571d, 0.581019889841013d, 0.581194413990598d, 0.581523950890432d, 0.581851930183655d, 0.581980851349117d, 0.582173111707366d, 0.582450968591764d, 0.582640191420578d, 0.582809929840184d, 0.582971860361844d, 0.583164313367662d, 0.583259099465441d, 0.5834137548218d, 0.583741443980385d, 0.583917433289699d, 0.584118932757407d, 0.584279510279426d, 0.58457077845257d, 0.584904681899683d, 0.58521311603782d, 0.585477665086587d, 0.585715896020084d, 0.585839300615795d, 0.586286706167162d, 0.586582529571872d, 0.586835487914406d, 0.587053879097357d, 0.58750062134156d, 0.587758846177737d, 0.587916940874244d, 0.58813012284512d, 0.588271063254866d, 0.588450339681892d, 0.588581162019357d, 0.588812488526612d, 0.588955786624881d, 0.589277822772433d, 0.58946122997943d, 0.589705920395136d, 0.589922720900612d, 0.590080161523825d, 0.590283981219529d, 0.590445782025322d, 0.590490724821162d, 0.590619464332138d, 0.590734247229287d, 0.590942797783458d, 0.591197269523079d, 0.591424572972858d, 0.591610330188738d, 0.591767911449906d, 0.591947560797758d, 0.592140256954682d, 0.592319618096199d, 0.59253287121134d, 0.592839899321512d, 0.59310030703346d, 0.593365914431873d, 0.593620229858526d, 0.59393336735391d, 0.594134576401095d, 0.594354738706333d, 0.594488171939056d, 0.59474916294884d, 0.594933577740117d, 0.595239241749333d, 0.595627284903239d, 0.595858812551309d, 0.596062842327531d, 0.596189649992178d, 0.596396667517639d, 0.596556396541073d, 0.596805423207415d, 0.597135068642858d, 0.59726627036467d, 0.59758496342399d, 0.597822841267596d, 0.597942832624406d, 0.598223443242784d, 0.59858776827638d, 0.598965522151183d, 0.599117695837117d, 0.599332935758365d, 0.599632287030252d, 0.599838022397312d, 0.599980769458585d, 0.60016874038483d, 0.600559746526296d, 0.600788034958287d, 0.601139216387745d, 0.601357068551258d, 0.601589501262966d, 0.601877767318532d, 0.602297509085732d, 0.602485002742407d, 0.602769647431223d, 0.60300469914922d, 0.603245232518138d, 0.603458954242442d, 0.603607463116875d, 0.603702372583067d, 0.603927861227329d, 0.604040385955484d, 0.604241601691069d, 0.604641358394027d, 0.604923780758424d, 0.605260543509678d, 0.605513984865158d, 0.605707930232758d, 0.605987202151166d, 0.606241928103975d, 0.606463088760099d, 0.606656578552529d, 0.606882324022187d, 0.607110288381181d, 0.607360798022438d, 0.607553015747129d, 0.607911144961861d, 0.608276989174074d, 0.608531746488826d, 0.608850833284843d, 0.609263095400592d, 0.609540877312514d, 0.609856696072001d, 0.610033588546475d, 0.610344624392978d, 0.610671677849079d, 0.61120600758696d, 0.611473472326172d, 0.611659464958601d, 0.611926071085198d, 0.612218029258051d, 0.612546751966143d, 0.612895135808327d, 0.613261646998507d, 0.613591650822345d, 0.613750503449819d, 0.61397618595144d, 0.614108495790223d, 0.614447016464664d, 0.614813273474845d, 0.61511053553755d, 0.615474486492309d, 0.615825367459603d, 0.616064232704223d, 0.616248056424401d, 0.616446011196674d, 0.616784096209231d, 0.617050567539271d, 0.617216401493306d, 0.617433659201994d, 0.617701325098521d, 0.617951767303795d, 0.618174477827378d, 0.618368019591233d, 0.618751072769855d, 0.618877169549946d, 0.619257009087645d, 0.619491911935098d, 0.620057097442784d, 0.62052734652895d, 0.62074088879485d, 0.621072444476775d, 0.621244522798851d, 0.621585090734311d, 0.621885597060805d, 0.622235576474535d, 0.622599413110781d, 0.622783171332853d, 0.623198917004881d, 0.623530651957939d, 0.623876799771509d, 0.62423766822499d, 0.624460316720948d, 0.62475012075564d, 0.625066088138667d, 0.625391120586019d, 0.625669403180618d, 0.625947901509101d, 0.626317157255919d, 0.626723242074645d, 0.626855259800103d, 0.627140781414863d, 0.627380635151219d, 0.627713937097404d, 0.627993850248546d, 0.62854492463566d, 0.628880073462974d, 0.629131600346612d, 0.629442897462982d, 0.629899068231317d, 0.630327391678621d, 0.630814739904547d, 0.631184089900984d, 0.631286517179322d, 0.631559204680816d, 0.631770457769966d, 0.632051959480225d, 0.632497395508479d, 0.633105118812391d, 0.633458232650673d, 0.633730977445907d, 0.634027392549987d, 0.634322073637721d, 0.634722326211078d, 0.634966326793301d, 0.635748829240487d, 0.636138125011636d, 0.63664317989459d, 0.637047699853959d, 0.637408218136958d, 0.637666995507834d, 0.637955584535765d, 0.638302222244258d, 0.63863914572535d, 0.639094517295922d, 0.639860343795247d, 0.640320876875823d, 0.640923020425416d, 0.641713294163129d, 0.64215288686506d, 0.642465352247153d, 0.643373274786286d, 0.643653059251921d, 0.644662064541519d, 0.645084820812728d, 0.645350552050766d, 0.645767519790204d, 0.646088688702272d, 0.646431753118228d, 0.646898812941771d, 0.647286905775333d, 0.647759605759092d, 0.64788398003841d, 0.648165370495168d, 0.648422757604497d, 0.649015156781252d, 0.649896960201279d, 0.650181004778008d, 0.650900270181566d, 0.651521140670389d, 0.651875939668944d, 0.652092957223586d, 0.652535945734941d, 0.652822813297914d, 0.653221950652709d, 0.653521578206969d, 0.653871308550743d, 0.654405213201015d, 0.655130457752929d, 0.655928292352433d, 0.656307919338051d, 0.656818163426396d, 0.657360981602348d, 0.65807388846684d, 0.658413766006444d, 0.658832578153502d, 0.659122462621423d, 0.659780895191657d, 0.660084442565826d, 0.660863806814813d, 0.661509341669084d, 0.662284308978582d, 0.662617246297277d, 0.663389117772186d, 0.664020714522357d, 0.664232078822926d, 0.664765789329253d, 0.665309174987895d, 0.666192248009733d, 0.666777329887797d, 0.66741449282544d, 0.667979028287753d, 0.668702676560271d, 0.669696716794398d, 0.670542260341971d, 0.671031197588221d, 0.671784725328125d, 0.672147149964844d, 0.672392014983465d, 0.673821491280003d, 0.67453279398783d, 0.675117068680935d, 0.675830611574839d, 0.676423022010207d, 0.67705511631853d, 0.67792372839439d, 0.678994553820782d, 0.680064283694037d, 0.680702677819114d, 0.681310309662691d, 0.682519156906861d, 0.683082730444891d, 0.683610034980791d, 0.684745735212296d, 0.685527816322579d, 0.68619154655809d, 0.687197339436018d, 0.688578510313136d, 0.69060343339678d, 0.691513100906727d, 0.692702182719529d, 0.693624708417134d, 0.69492468221801d, 0.697141774702455d, 0.698561987726519d, 0.700343867811402d, 0.703941095218426d, 0.704774741884391d, 0.707473055928038d, 0.708826562305705d, 0.710892401124452d, 0.711841263882068d, 0.713325556480867d, 0.714786979151903d, 0.718587266587454d, 0.721241444284976d, 0.723139386411856d, 0.72527595733956d, 0.727999379526455d, 0.729270479700822d, 0.731773893795738d, 0.735201510266325d, 0.739013450350887d, 0.742731699797059d, 0.751569383539783d, 0.753897149706535d, 0.756415606399992d, 0.768650129037235d, 0.788101301838964d, 0.799691029181569d };

        [TestMethod()]
        public void Test_KernelDensity_CDF()
        {
            var KDE = new KernelDensity(sample);
            var graph = KDE.CreateCDFGraph();
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                // Debug.Print(graph[i, 0] + ", " + graph[i, 1]);
            }
        }

        [TestMethod()]
        public void Test_KernelDensity_PDF()
        {
            var KDE = new KernelDensity(sample);
            var graph = KDE.CreatePDFGraph();
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                // Debug.Print(graph[i, 0] + ", " + graph[i, 1]);
            }
        }

        [TestMethod()]
        public void Test_KernelDensity_PDF_Dates()
        {
            var data = new double[] { 10, 37, 58, 118, 119, 166, 324, 51, 56, 63, 105, 112, 113, 114, 131, 133, 134, 135, 138, 139, 142, 143, 148, 149, 157, 158, 160, 174, 226, 18, 133, 166, 168, 179, 183, 199, 203, 208, 311, 320, 339, 340, 352, 353, 17, 38, 51, 70, 71, 113, 114, 154, 159, 175, 176, 184, 202, 219, 220, 300, 40, 41, 48, 49, 88, 89, 99, 116, 129, 166, 183, 184, 185, 190, 191, 192, 193, 303, 320, 354, 46, 47, 48, 53, 54, 56, 70, 90, 91, 122, 129, 153, 39, 64, 66, 67, 68, 114, 115, 116, 117, 155, 163, 164, 165, 182, 187, 193, 194, 201, 204, 327, 365, 50, 51, 59, 61, 76, 79, 80, 81, 119, 120, 132, 136, 160, 185, 297, 298, 46, 81, 86, 87, 106, 111, 112, 113, 114, 116, 117, 118, 119, 121, 128, 129, 130, 142, 146, 147, 148, 149, 152, 161, 185, 189, 217, 218, 293, 300, 341, 38, 102, 108, 133, 151, 152, 153, 156, 203, 204, 209, 319, 320, 321, 330, 57, 60, 61, 75, 78, 80, 82, 83, 105, 164, 200, 220, 294, 16, 43, 83, 85, 86, 87, 88, 89, 201, 249, 265, 42, 45, 47, 48, 65, 68, 69, 101, 109, 111, 129, 134, 135, 151, 155, 157, 158, 162, 171, 172, 173, 305, 49, 62, 93, 96, 97, 98, 99, 100, 152, 153, 154, 159, 291, 308, 340, 28, 29, 33, 65, 89, 126, 132, 157, 159, 165, 188, 191, 59, 60, 128, 129, 130, 131, 132, 153, 155, 198, 199, 220, 300, 302, 346, 347, 91, 92, 95, 98, 147, 176, 220, 221, 236, 251, 321, 96, 97, 101, 102, 106, 107, 109, 111, 132, 133, 134, 136, 151, 152, 153, 155, 156, 162, 173, 182, 189, 197, 198, 42, 43, 52, 62, 77, 78, 97, 98, 107, 110, 117, 123, 124, 125, 137, 144, 152, 178, 179, 293, 20, 101, 102, 103, 106, 107, 110, 120, 121, 136, 137, 138, 139, 140, 141, 142, 143, 144, 155, 158, 223, 297, 300, 120, 121, 122, 132, 149, 152, 156, 171, 177, 319, 57, 94, 97, 112, 113, 133, 134, 163, 166, 168, 169, 180, 181, 183, 216, 299, 326, 329, 26, 41, 61, 69, 70, 83, 84, 115, 119, 121, 131, 149, 150, 152, 153, 154, 169, 172, 175, 176, 200, 203, 211, 212, 311, 319, 320, 322, 1, 5, 43, 51, 52, 56, 64, 86, 279, 287, 289, 292, 301, 315, 324, 355, 39, 77, 80, 81, 153, 178, 282, 292, 362, 10, 43, 56, 60, 63, 72, 122, 138, 142, 159, 160, 161, 167, 331, 360, 49, 67, 68, 97, 99, 120, 147, 157, 166, 173, 186, 309, 12, 23, 31, 36, 77, 78, 90, 91, 97, 109, 110, 125, 198, 203, 219, 333, 45, 82, 86, 96, 98, 127, 132, 154, 176, 25, 90, 94, 106, 109, 137, 140, 141, 156, 162, 289, 81, 95, 151, 165, 166, 170, 171, 176, 177, 178, 304, 27, 80, 89, 91, 97, 98, 133, 134, 135, 136, 138, 153, 173, 175, 281, 306, 318, 24, 68, 69, 73, 79, 89, 90, 94, 98, 99, 100, 105, 106, 107, 108, 109, 110, 122, 131, 132, 141, 142, 143, 146, 147, 248, 42, 76, 116, 127, 130, 131, 140, 143, 158, 170, 176, 208, 317, 5, 41, 57, 86, 95, 96, 97, 99, 100, 102, 103, 107, 118, 137, 156, 157, 162, 172, 174, 75, 84, 114, 123, 124, 143, 144, 149, 157, 165, 201, 30, 31, 52, 53, 71, 105, 118, 349, 360, 57, 67, 69, 82, 102, 157, 165, 176, 179, 180, 217, 225, 315, 335, 336, 118, 133, 186, 194, 281, 284, 134, 135, 139, 140, 143, 144, 146, 147, 157, 160, 201, 333, 336, 30, 103, 136, 150, 151, 161, 189, 203, 55, 56, 158, 341, 39, 40, 41, 78, 81, 90, 134, 135, 136, 148, 151, 166, 167, 196, 197, 215, 324, 329, 331, 332, 43, 86, 87, 126, 154, 155, 319, 17, 20, 89, 90, 92, 95, 96, 99, 100, 101, 102, 103, 122, 134, 135, 141, 142, 144, 170, 174, 210, 217, 218, 295, 309, 355, 17, 94, 107, 151, 159, 161, 162, 163, 172, 180, 197, 271, 339, 78, 146, 178, 61, 62, 68, 70, 97, 98, 99, 119, 120, 143, 76, 78, 79, 84, 132, 136, 139, 140, 173, 59, 60, 68, 73, 74, 83, 110, 111, 112, 113, 114, 115, 119, 129, 132, 133, 145, 150, 297, 1, 19, 20, 51, 52, 67, 79, 80, 81, 82, 83, 95, 139, 142, 143, 149, 172, 190, 222, 281, 304, 345, 5, 6, 52, 62, 78, 117, 151, 178, 195, 196, 197, 287, 323, 325, 326, 334, 74, 131, 143, 159, 222, 294, 333, 342, 29, 53, 150, 151, 152, 153, 208, 32, 63, 64, 65, 73, 74, 78, 92, 103, 146, 153, 220, 256, 291, 292, 350, 47, 75, 97, 98, 99, 123, 158, 163, 194, 23, 24, 52, 59, 77, 83, 85, 93, 101, 104, 105, 106, 111, 112, 141, 151, 174, 220, 297, 100, 101, 102, 162, 166, 173, 214, 285, 320, 13, 60, 61, 67, 68, 98, 106, 107, 108, 109, 110, 125, 126, 127, 128, 133, 137, 141, 144, 153, 157, 163, 228, 231, 65, 85, 87, 133, 154, 162, 164, 175, 189, 190, 192, 320, 326, 39, 117, 121, 162, 25, 70, 75, 84, 85, 139, 146, 156, 179, 204, 329, 34, 39, 40, 72, 89, 93, 97, 101, 102, 145, 152, 161, 165, 166, 172, 173, 180, 181, 324, 325, 114, 125, 144 };

            var KDE = new KernelDensity(data);
            var graph = KDE.CreatePDFGraph();
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                Debug.Print(graph[i, 0] + ", " + graph[i, 1]);
            }
        }

    }
}
