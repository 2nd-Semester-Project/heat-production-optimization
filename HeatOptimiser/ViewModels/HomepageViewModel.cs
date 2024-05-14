using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using DynamicData;
using HeatOptimiser;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using ReactiveUI;

namespace UserInterface.ViewModels;

public class HomepageViewModel : ViewModelBase
{
    ObservableCollection<ProductionAsset> ProductionAssets;
    public int _assetCount;
    public int AssetCount
    {
        get => _assetCount;
        set => this.RaiseAndSetIfChanged(ref _assetCount, value);
    }
    private readonly Random _random = new();
    private readonly ObservableCollection<DateTimePoint> WinterHeatDemandData;
    private readonly ObservableCollection<DateTimePoint> SummerHeatDemandData;
    public ObservableCollection<ISeries> WinterSeries { get; set; }
    public ObservableCollection<ISeries> SummerSeries { get; set; }



    public HomepageViewModel()
    {
        // Use ObservableCollections to let the chart listen for changes (or any INotifyCollectionChanged). 
        WinterHeatDemandData = new ObservableCollection<DateTimePoint>();
        SummerHeatDemandData = new ObservableCollection<DateTimePoint>();

        foreach (var point in DataVisualizer.sourceData.WinterData)
        {
            if (point.HeatDemand.HasValue && point.TimeFrom.HasValue)
            {
                WinterHeatDemandData.Add(new DateTimePoint(point.TimeFrom.Value, point.HeatDemand.Value)); //Right now does not read the TimeFrom Value that is why the data is not displayed
            }
        }

        

        foreach (var point in DataVisualizer.sourceData.SummerData)
        {
            if (point.HeatDemand.HasValue&&point.TimeFrom.HasValue)
            {
                SummerHeatDemandData.Add(new DateTimePoint(point.TimeFrom.Value, point.HeatDemand.Value));
          
  }
        }
        WinterSeries = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = WinterHeatDemandData,
                Name = "Winter Heat Demand (MWh)",
                Fill = null,
                GeometryStroke = null,
                GeometryFill = null,
                LineSmoothness = 1 ,
            },
        };

        SummerSeries = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = SummerHeatDemandData,
                Name = "Summer Heat Demand (MWh)",
                Fill = null,
                GeometryStroke = null,
                GeometryFill = null,
                LineSmoothness = 1,
            },
        };

        AssetCount = AssetManager.LoadUnits(AssetManager.saveFileName).Count;
    }
    public Axis[] XAxesSummer { get; set; } =
    {
        new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMMM dd HH:mm"))
    };
    public Axis[] XAxesWinter { get; set; } =
    {
        new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMMM dd HH:mm"))
    };
}