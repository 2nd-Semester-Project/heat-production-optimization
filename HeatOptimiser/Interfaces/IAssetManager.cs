namespace HeatOptimiser
{
    public interface IAssetManager
    {
        void AddUnit(string name, string imgae, double heat, double electricity, double energy, double cost, double carbonDioxide);
        void EditUnit(Guid ID, int index, string stringValue);
        void EditUnit(Guid ID, int index, double doubleValue);
        void DeleteUnit(Guid ID);
        void SaveUnits(List<ProductionAsset> AllAssets, string fileName);
        List<ProductionAsset> LoadUnits(string fileName);
        List<ProductionAsset> GetAllUnits();
        List<ProductionAsset> SearchUnits(string name);
        void SetSaveFile(string fileName);
    }
    public interface IAssetStorage
    {
        void SaveUnits(List<ProductionAsset> AllAssets, string fileName);
        List<ProductionAsset> LoadUnits(string fileName);
    }
}