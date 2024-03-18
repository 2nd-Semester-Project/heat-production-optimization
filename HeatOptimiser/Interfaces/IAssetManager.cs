namespace HeatOptimiser
{
    public interface IAssetManager
    {
        public void AddUnit(string name, string image, double heat, double electricity, double energy, double cost, double carbonDioxide);
        public void EditUnit(Guid ID, int index, string stringValue);
        public void EditUnit(Guid ID, int index, double doubleValue);
        public void DeleteUnit(Guid ID);   
        public void SaveUnits(List<ProductionAsset> AllAssets, string fileName);
        List<ProductionAsset> LoadUnits(string fileName);
        List<ProductionAsset> GetAllUnits();
        List<ProductionAsset> SearchUnits(string name);
        public void SetSaveFile(string fileName);
        public void AddUnit();
        public void EditUnit();
        public void DeleteUnit();
        public void SaveUnits();
    }
    public interface IAssetStorage
    {
        void SaveUnits(List<ProductionAsset> AllAssets, string fileName);
        List<ProductionAsset> LoadUnits(string fileName);
    }
}