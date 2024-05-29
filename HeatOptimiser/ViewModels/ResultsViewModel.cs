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
    readonly List<SKColor> colors = [
        new SKColor(194, 36, 62),
        new SKColor(0, 92, 230),
        new SKColor(100, 87, 195)
    ];
    readonly List<SKColor> alphaColors = [
        new SKColor(194, 36, 62, 40),
        new SKColor(0, 92, 230, 40),
        new SKColor(100, 87, 195, 40)
    ];
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
    public ReactiveCommand<Unit, Unit> SelectProfitChart {get;}
    public ReactiveCommand<Unit, Unit> SelectEmissionsChart {get;}
    public ReactiveCommand<Unit, Unit> SelectElectricityChart {get;}
    public ReactiveCommand<Unit, Unit> SelectProfitByOptimisationChart {get;}
    public ReactiveCommand<Unit, Unit> SelectEmissionsByOptimisationChart {get;}
    public ResultsViewModel()
    {
        SelectUsageChart = ReactiveCommand.Create(UsageChart);
        SelectProfitChart = ReactiveCommand.Create(ProfitChart);
        SelectEmissionsChart = ReactiveCommand.Create(EmissionsChart);
        SelectElectricityChart = ReactiveCommand.Create(ElectricityChart);
        SelectProfitByOptimisationChart = ReactiveCommand.Create(ProfitByOptimisationChart);
        SelectEmissionsByOptimisationChart = ReactiveCommand.Create(EmissionsByOptimisationChart);
        UsageChart();
    }
    public void UsageChart()
    {
        Schedule results = ResultsDataManager.ResultsSchedule;
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
        
        List<double> demandValues = [];
        foreach(ScheduleHour hour in results.schedule)
        {
            double demand = (double)SourceDataManager.GetDataByDateTime(hour.Hour!.Value)!.HeatDemand!;
            demandValues.Add(demand);
        }
        LineSeries<double> demandSeries = new()
        {
            Values = demandValues,
            Name = "Total demand",
            LineSmoothness = 1,
            Stroke = null
        };
        Series.Add(demandSeries);
        
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
    public void ProfitChart()
    {
        Schedule results = ResultsDataManager.ResultsSchedule;
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
        List<string> CostNames = ["Assets", "Electricity", "Total Profit"];

        Series = [];
        foreach(List<double> costs in costList)
        {
            LineSeries<double> lineSeries = new()
            {
                Values = costs,
                LineSmoothness = 1,
                Name = CostNames[costList.IndexOf(costs)],
                Stroke = null
            };
            Series.Add(lineSeries);
        }
        
        YAxes = [
            new()
            {
                Name = "Profit (€)",
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
        Schedule results = ResultsDataManager.ResultsSchedule;
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
            LineSmoothness = 1,
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
        Schedule results = ResultsDataManager.ResultsSchedule;
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
            LineSmoothness = 1,
            Stroke = null
        };
        Series.Add(usageSeries);

        LineSeries<double> priceSeries = new()
        {
            Values = price,
            Name = "Price",
            Stroke = null,
            LineSmoothness = 1,
            ScalesYAt = 1
        };
        Series.Add(priceSeries);
    
        YAxes = [
            new()
            {
                Name = "Usage (MWh)",
                TextSize = 16,
                NameTextSize = 18,
            },
            new()
            {
                Name = "Price (€/MWh)",
                TextSize = 16,
                NameTextSize = 18,
                Position = LiveChartsCore.Measure.AxisPosition.End,
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
    public void ProfitByOptimisationChart()
    {
        Schedule results = ResultsDataManager.ResultsSchedule;
        Console.WriteLine(results.schedule.Count);
        List<List<double>> costList = [[], [], []]; //NetCost, Cost, Emissions
        List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
        Schedule NetOptimised = Optimiser.NetOptimise(results.startDate, results.endDate);
        Schedule CostOptimised = Optimiser.Optimise(results.startDate, results.endDate, OptimisationChoice.Cost);
        Schedule EmissionOptimised = Optimiser.Optimise(results.startDate, results.endDate, OptimisationChoice.Emissions);
        for (int i = 0; i < NetOptimised.schedule.Count; i++)
        {
            List<double> hourlyCosts = [0, 0, 0];
            foreach (ProductionAsset asset in assets)
            {
                if (NetOptimised.schedule[i].Assets!.Contains(asset))
                {
                    ScheduleHour hour = NetOptimised.schedule[i];
                    hourlyCosts[0] += hour.Demands![hour.Assets.IndexOf(asset)]*-(double)asset.Cost!;
                    hourlyCosts[0] += hour.Demands![hour.Assets.IndexOf(asset)]*(double)asset.Electricity!*(double)SourceDataManager.GetDataByDateTime((DateTime)hour.Hour!)!.ElectricityPrice!;
                }
                if (CostOptimised.schedule[i].Assets!.Contains(asset))
                {
                    ScheduleHour hour = CostOptimised.schedule[i];
                    hourlyCosts[1] += hour.Demands![hour.Assets.IndexOf(asset)]*-(double)asset.Cost!;
                    hourlyCosts[1] += hour.Demands![hour.Assets.IndexOf(asset)]*(double)asset.Electricity!*(double)SourceDataManager.GetDataByDateTime((DateTime)hour.Hour!)!.ElectricityPrice!;
                }
                if (EmissionOptimised.schedule[i].Assets!.Contains(asset))
                {
                    ScheduleHour hour = EmissionOptimised.schedule[i];
                    hourlyCosts[2] += hour.Demands![hour.Assets.IndexOf(asset)]*-(double)asset.Cost!;
                    hourlyCosts[2] += hour.Demands![hour.Assets.IndexOf(asset)]*(double)asset.Electricity!*(double)SourceDataManager.GetDataByDateTime((DateTime)hour.Hour!)!.ElectricityPrice!;
                }
            }
            for (int j = 0; j < costList.Count; j++)
            {
                costList[j].Add(hourlyCosts[j]);
            }
        }
        List<string> CostNames = ["Net Cost", "Cost", "Emissions"];

        Series = [];
        int colorIndex = 0;
        foreach(List<double> costs in costList)
        {
            LineSeries<double> lineSeries = new()
            {
                Values = costs,
                LineSmoothness = 1,
                Fill = new SolidColorPaint(alphaColors[colorIndex%colors.Count])
                {
                    StrokeThickness = 3
                },
                GeometryStroke = null,
                GeometryFill = null,
                Name = CostNames[costList.IndexOf(costs)],
                Stroke = new SolidColorPaint(colors[colorIndex%colors.Count])
                {
                    StrokeThickness = 3
                }
            };
            colorIndex = (colorIndex + 1) < colors.Count ? colorIndex + 1 : 0;
            Series.Add(lineSeries);
        }
        
        YAxes = [
            new()
            {
                Name = "Profit (€)",
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
    public void EmissionsByOptimisationChart()
    {
        Schedule results = ResultsDataManager.ResultsSchedule;
        List<List<double>> emissionList = [[], [], []]; //NetCost, Cost, Emissions
        List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
        Schedule NetOptimised = Optimiser.NetOptimise(results.startDate, results.endDate);
        Schedule CostOptimised = Optimiser.Optimise(results.startDate, results.endDate, OptimisationChoice.Cost);
        Schedule EmissionOptimised = Optimiser.Optimise(results.startDate, results.endDate, OptimisationChoice.Emissions);
        for (int i = 0; i < NetOptimised.schedule.Count; i++)
        {
            List<double> hourlyEmissions = [0, 0, 0]; //NetCost, Cost, Emissions
            foreach (ProductionAsset asset in assets)
            {
                if (NetOptimised.schedule[i].Assets!.Contains(asset))
                {
                    ScheduleHour hour = NetOptimised.schedule[i];
                    hourlyEmissions[0] += hour.Demands![hour.Assets!.IndexOf(asset)]*(double)asset.CarbonDioxide!;
                }
                if (CostOptimised.schedule[i].Assets!.Contains(asset))
                {
                    ScheduleHour hour = CostOptimised.schedule[i];
                    hourlyEmissions[1] += hour.Demands![hour.Assets!.IndexOf(asset)]*(double)asset.CarbonDioxide!;
                }
                if (EmissionOptimised.schedule[i].Assets!.Contains(asset))
                {
                    ScheduleHour hour = EmissionOptimised.schedule[i];
                    hourlyEmissions[2] += hour.Demands![hour.Assets!.IndexOf(asset)]*(double)asset.CarbonDioxide!;
                }
            }
            for (int j = 0; j < emissionList.Count; j++)
            {
                emissionList[j].Add(hourlyEmissions[j]);
            }
        }
        List<string> CostNames = ["Net Cost", "Cost", "Emissions"];

        Series = [];
        int colorIndex = 0;
        foreach(List<double> emissions in emissionList)
        {
            LineSeries<double> lineSeries = new()
            {
                Values = emissions,
                LineSmoothness = 1,
                Fill = new SolidColorPaint(alphaColors[colorIndex%colors.Count])
                {
                    StrokeThickness = 3
                },
                GeometryStroke = null,
                GeometryFill = null,
                Name = CostNames[emissionList.IndexOf(emissions)],
                Stroke = new SolidColorPaint(colors[colorIndex%colors.Count])
                {
                    StrokeThickness = 3
                }
            };
            colorIndex = (colorIndex + 1) < colors.Count ? colorIndex + 1 : 0;
            Series.Add(lineSeries);
        }
        
        YAxes = [
            new()
            {
                Name = "Emissions (t)",
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
}