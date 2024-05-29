using System;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using UserInterface.ViewModels;
using System.Net;
using Avalonia.Data;
using System.Globalization;
using Avalonia.Data.Converters;

namespace HeatOptimiser
{
    public class ProductionAsset : ViewModelBase 
    {
        public Guid ID { get; } = Guid.NewGuid();
        private string? _name;
        public string? Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string? _image;
        public string? Image
        {
            get { return _image; }
            set { _image = value; }
        }

        private double? _heat;
        public double? Heat
        {
            get { return _heat; }
            set { _heat = value; }
        }

        private double? _electricity;
        public double? Electricity
        {
            get { return _electricity; }
            set { this.RaiseAndSetIfChanged(ref _electricity,value);}
        }

        private double? _energy;
        public double? Energy
        {
            get { return _energy; }
            set { _energy=value;}
        }

        private double? _cost;
        public double? Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        private double? _carbonDioxide;
        public double? CarbonDioxide
        {
            get { return _carbonDioxide; }
            set { _carbonDioxide = value; }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }
        private bool _optimiseSelected;
        public bool OptimiseSelected
        {
            get => _optimiseSelected;
            set => this.RaiseAndSetIfChanged(ref _optimiseSelected, value);
        }
    }
    public static class AssetManager
    {
        public static ObservableCollection<ProductionAsset> _productionAssets = new ObservableCollection<ProductionAsset>();
        private static JsonAssetStorage _jsonAssetStorage = new JsonAssetStorage("ProductionAssets.json");
        public static void AddUnit(string name, string image, double heat, double electricity, double energy, double cost, double carbonDioxide)
        {
            if (name != null && image != null && !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(image))
            {
                _productionAssets.Add(new ProductionAsset
                {
                    Name = name,
                    Image = image,
                    Heat = heat,
                    Electricity = electricity,
                    Energy = energy,
                    Cost = cost,
                    CarbonDioxide = carbonDioxide,
                    IsSelected = false,
                    OptimiseSelected = false
                });
                _jsonAssetStorage.SaveUnits(_productionAssets); // this is up for debate, I just want to auto save, and they likely wont have thousands of production units, that could cause a performance issue.
            }
            else
            {
                throw new ArgumentNullException("Name and Image cannot be null");
            }
        }
        public static void DeleteUnit(Guid ID)
        {
            _productionAssets.Remove(_productionAssets.FirstOrDefault(x => x.ID == ID)!);
            _jsonAssetStorage.SaveUnits(_productionAssets); // this is also up for debate, just like on AddUnit.
        }
        public static void EditUnit(Guid ID, int index, string stringValue)
        {
            switch (index)
            {
                case 0:
                    _productionAssets.FirstOrDefault(x => x.ID == ID)!.Name = stringValue;
                    break;
                case 1:
                    _productionAssets.FirstOrDefault(x => x.ID == ID)!.Image = stringValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Index out of range or wrong type used");
            }
            _jsonAssetStorage.SaveUnits(_productionAssets); // this is also up for debate, just like on AddUnit.
        }
        public static void EditUnit(Guid ID, int index, double doubleValue)
        {
            switch (index)
            {
                case 2:
                    _productionAssets.FirstOrDefault(x => x.ID == ID)!.Heat = doubleValue;
                    break;
                case 3:
                    _productionAssets.FirstOrDefault(x => x.ID == ID)!.Electricity = doubleValue;
                    break;
                case 4:
                    _productionAssets.FirstOrDefault(x => x.ID == ID)!.Energy = doubleValue;
                    break;
                case 5:
                    _productionAssets.FirstOrDefault(x => x.ID == ID)!.Cost = doubleValue;
                    break;
                case 6:
                    _productionAssets.FirstOrDefault(x => x.ID == ID)!.CarbonDioxide = doubleValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Index out of range or wrong type used");
            }
            _jsonAssetStorage.SaveUnits(_productionAssets); // this is also up for debate, just like on AddUnit.
        }
        public static ObservableCollection<ProductionAsset> GetAllUnits()
        {
            return _productionAssets;
        }
        public static ObservableCollection<ProductionAsset> GetSelectedUnits()
        {
            var assets = _productionAssets.Where(x => x.OptimiseSelected == true).ToList();
            ObservableCollection<ProductionAsset> selectedAssets = new ObservableCollection<ProductionAsset>(assets);
            return selectedAssets;
            
        }
        public static ObservableCollection<ProductionAsset> LoadUnits()
        {
            _productionAssets = _jsonAssetStorage.LoadUnits();
            return _productionAssets;
        }
        public static void SaveUnits(ObservableCollection<ProductionAsset> AllAssets)
        {
            _jsonAssetStorage.SaveUnits(AllAssets);
        }
        public static ObservableCollection<ProductionAsset> SearchUnits(string name)
        {
            var selection = _productionAssets.Where(x => x.Name!.ToLower().Contains(name.ToLower())).ToList();
            ObservableCollection<ProductionAsset> selected = [.. selection];
            return selected;
        }
    }
    public class JsonAssetStorage
    {
        private string filePath;
        public JsonAssetStorage(string passedFilePath)
        {
            filePath = passedFilePath;
        }
        public ObservableCollection<ProductionAsset> LoadUnits()
        {
            if (File.Exists(filePath) && new FileInfo(filePath).Length > 2)
            {
                string jsonString = File.ReadAllText(filePath);
                try
                {
                    var info = JsonSerializer.Deserialize<ObservableCollection<ProductionAsset>>(jsonString)!;
                    return info;
                }
                catch (JsonException e)
                {
                    throw new JsonException("Error: " + e.Message);
                }
            }
            else
            {
                return new ObservableCollection<ProductionAsset>();
            }
        }
        public void SaveUnits(ObservableCollection<ProductionAsset> AllAssets)
        {
            string jsonString = JsonSerializer.Serialize(AllAssets);
            File.WriteAllText(filePath, jsonString);
        }
    }

    

}