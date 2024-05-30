using HeatOptimiser;
using ReactiveUI;
using System;
using Avalonia.Controls;
using System.Reactive;
using System.Data.SqlTypes;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace UserInterface.ViewModels;

public class OptimiserViewModel : ViewModelBase
{
    public DateTime _startingDate = new DateTime(2023,7,12,0,0,0);
    public DateTime _endingDate = new DateTime(2023,7,13,0,0,0);
    //public var optimisationCategory = this.Find<ComboBox>("OptimisationCategory")
    public DateTime StartingDate
    {
        get => _startingDate;
        set => this.RaiseAndSetIfChanged(ref _startingDate, value);
    }
    public DateTime EndingDate
    {
        get => _endingDate;
        set => this.RaiseAndSetIfChanged(ref _endingDate, value);
    }
    public ObservableCollection<ProductionAsset> ProductionAssets{get; set;} = new();
    public ReactiveCommand<Unit, Unit> OptimiseCommand { get; }
    private int _selectedCategoryIndex;
    public int SelectedCategoryIndex
    {
        get =>  _selectedCategoryIndex;
        set =>  this.RaiseAndSetIfChanged(ref _selectedCategoryIndex, value);
    }
    public void Optimise(DateTime start, DateTime end, int categoryIndex)
    {
        ResultsDataManager.AssetsSelected = AssetManager.GetSelectedUnits().Count > 0;
        OptimisationChoice choice;
        if (categoryIndex == 0)
        {
            choice = OptimisationChoice.Cost;
        }
        else
        {
            choice = OptimisationChoice.Emissions;
        }
        Schedule optimisedData = Optimiser.Optimise(start, end, choice);
        ResultsDataManager.Save(optimisedData);
        
        Console.WriteLine(StartingDate);
        Console.WriteLine("Optimised Schedule:");
        foreach (var hour in optimisedData.schedule)
        {
            Console.WriteLine($"Hour: {hour.Hour}, Assets: {string.Join(",", hour.Assets!)}, Demands: {string.Join(",", hour.Demands!)}");
            foreach (var asset in hour.Assets!)
            {
                Console.WriteLine($"Asset: {asset.Name}, Heat: {asset.Heat}, Demand {hour.Demands![hour.Assets!.IndexOf(asset)]}");
            }
        }
    }
           
    public OptimiserViewModel()
    {
        List<DateTime> StartEndDates = SourceDataManager.GetDates();
        StartingDate = StartEndDates[0];
        EndingDate = StartEndDates[1];
        
        ProductionAssets = AssetManager.LoadUnits();
        OptimiseCommand=ReactiveCommand.Create(()=> Optimise(_startingDate, _endingDate, _selectedCategoryIndex));
    }
}