using Xunit;

namespace HeatOptimiser.Tests
{
    public class SourceDataManagerTest
    {
        [Fact]
        public void TestLoadXLSXFile()
        {
            // Arrange
            SourceDataManager sourceManager = new SourceDataManager();
            string projectDirectory =  Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string file = Path.Combine(projectDirectory, "SourceDataTest.xlsx");
            Console.WriteLine(file);

            // Act
            var result = sourceManager.LoadXLSXFile(file, 4, 2);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void TestGetDataInRange()
        {
            // Arrange
            SourceData data = new SourceData();
            SourceDataManager sourceManager = new SourceDataManager();
            DateTime startDate = new DateTime(2023, 1, 1);
            DateTime endDate = new DateTime(2023, 1, 31);

            // Act
            var result = sourceManager.GetDataInRange(data, startDate, endDate);

            // Assert
            Assert.NotNull(result);
        }
    }
}