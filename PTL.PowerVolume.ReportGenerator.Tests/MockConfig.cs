
using System.Configuration;

namespace PTL.PowerVolume.ReportGenerator.Tests
{
    public class MockConfig : IConfiguration
    {
        public string ReportLocation => ConfigurationManager.AppSettings["ReportFileLocation"];
        public int ServiceInterval => int.Parse(ConfigurationManager.AppSettings["TimeInterval"]);
    }
}
