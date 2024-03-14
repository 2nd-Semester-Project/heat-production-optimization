using Xunit;

namespace HeatOptimiser.Tests
{
    public class AssetManagerTest
    {
        [Fact]
        public void TestAssetManagerAdd()
        {
            // Arrange
            AssetManager assetManager = new AssetManager();

            // Act
            assetManager.AddUnit("Asset Name", "Asset Image", 100.5, 200.0, 300.0, 400.0, 500.0);
            List<ProductionAsset> units = assetManager.GetAllUnits();

            // Assert
            Assert.NotNull(assetManager);
            Assert.Single(units); // Check if there is only one item in the list (the one we added
            Assert.Equal("Asset Name", units[0].Name);
            Assert.Equal("Asset Image", units[0].Image);
            Assert.Equal(100.5, units[0].Heat);
            Assert.Equal(200.0, units[0].Electricity);
            Assert.Equal(300.0, units[0].Energy);
            Assert.Equal(400.0, units[0].Cost);
            Assert.Equal(500.0, units[0].CarbonDioxide);
        }
        [Fact]
        public void AssetManagerDelete()
        {
            // Arrange
            var assetManager = new AssetManager();
            assetManager.AddUnit("Unit 1", "image1.jpg", 10.5, 20.5, 30.5, 40.5, 50.5);

            //Act
            assetManager.DeleteUnit(assetManager.GetAllUnits()[0].ID);
            var units = assetManager.GetAllUnits();

            //Asert
            Assert.NotNull(assetManager);
            Assert.Empty(units);
        }
        [Fact]
        public void TestAssetManagerEdit()
        {
            // Arrange
            var assetManager = new AssetManager();
            assetManager.AddUnit("Unit 1", "image1.jpg", 10.5, 20.5, 30.5, 40.5, 50.5);

            // Act
            assetManager.EditUnit(assetManager.GetAllUnits()[0].ID, 0, "Unit 2");
            assetManager.EditUnit(assetManager.GetAllUnits()[0].ID, 1, "image2.jpg");
            assetManager.EditUnit(assetManager.GetAllUnits()[0].ID, 2, 11.5);
            assetManager.EditUnit(assetManager.GetAllUnits()[0].ID, 3, 21.5);
            assetManager.EditUnit(assetManager.GetAllUnits()[0].ID, 4, 31.5);
            assetManager.EditUnit(assetManager.GetAllUnits()[0].ID, 5, 41.5);
            assetManager.EditUnit(assetManager.GetAllUnits()[0].ID, 6, 51.5);
            var units = assetManager.GetAllUnits();

            // Assert
            Assert.NotNull(assetManager);
            Assert.Single(units);
            Assert.Equal("Unit 2", units[0].Name);
            Assert.Equal("image2.jpg", units[0].Image);
            Assert.Equal(11.5, units[0].Heat);
            Assert.Equal(21.5, units[0].Electricity);
            Assert.Equal(31.5, units[0].Energy);
            Assert.Equal(41.5, units[0].Cost);
            Assert.Equal(51.5, units[0].CarbonDioxide);
        }
        [Fact]
        public void TestAssetManagerLoadAndSaveUnits()
        {
            // Arrange
            var assetManager = new AssetManager();
            assetManager.AddUnit("Unit 1", "image1.jpg", 10.5, 20.5, 30.5, 40.5, 50.5);
            assetManager.SaveUnits(assetManager.GetAllUnits(), "TestUnits.json");

            // Act
            assetManager.LoadUnits("TestUnits.json");
            var units = assetManager.GetAllUnits();

            // Assert
            Assert.NotNull(assetManager);
            Assert.Single(units);
            Assert.Equal("Unit 1", units[0].Name);
            Assert.Equal("image1.jpg", units[0].Image);
            Assert.Equal(10.5, units[0].Heat);
            Assert.Equal(20.5, units[0].Electricity);
            Assert.Equal(30.5, units[0].Energy);
            Assert.Equal(40.5, units[0].Cost);
            Assert.Equal(50.5, units[0].CarbonDioxide);
        }
        [Fact]
        public void TestAssetManagerSetSaveFile()
        {
            // Arrange
            var assetManager = new AssetManager();

            // Act
            assetManager.SetSaveFile("TestUnits.json");
            var saveFileName = assetManager.saveFileName;

            // Assert
            Assert.NotNull(assetManager);
            Assert.Equal("TestUnits.json", saveFileName);
        }
    }
}