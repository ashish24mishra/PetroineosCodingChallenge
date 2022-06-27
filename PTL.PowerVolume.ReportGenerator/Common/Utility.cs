using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace PTL.PowerVolume.ReportGenerator.Common
{
    public static class Utility 
    {
        /// <summary>
        /// Writes data on to file system
        /// </summary>
        /// <param name="positionData"></param>
        /// <param name="filePath"></param>
        public static void WriteToCsv(IEnumerable<object> positionData, string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (var writer = new StreamWriter(filePath))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecord(new { Time = "Local Time", Volume = "Volume" });
                csvWriter.NextRecord();
                csvWriter.WriteRecords(positionData);
            }
        }

        /// <summary>
        /// Sets expected file name with timestamp
        /// </summary>
        /// <param name="timeOfExtract"></param>
        /// <returns></returns>
        public static string GetFileName(DateTime timeOfExtract)
        {
            return $"PowerPosition_{timeOfExtract:yyyyMMdd_HHmm}.csv";
        }

        /// <summary>
        /// File name along with full path
        /// </summary>
        /// <param name="timeOfExtract"></param>
        /// <param name="reportLocation"></param>
        /// <returns></returns>
        public static string GetFullPath(DateTime timeOfExtract, string reportLocation)
        {
            return Path.Combine(reportLocation, GetFileName(timeOfExtract));
        }

        /// <summary>
        /// Converts given period data on to local time
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public static string ConvertToLocalTime(int period)
        {
            return DateTime.Today.AddHours(period - 2).ToString("HH:mm");
        }
    }
}
