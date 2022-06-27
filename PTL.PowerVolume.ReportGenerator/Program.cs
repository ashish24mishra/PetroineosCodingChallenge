using Services;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace PTL.PowerVolume.ReportGenerator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static async Task Main(string[] args)
        {
            var reportGeneratorService = new ReportGeneratorService();
            if ((!Environment.UserInteractive))
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    reportGeneratorService
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                await reportGeneratorService.RunAsConsoleAsync(args);
                while (true)
                {
                    Thread.Sleep(30000);
                }
            }
        }

        private static async Task RunAsConsole()
        {
            IConfiguration config = new Configuration();
            IPowerService powerService = new PowerService();

            VolumeReportGenerator volumeReportGenerator = new VolumeReportGenerator(config, powerService);
            //await reportGenerator.RunExtractAsync(); //in case to run just once (without scheduling)

            var timeIntervalInMinutes = ConfigurationManager.AppSettings["TimeInterval"];
            var intervalInMinutes = int.Parse(timeIntervalInMinutes);

            TaskScheduler scheduler = new TaskScheduler(volumeReportGenerator.RunExtractAsync, intervalInMinutes);
            await scheduler.RunScheduleTaskAsync();

        }
    }
}
