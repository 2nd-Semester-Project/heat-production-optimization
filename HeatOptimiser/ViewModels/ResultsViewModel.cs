using ReactiveUI;
using System.Reactive;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using HeatOptimiser;
using System.Collections.ObjectModel;

namespace UserInterface.ViewModels;

public class ResultsViewModel : ViewModelBase
{
    private ObservableCollection<ISeries> _series = [];
    public ObservableCollection<ISeries> Series {
        get => _series;
        set => this.RaiseAndSetIfChanged(ref _series, value);
    }
    private Axis[] _YAxes = [];
    public Axis[] YAxes {
        get => _YAxes;
        set => this.RaiseAndSetIfChanged(ref _YAxes, value);
    }
    private Axis[] _XAxes = [];
    public Axis[] XAxes {
        get => _XAxes;
        set => this.RaiseAndSetIfChanged(ref _XAxes, value);
    }
    public ReactiveCommand<Unit, Unit> SelectUsageChart {get;}
    public ReactiveCommand<Unit, Unit> SelectCostsChart {get;}
    public ReactiveCommand<Unit, Unit> SelectEmissionsChart {get;}
    public ReactiveCommand<Unit, Unit> SelectElectricityChart {get;}
    public ReactiveCommand<Unit, Unit> SelectCostByOptimisationChart {get;}
    public ReactiveCommand<Unit, Unit> SelectEmissionsByOptimisationChart {get;}
    private bool _assetsSelected;
    public bool AssetsSelected
    {
        get =>  _assetsSelected;
        set =>  this.RaiseAndSetIfChanged(ref _assetsSelected, value);
    }
    public ResultsViewModel()
    {
        SelectUsageChart = ReactiveCommand.Create(UsageChart);
        SelectCostsChart = ReactiveCommand.Create(CostsChart);
        SelectEmissionsChart = ReactiveCommand.Create(EmissionsChart);
        SelectElectricityChart = ReactiveCommand.Create(ElectricityChart);
        SelectCostByOptimisationChart = ReactiveCommand.Create(CostByOptimisationChart);
        SelectEmissionsByOptimisationChart = ReactiveCommand.Create(EmissionsByOptimisationChart);
        UsageChart();
        AssetsSelected = ResultsDataManager.AssetsSelected;
    }
    // Synchronizes the variables from DataVisualiser module
    public void SyncWithVisualiser()
    {
        Series = DataVisualiser.Series;
        XAxes = DataVisualiser.XAxes;
        YAxes = DataVisualiser.YAxes;
    }

    // Generates a chart displaying the usage of assets throughout the schedule
    public void UsageChart()
    {
        DataVisualiser.VisualiseUsageData();
        SyncWithVisualiser();
    }
    // Generates a chart displaying the different costs throughout the schedule
    public void CostsChart()
    {
        DataVisualiser.VisualiseCostsData();
        SyncWithVisualiser();
    }
    // Generates a chart displaying the emissions throughout the schedule
    public void EmissionsChart()
    {
        DataVisualiser.VisualiseEmissionsData();
        SyncWithVisualiser();
    }
    // Generates a chart displaying the usage and price of electricity throughout the schedule
    public void ElectricityChart()
    {
        DataVisualiser.VisualiseElectricityData();
        SyncWithVisualiser();
    }
    // Generates a chart displaying the total cost throughout the schedule for different optimisation scenarios
    public void CostByOptimisationChart()
    {
        DataVisualiser.VisualiseCostByOptimisationData();
        SyncWithVisualiser();
    }
    // Generates a chart displaying the emissions throughout the schedule for different optimisation scenarios
    public void EmissionsByOptimisationChart()
    {
        DataVisualiser.VisualiseEmissionsByOptimisationData();
        SyncWithVisualiser();
    }
}