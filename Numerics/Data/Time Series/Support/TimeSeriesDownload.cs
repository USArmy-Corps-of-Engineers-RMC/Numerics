using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace Numerics.Data
{

    public class TimeSeriesDownload
    {
        /// <summary>
        /// Enumeration of USGS time series options.
        /// </summary>
        public enum USGSTimeSeriesType
        {
            DailyDischarge,
            DailyStage,
            InstantaneousDischarge,
            InstantaneousStage,
            PeakDischarge,
            PeakStage
        }

        /// <summary>
        /// Enumeration of GHCN time series options.
        /// </summary>
        public enum GHCNTimeSeriesType
        {
            DailyPrecipitation,
            DailySnow,
        }

        /// <summary>
        /// Enumeration of GHCN depth unit options.
        /// </summary>
        public enum DepthUnit
        {
            Millimeters,
            Centimeters,
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
                        var segments = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                        if (segments.Count() >= 4 && segments.First() == "USGS")
                        {
                            // Get date
                            DateTime index = DateTime.Now;
                            var dateString = segments[2].Split('-');
                            if (dateString[1] == "00")
                            {
                                int year = 2000;
                                int.TryParse(dateString[0], out year);
                                index = new DateTime(year, 1, 1, 0, 0, 0);
                            }
                            else
                            {
                                DateTime.TryParse(segments[2], out index);
                            }


                            // Get value
                            double value = 0;

                            // see if the 4th column has a time
                            int offset = 0;
                            var timeString = segments[3].Split(':');
                            if (timeString.Length == 2)
                                offset = 1;

                            int idx = timeSeriesType == USGSTimeSeriesType.PeakDischarge ? 3 + offset : 4 + offset;
                            if (segments[idx] == "" || segments[idx] == " " || segments[idx] == "  " || string.IsNullOrEmpty(segments[idx]))
                                value = double.NaN;
                            else
                            {
                                double.TryParse(segments[idx], out value);
                            }
                            timeSeries.Add(new SeriesOrdinate<DateTime, double>(index, value));
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
