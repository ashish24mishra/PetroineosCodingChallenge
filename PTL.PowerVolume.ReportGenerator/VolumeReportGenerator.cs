using PTL.PowerVolume.ReportGenerator.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTL.PowerVolume.ReportGenerator
{
    /// <summary>
    /// This class is reposible connects PowerService component, collects trade period data then converts into the expected volume file output report
    /// </summary>
    public class VolumeReportGenerator : Base, IPowerService
    {       
        public VolumeReportGenerator(IConfiguration config, IPowerService powerDataService)
        {
            Config   = config;
            PowerDataService = powerDataService;
        }

        /// <summary>
        /// Fetches the trade data and process it as expected output report
        /// </summary>
        /// <param name="runDateTime"></param>
        /// <returns></returns>
        public async Task<string> RunExtractAsync(DateTime runDateTime)
        {
            DateTime runDate = runDateTime.Date;
            
            var filePath = Utility.GetFullPath(runDateTime, Config.ReportLocation); //get filename

            Log.Info($"Running Extract for {runDateTime}");

            try
            {
                //get positions
                var trades = await GetTradesAsync(runDate);

                //aggregate by hour
                var aggregatedPositions = AggregatePositions(trades.ToList());

                //export file
                Utility.WriteToCsv(aggregatedPositions.Select(x => new { Time = Utility.ConvertToLocalTime(x.Key), Volume = x.Value }), filePath);

                Log.Info($"Report has been generated: {filePath}");
            }
            catch (Exception ex)
            {
                Log.Error($"Exception occured running extract for {runDateTime}.", ex);
            }
            return filePath;
        }        

        /// <summary>
        /// Aggregates the given trade period data
        /// </summary>
        /// <param name="trades"></param>
        /// <returns></returns>
        public Dictionary<int, double> AggregatePositions(List<PowerTrade> trades)
        {
            var positions = new Dictionary<int, double>();
            foreach (var trade in trades)
            {
                foreach (var period in trade.Periods)
                {
                    if (positions.ContainsKey(period.Period))
                        positions[period.Period] += period.Volume;
                    else
                        positions[period.Period] = period.Volume;
                }
            }
            return positions;
        }

        /// <summary>
        /// Asyncronously hits the PowerService to fetch trade data
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        {
            Log.Info($"Requesting power trade data for {date}");
            var powerTrades = await PowerDataService.GetTradesAsync(date);
            Log.Info($"Requesting power trade data for {date} received.");
            return powerTrades;
        }

        public IEnumerable<PowerTrade> GetTrades(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
