using Xunit;

namespace HeatOptimiser.Tests
{
    public class SourceDataManagerTest
    {
        [Fact]
        public void TestLoadXLSXFile()
        {
            // Arrange
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string projectDirectory =  Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            string file = Path.Combine(projectDirectory, "SourceDataTest.xlsx");
            Console.WriteLine(file);

            // Act
            var result = SourceDataManager.LoadXLSXFile(file, 4, 2);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void TestGetDataInRange()
        {
            // Arrange
            SourceData data = new SourceData();
            data.LoadedData = new List<SourceDataPoint>(); // Initialize LoadedData
            DateTime startDate = new DateTime(2023, 1, 1);
            DateTime endDate = new DateTime(2023, 1, 31);

            // Act
            var result = SourceDataManager.GetDataInRange(data, startDate, endDate);

            // Assert
            Assert.NotNull(result);
        }
    }
}