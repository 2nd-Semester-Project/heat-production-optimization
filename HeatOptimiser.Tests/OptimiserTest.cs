using Xunit;

namespace HeatOptimiser.Tests
{
    public class OptimiserTest
    {

        [Fact]
        public void TestOptimise()
        {
            // Arrange
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string projectDirectory =  Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            string file = Path.Combine(projectDirectory, "SourceDataTest.xlsx");
            var data = SourceDataManager.LoadXLSXFile(file, 4, 2);

            AssetManager.AddUnit("GB", "none", 5.0, 0, 1.1, 500, 215);
            AssetManager.AddUnit("OB", "none", 4.0, 0, 1.2, 700, 265);

            string startDateStr = "12/02/2023";
            string endDateStr = "25/02/2023";
            DateTime startDate = DateTime.ParseExact(startDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(endDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            // Act
            Schedule schedule = Optimiser.Optimise(startDate, endDate);

            // Assert
            Assert.NotNull(schedule);
            Assert.Equal(startDate, schedule.startDate);
            Assert.Equal(endDate, schedule.endDate);
        }
    }

}