using Xunit;

namespace HeatOptimiser.Tests
{
    public class OptimiserTest
    {

        [Fact]
        public void TestOptimise()
        {
            // Arrange
            SourceDataManager sourceManager = new SourceDataManager();
            string projectDirectory =  Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string file = Path.Combine(projectDirectory, "SourceDataTest.xlsx");
            var data = sourceManager.LoadXLSXFile(file, 4, 2);

            IAssetManager assetManager = new AssetManager();
            assetManager.AddUnit("GB", "none", 5.0, 0, 1.1, 500, 215);
            assetManager.AddUnit("OB", "none", 4.0, 0, 1.2, 700, 265);

            Optimiser optimiser = new Optimiser(sourceManager, assetManager);
            string startDateStr = "12/02/2023";
            string endDateStr = "25/02/2023";
            DateTime startDate = DateTime.ParseExact(startDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(endDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            // Act
            Schedule schedule = optimiser.Optimise(startDate, endDate);

            // Assert
            Assert.NotNull(schedule);
            Assert.Equal(startDate, schedule.startDate);
            Assert.Equal(endDate, schedule.endDate);
        }
    }

}