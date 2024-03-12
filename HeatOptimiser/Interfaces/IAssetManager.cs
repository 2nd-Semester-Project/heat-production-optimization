namespace HeatOptimiser
{
    public interface IAssetManager
    {
        void AddUnit(string name, string imgae, double heat, double electricity, double energy, double cost, double carbonDioxide);
        void EditUnit(Guid ID, int index, string value);
        void DeleteUnit(Guid ID);
        void SaveUnits(List<ProductionAsset> AllAssets, string fileName);
        List<ProductionAsset> LoadUnits(string fileName);
        List<ProductionAsset> GetAllUnits();
        List<ProductionAsset> SearchUnits();
    }
    public interface IAssetStorage
    {
        void SaveUnits(List<ProductionAsset> AllAssets, string fileName);
        List<ProductionAsset> LoadUnits(string fileName);
    }
}