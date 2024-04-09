using Xunit;

namespace HeatOptimiser.Tests
{
    public class ResultsDataManagerTest
    {
        [Fact]
        public void TestResultsManagerSave() {
            // Arrange
            AssetManager assetManager = new AssetManager();
            assetManager.AddUnit("GB", "none", 5.0, 0, 1.1, 500, 215);
            assetManager.AddUnit("OB", "none", 4.0, 0, 1.2, 700, 265);

            SourceDataManager sourceManager = new SourceDataManager();
            string projectDirectory =  Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string file = Path.Combine(projectDirectory, "SourceDataTest.xlsx");
            var data = sourceManager.LoadXLSXFile(file, 4, 2);

            Optimiser optimiser = new Optimiser(sourceManager, assetManager);
            string startDateStr = "12/02/2023";
            string endDateStr = "25/02/2023";
            DateTime startDate = DateTime.ParseExact(startDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(endDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            Schedule schedule = optimiser.Optimise(startDate, endDate);

            string resFile = Path.Combine(projectDirectory, "resultdata.csv");
            ResultsDataManager resultsManager = new(resFile, assetManager);

            // Act
            resultsManager.Save(schedule);

            // Assert
            Assert.True(File.Exists(resFile));
        }
        [Fact]
        public void TestResultsManagerRemove() {
            // Arrange
            AssetManager assetManager = new AssetManager();
            assetManager.AddUnit("GB", "none", 5.0, 0, 1.1, 500, 215);
            assetManager.AddUnit("OB", "none", 4.0, 0, 1.2, 700, 265);

            SourceDataManager sourceManager = new SourceDataManager();
            string projectDirectory =  Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string file = Path.Combine(projectDirectory, "SourceDataTest.xlsx");
            var data = sourceManager.LoadXLSXFile(file, 4, 2);

            Optimiser optimiser = new Optimiser(sourceManager, assetManager);
            string startDateStr = "12/02/2023";
            string endDateStr = "25/02/2023";
            DateTime startDate = DateTime.ParseExact(startDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(endDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            Schedule schedule = optimiser.Optimise(startDate, endDate);

            string resFile = Path.Combine(projectDirectory, "resultdata.csv");
            ResultsDataManager resultsManager = new(resFile, assetManager);

            // Act
            resultsManager.Save(schedule);
            resultsManager.Remove(DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate));

            // Assert
            Assert.Empty(File.ReadAllLines(resFile));
        }
        [Fact]
        public void TestResultsManagerLoad() {
            // Arrange
            AssetManager assetManager = new AssetManager();
            assetManager.AddUnit("GB", "none", 5.0, 0, 1.1, 500, 215);
            assetManager.AddUnit("OB", "none", 4.0, 0, 1.2, 700, 265);

            SourceDataManager sourceManager = new SourceDataManager();
            string projectDirectory =  Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string file = Path.Combine(projectDirectory, "SourceDataTest.xlsx");
            var data = sourceManager.LoadXLSXFile(file, 4, 2);

            Optimiser optimiser = new Optimiser(sourceManager, assetManager);
            string startDateStr = "12/02/2023";
            string endDateStr = "25/02/2023";
            DateTime startDate = DateTime.ParseExact(startDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(endDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            Schedule schedule = optimiser.Optimise(startDate, endDate);

            string resFile = Path.Combine(projectDirectory, "resultdata.csv");
            ResultsDataManager resultsManager = new(resFile, assetManager);

            // Act
            resultsManager.Save(schedule);
            Schedule loadedSchedule = resultsManager.Load(DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate));

            // Assert
            Assert.NotNull(loadedSchedule);
            Assert.Equal(startDate, loadedSchedule.startDate);
            Assert.Equal(endDate, loadedSchedule.endDate);
        }
    }
}