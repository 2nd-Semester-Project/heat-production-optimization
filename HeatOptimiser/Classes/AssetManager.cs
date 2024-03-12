using System.Text.Json;

namespace HeatOptimiser
{
    public class ProductionAsset
    {
        Guid ID { get; set; } = Guid.NewGuid();
        string? name { get; set; }
        string? _image { get; set; }
        double? Heat { get; set; }
        double? Electricity { get; set; }
        double? Energy { get; set; }
        double? Cost { get; set; }
        double? CarbonDioxide { get; set; }
    }
    public class AssetManager: IAssetManager
    {
        private List<ProductionAsset> _productionAssets = new List<ProductionAsset>();
        private JsonAssetStorage _jsonAssetStorage = new JsonAssetStorage();
        public AssetManager()
        {
            _productionAssets = LoadUnits("ProductionAssets.json");
        }
        public void AddUnit(string name, string imgae, double heat, double electricity, double energy, double cost, double carbonDioxide)
        {
            throw new System.NotImplementedException();
        }
        public void DeleteUnit(Guid ID)
        {
            throw new System.NotImplementedException();
        }
        public void EditUnit(Guid ID, int index, string value)
        {
            throw new System.NotImplementedException();
        }
        public List<ProductionAsset> GetAllUnits()
        {
            throw new System.NotImplementedException();
        }
        public List<ProductionAsset> LoadUnits(string fileName)
        {
            return _jsonAssetStorage.LoadUnits(fileName);
        }
        public void SaveUnits(List<ProductionAsset> AllAssets, string fileName)
        {
            _jsonAssetStorage.SaveUnits(AllAssets, fileName);
        }
        public List<ProductionAsset> SearchUnits()
        {
            throw new System.NotImplementedException();
        }
    }
    public class JsonAssetStorage: IAssetStorage
    {
        public List<ProductionAsset> LoadUnits(string fileName)
        {
            if (File.Exists(fileName) && new FileInfo(fileName).Length > 2)
            {
                string jsonString = File.ReadAllText(fileName);
                try
                {
                    var info = JsonSerializer.Deserialize<List<ProductionAsset>>(jsonString)!;
                    return info;
                }
                catch (JsonException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    return new List<ProductionAsset>();
                }
            }
            else
            {
                return new List<ProductionAsset>();
            }
        }
        public void SaveUnits(List<ProductionAsset> AllAssets, string fileName)
        {
            string jsonString = JsonSerializer.Serialize(AllAssets);
            File.WriteAllText(fileName, jsonString);
        }
    }
}