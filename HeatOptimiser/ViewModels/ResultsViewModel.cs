using System;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using HeatOptimiser;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting;

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
    public void SyncWithVisualiser()
    {
        Series = DataVisualizer.Series;
        XAxes = DataVisualizer.XAxes;
        YAxes = DataVisualizer.YAxes;
    }
    public void UsageChart()
    {
        DataVisualizer.VisualiseUsageData();
        SyncWithVisualiser();
    }
    public void CostsChart()
    {
        DataVisualizer.VisualiseCostsData();
        SyncWithVisualiser();
    }
    public void EmissionsChart()
    {
        DataVisualizer.VisualiseEmissionsData();
        SyncWithVisualiser();
    }
    public void ElectricityChart()
    {
        DataVisualizer.VisualiseElectricityData();
        SyncWithVisualiser();
    }
    public void CostByOptimisationChart()
    {
        DataVisualizer.VisualiseCostByOptimisationData();
        SyncWithVisualiser();
    }
    public void EmissionsByOptimisationChart()
    {
        DataVisualizer.VisualiseEmissionsByOptimisationData();
        SyncWithVisualiser();
    }
}