using log4net;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace PTL.PowerVolume.ReportGenerator
{
    public class TaskScheduler
    {
        private ILog _log = LogManager.GetLogger(nameof(VolumeReportGenerator));
        private Timer _timer;
        private Func<DateTime, Task> _taskToRun { get; }
        private int _intervalInMinutes { get; set; }

        public TaskScheduler(Func<DateTime, Task> taskToRun, int intervalInMinutes)
        {
            _taskToRun = taskToRun;
            _intervalInMinutes = intervalInMinutes;
        }

        public async Task RunScheduleTaskAsync()
        {
            _log.Info($"Scheduling timer for {_intervalInMinutes} minutes");
            _timer = new Timer
            {
                Interval = 1000 * _intervalInMinutes * 60
            };
            _timer.Elapsed += new ElapsedEventHandler(TriggerElapsedAsync);

            _timer.AutoReset = false;
            _timer.Start();

            await _taskToRun(DateTime.Now);
        }

        private async void TriggerElapsedAsync(object sender, ElapsedEventArgs e)
        {            
            await RunScheduleTaskAsync();
        }
    }
}
