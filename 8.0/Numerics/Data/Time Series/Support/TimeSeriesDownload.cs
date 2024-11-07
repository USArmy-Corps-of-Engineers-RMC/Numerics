/*
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

using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System;

namespace Numerics.Data
{

    /// <summary>
    /// Download time series data from the Internet.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class TimeSeriesDownload
    {
        /// <summary>
        /// Enumeration of USGS time series options.
        /// </summary>
        public enum USGSTimeSeriesType
        {
            /// <summary>
            /// Daily discharge.
            /// </summary>
            DailyDischarge,
            /// <summary>
            /// Daily stage.
            /// </summary>
            DailyStage,
            /// <summary>
            /// Instantaneous discharge, typically record at a 15-minute interval.
            /// </summary>
            InstantaneousDischarge,
            /// <summary>
            /// Instantaneous stage, typically record at a 15-minute interval.
            /// </summary>
            InstantaneousStage,
            /// <summary>
            /// Annual max peak discharge.
            /// </summary>
            PeakDischarge,
            /// <summary>
            /// Annual max peak stage.
            /// </summary>
            PeakStage
        }

        /// <summary>
        /// Enumeration of GHCN time series options.
        /// </summary>
        public enum GHCNTimeSeriesType
        {
            /// <summary>
            /// Daily precipitation.
            /// </summary>
            DailyPrecipitation,
            /// <summary>
            /// Daily snow.
            /// </summary>
            DailySnow,
        }

        /// <summary>
        /// Enumeration of GHCN depth unit options.
        /// </summary>
        public enum DepthUnit
        {
            /// <summary>
            /// Millimeters
            /// </summary>
            Millimeters,
            /// <summary>
            /// Centimeters.
            /// </summary>
            Centimeters,
            /// <summary>
            /// Inches.
            /// </summary>
            Inches
        }

        /// <summary>
        /// Download data from the Global Historical Climatology Network (GHCN). 
        /// </summary>
        /// <param name="siteNumber">The station identification code.</param>
        /// <param name="timeSeriesType">The time series type. Default = Daily precipitation.</param>
        /// <param name="unit">The depth unit. Default = inches.</param>
        /// <returns>A downloaded time series.</returns>
        public static TimeSeries FromGHCN(string siteNumber, GHCNTimeSeriesType timeSeriesType = GHCNTimeSeriesType.DailyPrecipitation, DepthUnit unit = DepthUnit.Inches)
        {     
            var timeSeries = new TimeSeries(TimeInterval.OneDay);
      
            try
            {
                // Check if there is an Internet connection
                if (IsConnectedToInternet() == false)
                    throw new Exception("There is no Internet connection!");

                // Setup url parameters
                string url = "https://ncei.noaa.gov/pub/data/ghcn/daily/by_station/" + siteNumber + ".csv.gz";

                // Temp file names
                string zipFileName = "C:/Temp/" + siteNumber + ".csv.gz";

                // Download data to temp directory
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, zipFileName);
                }

                // Unzip the file
                FileInfo fileToDecompress = new FileInfo(zipFileName);
                using (FileStream originalFileStream = fileToDecompress.OpenRead())
                {
                    string currentFileName = fileToDecompress.FullName;
                    string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);
                    using (FileStream decompressedFileStream = File.Create(newFileName))
                    {
                        using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(decompressedFileStream);
                        }
                    }
                }

                // Parse the file and create time series
                // import at-site data
                string filePath = "C:/Temp/" + siteNumber + ".csv";
                StreamReader reader = null;
                if (File.Exists(filePath))
                {               
                    reader = new StreamReader(File.OpenRead(filePath));
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine().Split(',');

                        // Get date
                        var dateString = line[1];
                        int.TryParse(dateString.Substring(0, 4), out var year);
                        int.TryParse(dateString.Substring(4, 2), out var month);
                        int.TryParse(dateString.Substring(6, 2), out var day);
                        var dateTime = new DateTime(year, month, day);

                        // Get type
                        var typeString = line[2];

                        // Get value
                        double.TryParse(line[3], out var value);
                        if ((typeString == "PRCP" && timeSeriesType == GHCNTimeSeriesType.DailyPrecipitation) || (typeString == "SNOW" && timeSeriesType == GHCNTimeSeriesType.DailySnow))
                        {
                            value /= 10d;
                            if (unit == DepthUnit.Millimeters)
                            {
                                timeSeries.Add(new SeriesOrdinate<DateTime, double>(dateTime, value));
                            }
                            else if (unit == DepthUnit.Centimeters)
                            {
                                timeSeries.Add(new SeriesOrdinate<DateTime, double>(dateTime, value / 10d));
                            }
                            else if (unit == DepthUnit.Inches)
                            {
                                timeSeries.Add(new SeriesOrdinate<DateTime, double>(dateTime, value / 25.4d));
                            }
                        }
                    }
                    reader.Close();
                }
                else
                {
                    throw new Exception("File doesn't exist");
                }

                // Delete the zip file and .csv file
                File.Delete(zipFileName);
                File.Delete(filePath);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return timeSeries;
        }

        /// <summary>
        /// Download time series data from USGS
        /// </summary>
        /// <param name="siteNumber">USGS site number.</param>
        /// <param name="timeSeriesType">The time series type.</param>
        public static TimeSeries FromUSGS(string siteNumber, USGSTimeSeriesType timeSeriesType = USGSTimeSeriesType.DailyDischarge)
        {
            var timeSeries = new TimeSeries();

            try
            {
                // Check if there is an Internet connection
                if (IsConnectedToInternet() == false)
                    throw new Exception("There is no Internet connection!");

                // Setup url parameters
                string timeInterval = "dv";
                string startDate = "1800-01-01";
                string endDate = DateTime.Now.ToString("yyyy-MM-dd");
                string statCode = "&statCd=00003";
                string parameterCode = "00060";
                string url = "";

                if (timeSeriesType == USGSTimeSeriesType.DailyDischarge || timeSeriesType == USGSTimeSeriesType.DailyStage)
                {
                    timeInterval = "dv";
                    statCode = "&statCd=00003";
                    parameterCode = timeSeriesType == USGSTimeSeriesType.DailyDischarge ? "00060" : "00065";
                    timeSeries = new TimeSeries(TimeInterval.OneDay);
                    url = "https://waterservices.usgs.gov/nwis/" + timeInterval + "/?format=waterml,2.0&sites=" + siteNumber + "&startDT=" + startDate + "&endDT=" + endDate + statCode + "&parameterCd=" + parameterCode + "&siteStatus=all";
                }
                else if (timeSeriesType == USGSTimeSeriesType.InstantaneousDischarge || timeSeriesType == USGSTimeSeriesType.InstantaneousStage)
                {
                    timeInterval = "iv";
                    statCode = "";
                    parameterCode = timeSeriesType == USGSTimeSeriesType.InstantaneousDischarge ? "00060" : "00065";
                    timeSeries = new TimeSeries(TimeInterval.FifteenMinute);
                    url = "https://waterservices.usgs.gov/nwis/" + timeInterval + "/?format=waterml,2.0&sites=" + siteNumber + "&startDT=" + startDate + "&endDT=" + endDate + statCode + "&parameterCd=" + parameterCode + "&siteStatus=all";
                }
                else if (timeSeriesType == USGSTimeSeriesType.PeakDischarge || timeSeriesType == USGSTimeSeriesType.PeakStage)
                {
                    timeInterval = "peak";
                    statCode = "";
                    timeSeries = new TimeSeries(TimeInterval.Irregular);
                    url = "https://nwis.waterdata.usgs.gov/nwis/peak?site_no=" + siteNumber + "&agency_cd=USGS&format=rdb";
                }

                // Download data
                string textDownload;
                using (var client = new WebClient())
                    textDownload = client.DownloadString(url);

                if (timeSeriesType == USGSTimeSeriesType.DailyDischarge || timeSeriesType == USGSTimeSeriesType.DailyStage)
                {
                    // Convert to XElement and check if the download was valid
                    var xElement = XElement.Parse(textDownload);
                    var points = xElement.Descendants("{http://www.opengis.net/waterml/2.0}point");

                    // Create time series
                    foreach (XElement point in points.Elements())
                    {
                        if (point.Element("{http://www.opengis.net/waterml/2.0}time") != null && point.Element("{http://www.opengis.net/waterml/2.0}value") != null)
                        {
                            // Get date
                            DateTime index = DateTime.Now;
                            DateTime.TryParse(point.Element("{http://www.opengis.net/waterml/2.0}time").Value, out index);

                            // See if this date is 
                            if (timeSeries.Count > 0 && index != TimeSeries.AddTimeInterval(timeSeries.Last().Index, TimeInterval.OneDay))
                            {
                                while (timeSeries.Last().Index < TimeSeries.SubtractTimeInterval(index, TimeInterval.OneDay))
                                    timeSeries.Add(new SeriesOrdinate<DateTime, double>(TimeSeries.AddTimeInterval(timeSeries.Last().Index, TimeInterval.OneDay), double.NaN));
                            }

                            // Get value
                            double value = 0;
                            string valueStg = point.Element("{http://www.opengis.net/waterml/2.0}value").Value;
                            if (valueStg == "" || valueStg == " " || valueStg == "  " || string.IsNullOrEmpty(valueStg))
                                value = double.NaN;
                            else
                            {
                                double.TryParse(valueStg, out value);
                            }
                            timeSeries.Add(new SeriesOrdinate<DateTime, double>(index, value));
                        }
                    }
                }
                else if (timeSeriesType == USGSTimeSeriesType.PeakDischarge || timeSeriesType == USGSTimeSeriesType.PeakStage)
                {
                    var delimiters = new char[] { '\t' };
                    var lines = textDownload.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines)
                    {
                        var segments = line.Split(delimiters);
                        if (segments.First() == "USGS" && segments.Count() >= 5)
                        {
                            // Get date
                            DateTime index = DateTime.Now;
                            int year = 2000;
                            int month = 1;
                            int day = 1;
                            var dateString = segments[2].Split('-');
                            DateTime.TryParse(segments[2], out index);

                            if (index == DateTime.MinValue)
                            {
                                // The date parsing failed, so try to manually parse it
                                if (dateString[1] == "00" && dateString[2] != "00")
                                {
                                    int.TryParse(dateString[0], out year);
                                    int.TryParse(dateString[2], out day);
                                    index = new DateTime(year, month, day, 0, 0, 0);
                                }
                                else if (dateString[1] != "00" && dateString[2] == "00")
                                {
                                    int.TryParse(dateString[0], out year);
                                    int.TryParse(dateString[1], out month);
                                    index = new DateTime(year, month, day, 0, 0, 0);
                                }
                                else if (dateString[1] == "00" && dateString[2] == "00")
                                {
                                    int.TryParse(dateString[0], out year);
                                    index = new DateTime(year, month, day, 0, 0, 0);
                                }
                            }
                            // Get value
                            double value = 0;
                            int idx = timeSeriesType == USGSTimeSeriesType.PeakDischarge ? 4 : 6;
                            if (segments[idx] != "" && segments[idx] != " " && segments[idx] != "  " && !string.IsNullOrEmpty(segments[idx]))
                            {
                                double.TryParse(segments[idx], out value);
                                timeSeries.Add(new SeriesOrdinate<DateTime, double>(index, value));
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return timeSeries;
        }

        /// <summary>
        /// Checks if there is an Internet connection.
        /// </summary>
        public static bool IsConnectedToInternet()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch { }
            return false;
        }
    }
}
