using HeatOptimiser;
using ReactiveUI;
using System;
using System.Reactive;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace UserInterface.ViewModels;

public class OptimiserViewModel : ViewModelBase
{
    public DateTime _startingDate = new DateTime(2023,7,12,0,0,0);
    public DateTime _endingDate = new DateTime(2023,7,13,0,0,0);
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
    // Optimises the schedule by use of Optimiser module, given the start and end dates as well as a category index.
    public static void Optimise(DateTime start, DateTime end, int categoryIndex)
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