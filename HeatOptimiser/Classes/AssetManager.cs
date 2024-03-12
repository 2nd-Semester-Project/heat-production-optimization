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
            _productionAssets = LoadUnits();
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
        public List<ProductionAsset> LoadUnits()
        {
            // Implement checking for valid data
            //_jsonAssetStorage.LoadUnits();
            throw new System.NotImplementedException();
        }
        public void SaveUnits(List<ProductionAsset> AllAssets)
        {
            // Implement check for null
            _jsonAssetStorage.SaveUnits(AllAssets, "ProductionAssets.json");
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
            throw new System.NotImplementedException();
        }
        public void SaveUnits(List<ProductionAsset> AllAssets, string fileName)
        {
            throw new System.NotImplementedException();
        }
    }
}