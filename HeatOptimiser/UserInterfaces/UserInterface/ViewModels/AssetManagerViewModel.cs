using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using System.Linq;
using DynamicData;
using System.Runtime.Serialization;
using Avalonia.Controls;
using System;
using System.Collections.Generic;




namespace UserInterface.ViewModels;


public class NewAsset : ViewModelBase
{
    public string _assetName;
    public string _assetHeat;
    public string _assetElectricity;
    public string _assetEnergy;
    public string _assetCost;
    public string _assetCarbon;
    public string AssetName{get =>_assetName;
    set=> this.RaiseAndSetIfChanged(ref _assetName, value);
    }
    public string AssetHeat{get =>_assetHeat;
    set=> this.RaiseAndSetIfChanged(ref _assetHeat, value);
    }
    public string AssetElectricity{get =>_assetElectricity;
    set=> this.RaiseAndSetIfChanged(ref _assetElectricity, value);
    }
    public string AssetEnergy{get =>_assetEnergy;
    set=> this.RaiseAndSetIfChanged(ref _assetEnergy, value);
    }
    public string AssetCost{get =>_assetCost;
    set=> this.RaiseAndSetIfChanged(ref _assetCost, value);
    }
    public string AssetCarbon{get =>_assetCarbon;
    set=> this.RaiseAndSetIfChanged(ref _assetCarbon, value);
    }
    public NewAsset(string n, string h, string elec, string ener, string cost, string carbon)
    {
        AssetName=n;
        AssetHeat=h;
        AssetElectricity=elec;
        AssetEnergy=ener;
        AssetCost=cost;
        AssetCarbon=carbon;
    }
}
public class AssetManagerViewModel : ViewModelBase
{
    
    public string _assetNameNew;
    public string AssetNameNew{get =>_assetNameNew;
    set=> this.RaiseAndSetIfChanged(ref _assetNameNew, value);
    }
    public string _assetHeatNew;
    public string AssetHeatNew{get =>_assetHeatNew;
    set=> this.RaiseAndSetIfChanged(ref _assetHeatNew, value);
    }
    public string _assetElectricityNew;
    public string AssetElectricityNew{get =>_assetElectricityNew;
    set=> this.RaiseAndSetIfChanged(ref _assetElectricityNew, value);
    }
    public string _assetEnergyNew;
    public string AssetEnergyNew{get =>_assetEnergyNew;
    set=> this.RaiseAndSetIfChanged(ref _assetEnergyNew, value);
    }
    public string _assetCostNew;
    public string AssetCostNew{get =>_assetCostNew;
    set=> this.RaiseAndSetIfChanged(ref _assetCostNew, value);
    }
    public string _assetCarbonNew;
    public string AssetCarbonNew{get =>_assetCarbonNew;
    set=> this.RaiseAndSetIfChanged(ref _assetCarbonNew, value);
    }
    public int Testindex=0;
    ObservableCollection<NewAsset> Assets {get;} = new();
    //public List<string> TestList {get;}=new(){"ItemNumber1"};
    public ReactiveCommand<Unit, Unit> AddAssetCommand { get; }

    public string _assetEdit;

    public string AssetToEdit
    {
        get => _assetEdit;
        set => this.RaiseAndSetIfChanged(ref _assetEdit, value);
    }
    public void AddAsset()
    {
        Assets.Add(new NewAsset(AssetNameNew, AssetHeatNew, AssetElectricityNew, AssetEnergyNew, AssetCostNew, AssetCarbonNew));
        Console.WriteLine(Assets[Testindex].AssetName);
    }
    public void DeleteAsset()
    {
    }
    public void EditAsset()
    {   
    }
    public AssetManagerViewModel()
    {
        
        AddAssetCommand=ReactiveCommand.Create(AddAsset);
        
    }

}