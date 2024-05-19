using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using System.Linq;
using DynamicData;
using System.Runtime.Serialization;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using HeatOptimiser;
using System.Globalization;






namespace UserInterface.ViewModels;



    

public class NewAsset : ViewModelBase
{
    public string _assetName;
    public string _assetHeat;
    public string _assetElectricity;
    public string _assetEnergy;
    public string _assetCost;
    public string _assetCarbon;
    public bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }
    
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
        IsSelected=false;
    }
}
public class AssetManagerViewModel : ViewModelBase
{
    public JsonAssetStorage jsonAssetStorage;
    
    //public ProductionAsset productionAsset;
    public string _errorText1;
    public string _errorText2;
    public string _errorText3;
    public string _errorText4;
    public string _errorText5;
    public string _errorText6;
    public string ErrorText1{
        get => _errorText1;
        set => this.RaiseAndSetIfChanged(ref _errorText1, value);
    }
    public string ErrorText2{
        get => _errorText2;
        set => this.RaiseAndSetIfChanged(ref _errorText2, value);
    }
    public string ErrorText3{
        get => _errorText3;
        set => this.RaiseAndSetIfChanged(ref _errorText3, value);
    }
    public string ErrorText4{
        get => _errorText4;
        set => this.RaiseAndSetIfChanged(ref _errorText4, value);
    }
    public string ErrorText5{
        get => _errorText5;
        set => this.RaiseAndSetIfChanged(ref _errorText5, value);
    }
    public string ErrorText6{
        get => _errorText6;
        set => this.RaiseAndSetIfChanged(ref _errorText6, value);
    }
    
    public string _assetNameNew;
    public string AssetNameNew
        {
            get =>_assetNameNew;
            set => this.RaiseAndSetIfChanged(ref _assetNameNew, value);
        }
    public string _assetHeatNew;
    public string AssetHeatNew{get =>_assetHeatNew;
    set{
        this.RaiseAndSetIfChanged(ref _assetHeatNew, value);
        if (!double.TryParse(AssetHeatNew, out _) && AssetHeatNew!= string.Empty)
        {   ErrorText2 = "Input must be a valid double.";}
        else{   ErrorText2 = string.Empty;}
        }
    }
    public string _assetElectricityNew;
    public string AssetElectricityNew{get =>_assetElectricityNew;
    set{
        this.RaiseAndSetIfChanged(ref _assetElectricityNew, value);
        if (!double.TryParse(AssetElectricityNew, out _) && AssetElectricityNew!= string.Empty)
        {   ErrorText3 = "Input must be a valid double.";}
        else{   ErrorText3 = string.Empty;}
        }
    }
    public string _assetEnergyNew;
    public string AssetEnergyNew{get =>_assetEnergyNew;
    set{
        this.RaiseAndSetIfChanged(ref _assetEnergyNew, value);
        if (!double.TryParse(AssetEnergyNew, out _) && AssetEnergyNew!= string.Empty)
        {   ErrorText4 = "Input must be a valid double.";}
        else{   ErrorText4 = string.Empty;}
        }
    }
    public string _assetCostNew;
    public string AssetCostNew{get =>_assetCostNew;
    set{
        this.RaiseAndSetIfChanged(ref _assetCostNew, value); 
        if (!double.TryParse(AssetCostNew, out _) && AssetCostNew!= string.Empty)
            {   ErrorText5 = "Input must be a valid double.";}
            else
            {   ErrorText5 = string.Empty;}
            }
    }
    public string _assetCarbonNew;
    public string AssetCarbonNew{get =>_assetCarbonNew;
    set {
        this.RaiseAndSetIfChanged(ref _assetCarbonNew, value);
        if (!double.TryParse(AssetCarbonNew, out _) && AssetCarbonNew!= string.Empty)
        {   ErrorText6 = "Input must be a valid double.";}
        else{   ErrorText6 = string.Empty;}
        }
    }
    public string _assetButton = "Add Unit";
    public string AssetButton 
    {
        get =>_assetButton;
        set => this.RaiseAndSetIfChanged(ref _assetButton, value);
    }
    public int _assetCount;
    public int AssetCount
    {
        get => Assets.Count();
    }
    public string _errorText;
    public string ErrorText
    {
        get => _errorText;
        set => this.RaiseAndSetIfChanged(ref _errorText, value);
    }
    public ObservableCollection<NewAsset> Assets {get;} = new();
    public ObservableCollection<ProductionAsset> ProductionAssets{get; set;} = new();
    
    public ReactiveCommand<Unit, Unit> AddAssetCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteAssetCommand { get; }
    public ReactiveCommand<Unit, Unit> UpdateAssetCommand { get; }
    
    public string _assetEdit;

    public string AssetToEdit
    {
        get => _assetEdit;
        set => this.RaiseAndSetIfChanged(ref _assetEdit, value);
    }
    public void AddAsset()
    {
        if (AssetNameNew!= null && double.TryParse(AssetHeatNew, out double AssetHeat)&&double.TryParse(AssetElectricityNew, out double AssetElectricity) &&double.TryParse(AssetEnergyNew, out double AssetEnergy) &&double.TryParse(AssetCostNew, out double AssetCost) &&double.TryParse(AssetCarbonNew, out double AssetCarbon))
            {
            AssetManager.AddUnit(AssetNameNew,"none",AssetHeat,AssetElectricity, AssetEnergy, AssetCost, AssetCarbon);
            AssetNameNew=string.Empty;
            AssetHeatNew=string.Empty;
            AssetElectricityNew=string.Empty;
            AssetEnergyNew=string.Empty;
            AssetCostNew=string.Empty;
            AssetCarbonNew=string.Empty;
            AssetButton="Add Unit";
            }
       
        
    }
    public void DeleteAsset()
    {
        var selectedAsset = ProductionAssets.Where(x => x.IsSelected == true).ToList(); 
        foreach (var asset in selectedAsset)
        {
            AssetManager.DeleteUnit(asset.ID);
        }
    }
    public void EditAsset()
    {   
        AssetManager.SaveUnits(ProductionAssets, "ProductionAssets.json");

    }
    public void ValidateInput(string input)
    {   if (!double.TryParse(input, out _) && input!= string.Empty)
        {   ErrorText = "Input must be a valid double.";}
        else
        {   ErrorText = string.Empty;}}
    public AssetManagerViewModel()
    {
        //Assets = new ObservableCollection<ProductionAsset>(assetManager.LoadUnits("ProductionAssets.json"));
        AddAssetCommand=ReactiveCommand.Create(AddAsset);
        DeleteAssetCommand=ReactiveCommand.Create(DeleteAsset);
        UpdateAssetCommand=ReactiveCommand.Create(EditAsset);
        //assetManager.SaveUnits(ProductionAssets, assetManager.saveFileName);
        ProductionAssets=AssetManager.LoadUnits(AssetManager.saveFileName);
    }

}