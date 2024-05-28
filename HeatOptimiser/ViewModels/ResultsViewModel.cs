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
    public ReactiveCommand<Unit, Unit> SelectScheduleChart {get;}
    public ReactiveCommand<Unit, Unit> SelectCostsChart {get;}
    public ReactiveCommand<Unit, Unit> SelectEmissionsChart {get;}
    public ReactiveCommand<Unit, Unit> SelectElectricityChart {get;}
    public ResultsViewModel()
    {
        SelectScheduleChart = ReactiveCommand.Create(ScheduleChart);
        SelectCostsChart = ReactiveCommand.Create(CostsChart);
        SelectEmissionsChart = ReactiveCommand.Create(EmissionsChart);
        SelectElectricityChart = ReactiveCommand.Create(ElectricityChart);
        ScheduleChart();
    }
    public void ScheduleChart()
    {
        Schedule results = ResultsDataManager.LoadAll();
        List<List<double>> demandsList = [];
        List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
        
        foreach (ProductionAsset asset in assets)
        {
            List<double> demands = [];
            foreach (ScheduleHour hour in results.schedule)
            {
                if (hour.Assets!.Contains(asset))
                {
                    demands.Add(hour.Demands![hour.Assets.IndexOf(asset)]);
                }
                else
                {
                    demands.Add(0);
                }
            }
            demandsList.Add(demands);
        }
        List<string> AssetNames = [];
        foreach (ProductionAsset asset in assets)
        {
            AssetNames.Add(asset.Name!);
        }

        Series = [];
        foreach(List<double> demands in demandsList)
        {
            StackedStepAreaSeries<double> ssaSeries = new()
            {
                Values = demands,
                Name = AssetNames[demandsList.IndexOf(demands)],
                Stroke = null
            };
            Series.Add(ssaSeries);
        }
        
        YAxes = [
            new()
            {
                Name = "Usage (MWh)",
                TextSize = 16,
                NameTextSize = 18
            }
        ];
    
        List<DateTime> hours = [];
        foreach (ScheduleHour hour in results.schedule)
        {
            hours.Add(hour.Hour!.Value);
        }

        XAxes =
        [
            new Axis
            {
                Labels = hours.Select(hour => hour.ToString("dd/MM/yyyy HH:mm")).ToArray()
            }
        ];
    }
    public void CostsChart()
    {
        Schedule results = ResultsDataManager.LoadAll();
        List<List<double>> costList = [[], [], []]; //Asset, Electricity, Total
        List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
        foreach (ScheduleHour hour in results.schedule)
        {
            List<double> hourlyCosts = [0, 0];
            foreach (ProductionAsset asset in assets)
            {
                if (hour.Assets!.Contains(asset))
                {
                    hourlyCosts[0] += hour.Demands![hour.Assets.IndexOf(asset)]*-(double)asset.Cost!;
                    hourlyCosts[1] += hour.Demands![hour.Assets.IndexOf(asset)]*(double)asset.Electricity!*(double)SourceDataManager.GetDataByDateTime((DateTime)hour.Hour!)!.ElectricityPrice!;
                }
            }
            costList[0].Add(hourlyCosts[0]);
            costList[1].Add(hourlyCosts[1]);
            costList[2].Add(hourlyCosts[0]+hourlyCosts[1]);
        }
        List<string> CostNames = ["Assets", "Electricity", "Total"];

        Series = [];
        foreach(List<double> costs in costList)
        {
            LineSeries<double> lineSeries = new()
            {
                Values = costs,
                Name = CostNames[costList.IndexOf(costs)],
                Stroke = null
            };
            Series.Add(lineSeries);
        }
        
        YAxes = [
            new()
            {
                Name = "Cost (€)",
                TextSize = 16,
                NameTextSize = 18
            }
        ];
    
        List<DateTime> hours = [];
        foreach (ScheduleHour hour in results.schedule)
        {
            hours.Add(hour.Hour!.Value);
        }

        XAxes =
        [
            new Axis
            {
                Labels = hours.Select(hour => hour.ToString("dd/MM/yyyy HH:mm")).ToArray()
            }
        ];
    }
    public void EmissionsChart()
    {
        Schedule results = ResultsDataManager.LoadAll();
        List<double> emissions = [];
        List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
        foreach (ScheduleHour hour in results.schedule)
        {
            double hourlyEmissions = 0;
            foreach (ProductionAsset asset in assets)
            {
                if (hour.Assets!.Contains(asset))
                {
                    hourlyEmissions += hour.Demands![hour.Assets.IndexOf(asset)]*(double)asset.CarbonDioxide!;
                }
            }
            emissions.Add(hourlyEmissions);
        }

        Series = [new LineSeries<double>()
        {
            Values = emissions,
            Stroke = null
        }];
        
        YAxes = [
            new()
            {
                Name = "Amount (t)",
                TextSize = 16,
                NameTextSize = 18
            }
        ];
    
        List<DateTime> hours = [];
        foreach (ScheduleHour hour in results.schedule)
        {
            hours.Add(hour.Hour!.Value);
        }

        XAxes =
        [
            new Axis
            {
                Labels = hours.Select(hour => hour.ToString("dd/MM/yyyy HH:mm")).ToArray()
            }
        ];
    }
    public void ElectricityChart()
    {
        Schedule results = ResultsDataManager.LoadAll();
        List<double> usage = [];
        List<double> price = [];
        List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
        foreach (ScheduleHour hour in results.schedule)
        {
            double hourlyUsage = 0;
            foreach (ProductionAsset asset in assets)
            {
                if (hour.Assets!.Contains(asset))
                {
                    hourlyUsage += hour.Demands![hour.Assets.IndexOf(asset)]*-(double)asset.Electricity!;
                }
            }
            usage.Add(hourlyUsage);
            price.Add((double)SourceDataManager.GetDataByDateTime((DateTime)hour.Hour!)!.ElectricityPrice!);
        }

        Series = [];
        LineSeries<double> usageSeries = new()
        {
            Values = usage,
            Name = "Usage",
            Stroke = null
        };
        Series.Add(usageSeries);

        LineSeries<double> priceSeries = new()
        {
            Values = price,
            Name = "Price",
            Stroke = null,
            ScalesYAt = 1
        };
        Series.Add(priceSeries);
    
        YAxes = [
            new()
            {
                Name = "Usage (MWh)",
                TextSize = 16,
                NameTextSize = 18
            },
            new()
            {
                Name = "Price (€/MWh)",
                TextSize = 16,
                NameTextSize = 18,
                Position = LiveChartsCore.Measure.AxisPosition.End
            }
        ];

        List<DateTime> hours = [];
        foreach (ScheduleHour hour in results.schedule)
        {
            hours.Add(hour.Hour!.Value);
        }

        XAxes =
        [
            new Axis
            {
                Labels = hours.Select(hour => hour.ToString("dd/MM/yyyy HH:mm")).ToArray()
            }
        ];


    }
}