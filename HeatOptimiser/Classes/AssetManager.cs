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
            set { _energy = value; }
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
        public bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }
    }
    public static class AssetManager
    {
        public static string saveFileName = "ProductionAssets.json"; 
        public static ObservableCollection<ProductionAsset> _productionAssets = new ObservableCollection<ProductionAsset>();
        private static JsonAssetStorage _jsonAssetStorage = new JsonAssetStorage();
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
                    IsSelected = false
                });
                _jsonAssetStorage.SaveUnits(_productionAssets, saveFileName); // this is up for debate, I just want to auto save, and they likely wont have thousands of production units, that could cause a performance issue.
            }
            else
            {
                throw new ArgumentNullException("Name and Image cannot be null");
            }
        }
        public static void DeleteUnit(Guid ID)
        {
            _productionAssets.Remove(_productionAssets.FirstOrDefault(x => x.ID == ID)!);
            _jsonAssetStorage.SaveUnits(_productionAssets, saveFileName); // this is also up for debate, just like on AddUnit.
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
            _jsonAssetStorage.SaveUnits(_productionAssets, saveFileName); // this is also up for debate, just like on AddUnit.
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
            _jsonAssetStorage.SaveUnits(_productionAssets, saveFileName); // this is also up for debate, just like on AddUnit.
        }
        public static ObservableCollection<ProductionAsset> GetAllUnits()
        {
            return _productionAssets;
        }
        public static ObservableCollection<ProductionAsset> LoadUnits(string fileName)
        {
            _productionAssets = _jsonAssetStorage.LoadUnits(fileName);
            return _productionAssets;
        }
        public static void SaveUnits(ObservableCollection<ProductionAsset> AllAssets, string fileName)
        {
            _jsonAssetStorage.SaveUnits(AllAssets, fileName);
        }
        public static ObservableCollection<ProductionAsset> SearchUnits(string name)
        {
            var selection = _productionAssets.Where(x => x.Name!.ToLower().Contains(name.ToLower())).ToList();
            ObservableCollection<ProductionAsset> selected = [.. selection];
            return selected;
        }
        public static void SetSaveFile(string fileName)
        {
            saveFileName = fileName;
        }
    }
    public class JsonAssetStorage
    {
        public ObservableCollection<ProductionAsset> LoadUnits(string fileName)
        {
            if (File.Exists(fileName) && new FileInfo(fileName).Length > 2)
            {
                string jsonString = File.ReadAllText(fileName);
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
        public void SaveUnits(ObservableCollection<ProductionAsset> AllAssets, string fileName)
        {
            string jsonString = JsonSerializer.Serialize(AllAssets);
            File.WriteAllText(fileName, jsonString);
        }
    }
}