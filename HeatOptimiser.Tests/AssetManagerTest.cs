using System.Collections.ObjectModel;
using Xunit;

namespace HeatOptimiser.Tests
{
    public class AssetManagerTest
    {
        [Fact]
        public void TestAssetManagerAdd()
        {
            // Act
            AssetManager.AddUnit("Asset Name", "Asset Image", 100.5, 200.0, 300.0, 400.0, 500.0);
            ObservableCollection<ProductionAsset> units = AssetManager.GetAllUnits();

            // Assert
            Assert.NotNull(units);
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
            //Act
            AssetManager.DeleteUnit(AssetManager.GetAllUnits()[0].ID);
            ObservableCollection<ProductionAsset> units = AssetManager.GetAllUnits();

            //Asert
            Assert.NotNull(units);
            Assert.Empty(units);
        }
        [Fact]
        public void TestAssetManagerEdit()
        {
            // Arrange
            AssetManager.AddUnit("Asset Name", "Asset Image", 100.5, 200.0, 300.0, 400.0, 500.0);

            // Act
            AssetManager.EditUnit(AssetManager.GetAllUnits()[0].ID, 0, "Unit 2");
            AssetManager.EditUnit(AssetManager.GetAllUnits()[0].ID, 1, "image2.jpg");
            AssetManager.EditUnit(AssetManager.GetAllUnits()[0].ID, 2, 11.5);
            AssetManager.EditUnit(AssetManager.GetAllUnits()[0].ID, 3, 21.5);
            AssetManager.EditUnit(AssetManager.GetAllUnits()[0].ID, 4, 31.5);
            AssetManager.EditUnit(AssetManager.GetAllUnits()[0].ID, 5, 41.5);
            AssetManager.EditUnit(AssetManager.GetAllUnits()[0].ID, 6, 51.5);
            var units = AssetManager.GetAllUnits();

            // Assert
            Assert.NotNull(units);
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
            AssetManager.AddUnit("Unit 1", "image1.jpg", 10.5, 20.5, 30.5, 40.5, 50.5);
            AssetManager.SaveUnits(AssetManager.GetAllUnits(), "TestUnits.json");

            // Act
            AssetManager.LoadUnits("TestUnits.json");
            var units = AssetManager.GetAllUnits();

            // Assert
            Assert.NotNull(units);
            Assert.Single(units);
            Assert.Equal("Unit 1", units[0].Name);
            Assert.Equal("image1.jpg", units[0].Image);
            Assert.Equal(10.5, units[0].Heat);
            Assert.Equal(20.5, units[0].Electricity);
            Assert.Equal(30.5, units[0].Energy);
            Assert.Equal(40.5, units[0].Cost);
            Assert.Equal(50.5, units[0].CarbonDioxide);

            AssetManager.DeleteUnit(AssetManager.GetAllUnits()[0].ID);
        }
        [Fact]
        public void TestAssetManagerSetSaveFile()
        {
            // Act
            AssetManager.SetSaveFile("TestUnits.json");
            var saveFileName = AssetManager.saveFileName;

            // Assert
            Assert.NotNull(saveFileName);
            Assert.Equal("TestUnits.json", saveFileName);
        }
    }
}