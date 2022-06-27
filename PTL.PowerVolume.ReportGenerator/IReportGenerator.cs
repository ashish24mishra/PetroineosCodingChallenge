using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PTL.PowerVolume.ReportGenerator
{
    public interface IReportGenerator
    {
        Dictionary<int, double> AggregatePositions(List<PowerTrade> trades);
     
        Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date);

        Task RunExtractAsync(DateTime runDateTime);

        void WriteToCsv(IEnumerable<object> positionData, string fileFullPath);
    }
}
