using System.Collections.ObjectModel;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using HeatOptimiser;
using ReactiveUI;

namespace UserInterface.ViewModels;

public class HomepageViewModel : ViewModelBase
{
     
    AssetManager assetManager = new();
    ObservableCollection<ProductionAsset> ProductionAssets;
    public int _assetCount;
    public int AssetCount
    {
        get => _assetCount;
        set => this.RaiseAndSetIfChanged(ref _assetCount, value);
    }

    public HomepageViewModel()
    {
        
        AssetCount = assetManager.LoadUnits(assetManager.saveFileName).Count;
    }
}