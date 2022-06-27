using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PTL.PowerVolume.ReportGenerator.Common;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PTL.PowerVolume.ReportGenerator.Tests
{
    [TestClass()]
    public class ReportGeneratorTests
    {
        private IConfiguration mockConfig;
        private IPowerService mockPowerService;

        [TestInitialize()]
        public void Startup()
        {
            mockConfig = new MockConfig();
            Directory.CreateDirectory(mockConfig.ReportLocation);

            mockPowerService = new Mock<IPowerService>().Object;
        }

        [TestMethod()]
        public void GetFileNameTest()
        {
            var reportGenerator = new VolumeReportGenerator(mockConfig, mockPowerService);
            var timeOfExtract = DateTime.Parse("15/Jul/2021 21:20");
            var expectedFileName = "PowerPosition_20210715_2120.csv";

            var actualFilename = Utility.GetFileName(timeOfExtract);

            Assert.AreEqual(expectedFileName, actualFilename);
        }

        [TestMethod()]
        public void GetFullPathTest()
        {
            var timeOfExtract = DateTime.Parse("15/Jul/2021 21:20");
            var expectedFileName = "PowerPosition_20210715_2120.csv";
            var expectedFullPath = Path.Combine(mockConfig.ReportLocation, expectedFileName);

            var reportGenerator = new VolumeReportGenerator(mockConfig, mockPowerService);
            var actualFullPath = Utility.GetFullPath(timeOfExtract, mockConfig.ReportLocation);

            Assert.AreEqual(expectedFullPath, actualFullPath);
        }

        [TestMethod()]
        public void AggregatePositionsTest()
        {
            PowerTrade pt1 = PowerTrade.Create(DateTime.Parse("15/07/2021"), 2);
            pt1.Periods[0].Volume = 100;
            pt1.Periods[1].Volume = 200;

            PowerTrade pt2 = PowerTrade.Create(DateTime.Parse("15/07/2021"), 4);
            pt2.Periods[0].Volume = 50;
            pt2.Periods[1].Volume = 150;
            pt2.Periods[2].Volume = 250;
            pt2.Periods[3].Volume = 350;

            var expectedPosition = new Dictionary<int, double>();
            expectedPosition[1] = 150;
            expectedPosition[2] = 350;
            expectedPosition[3] = 250;
            expectedPosition[4] = 350;

            var reportGenerator = new VolumeReportGenerator(mockConfig, mockPowerService);
            var actualPosition = reportGenerator.AggregatePositions(new List<PowerTrade> { pt1, pt2 });

            foreach (var position in expectedPosition)
            {
                Assert.AreEqual(position.Value, actualPosition[position.Key]);
            }
        }

        [TestMethod()]
        public void ConvertToLocalTimeTest()
        {
            var expectedTime1 = "23:00";
            var expectedTime2 = "00:00";
            var expectedTime14 = "12:00";
            var expectedTime24 = "22:00";

            Assert.AreEqual(expectedTime1, Utility.ConvertToLocalTime(1));
            Assert.AreEqual(expectedTime2, Utility.ConvertToLocalTime(2));
            Assert.AreEqual(expectedTime14, Utility.ConvertToLocalTime(14));
            Assert.AreEqual(expectedTime24, Utility.ConvertToLocalTime(24));
        }

        [TestMethod()]
        public async Task GetTradesAsyncIsCalledTestAsync()
        {
            Mock<IPowerService> mock = new Mock<IPowerService>();
            var reportGenerator = new VolumeReportGenerator(mockConfig, mock.Object);
            _ = await reportGenerator.GetTradesAsync(DateTime.Now);

            mock.Verify(m => m.GetTradesAsync(It.IsAny<DateTime>()), Times.Once);
        }

        [TestMethod()]
        public async Task RunExtractAsyncTestAsync()
        {
            var runDateTime = DateTime.Parse("26/Jun/2022 21:20");

            PowerTrade pt1 = PowerTrade.Create(runDateTime, 2);
            pt1.Periods[0].Volume = 100;
            pt1.Periods[1].Volume = 200;

            PowerTrade pt2 = PowerTrade.Create(runDateTime, 4);
            pt2.Periods[0].Volume = 50;
            pt2.Periods[1].Volume = 150;
            pt2.Periods[2].Volume = 250;
            pt2.Periods[3].Volume = 350;

            var powerTrades = Task.FromResult((IEnumerable<PowerTrade>)new List<PowerTrade> { pt1, pt2 });

            Mock<IPowerService> mock = new Mock<IPowerService>();
            mock.Setup(m => m.GetTradesAsync(It.IsAny<DateTime>())).Returns(powerTrades);
            var reportGenerator = new VolumeReportGenerator(mockConfig, mock.Object);
            var fileName = await reportGenerator.RunExtractAsync(runDateTime);

            //preparing expected data
            var expectedPosition = new Dictionary<int, double>();
            expectedPosition[1] = 150;
            expectedPosition[2] = 350;
            expectedPosition[3] = 250;
            expectedPosition[4] = 350;

            var expectedMockFileName = $"{mockConfig.ReportLocation}/MockTestOutput/{Path.GetFileName(fileName)}";
            Utility.WriteToCsv(expectedPosition.Select(x => new { Time = Utility.ConvertToLocalTime(x.Key), Volume = x.Value }), expectedMockFileName);

            var expectedResponseText = File.ReadAllText(expectedMockFileName);

            var actualResponseText = File.ReadAllText(fileName);
                        
            Assert.AreEqual(expectedResponseText, actualResponseText);  //Assert
        }
    }
}