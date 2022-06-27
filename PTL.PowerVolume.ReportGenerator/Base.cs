using log4net;
using Services;

namespace PTL.PowerVolume.ReportGenerator
{
    public class Base 
    {
        public ILog Log = LogManager.GetLogger(nameof(VolumeReportGenerator));        
        public IConfiguration Config;
        public IPowerService PowerDataService;

    }
}
