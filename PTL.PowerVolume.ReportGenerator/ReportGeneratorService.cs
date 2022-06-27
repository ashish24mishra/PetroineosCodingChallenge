using Services;
using System.Configuration;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace PTL.PowerVolume.ReportGenerator
{
    public partial class ReportGeneratorService : ServiceBase
    { 
        private static readonly int _interval = int.Parse(ConfigurationManager.AppSettings["TimeInterval"]);

        public ReportGeneratorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Task.Run(() => StartReportGenerator());
        }

        private static async Task StartReportGenerator()
        {
            var config = new Configuration();
            var powerService = new PowerService();

            var volumeReportGenerator = new VolumeReportGenerator(config, powerService);

            TaskScheduler scheduler = new TaskScheduler(volumeReportGenerator.RunExtractAsync, _interval);
            await scheduler.RunScheduleTaskAsync();
        }

        public async Task RunAsConsoleAsync(string[] args)
        {
            await StartReportGenerator();
        }

        protected override void OnStop()
        {
        }
    }
}
