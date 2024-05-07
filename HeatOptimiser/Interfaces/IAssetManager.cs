using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HeatOptimiser
{
    public interface IAssetManager
    {
        void AddUnit(string name, string imgae, double heat, double electricity, double energy, double cost, double carbonDioxide);
        void EditUnit(Guid ID, int index, string stringValue);
        void EditUnit(Guid ID, int index, double doubleValue);
        void DeleteUnit(Guid ID);
        void SaveUnits(ObservableCollection<ProductionAsset> AllAssets, string fileName);
        ObservableCollection<ProductionAsset> LoadUnits(string fileName);
        ObservableCollection<ProductionAsset> GetAllUnits();
        ObservableCollection<ProductionAsset> SearchUnits(string name);
        void SetSaveFile(string fileName);
    }
    public interface IAssetStorage
    {
        void SaveUnits(ObservableCollection<ProductionAsset> AllAssets, string fileName);
        ObservableCollection<ProductionAsset> LoadUnits(string fileName);
    }
}