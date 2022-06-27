using System.Configuration;

namespace PTL.PowerVolume.ReportGenerator
{
    /// <summary>
    /// This file is reposible getting configuration settings
    /// </summary>
    public class Configuration : IConfiguration
    {
        public string ReportLocation => ConfigurationManager.AppSettings["ReportFileLocation"];
        public int ServiceInterval => int.Parse(ConfigurationManager.AppSettings["TimeInterval"]);
    }
}
