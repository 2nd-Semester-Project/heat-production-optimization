using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using HeatOptimiser;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using ReactiveUI;

namespace UserInterface.ViewModels;

public class HomepageViewModel : ViewModelBase
{

    AssetManager assetManager = new();
    ObservableCollection<ProductionAsset> ProductionAssets;
    public int _assetCount;
    public int AssetCount
    {
        get => _assetCount;
        set => this.RaiseAndSetIfChanged(ref _assetCount, value);
    }

    private readonly DataVisualizer dataVisualizer = new DataVisualizer();
    private readonly Random _random = new();
    //private readonly ObservableCollection<ObservableValue> WinterHeatDemandData;
    //private readonly ObservableCollection<ObservableValue> SummerHeatDemandData;
    public ObservableCollection<ISeries> WinterSeries { get; set; }
    public ObservableCollection<ISeries> SummerSeries { get; set; }



    public HomepageViewModel()
    {
        dataVisualizer = new DataVisualizer();


        // Use ObservableCollections to let the chart listen for changes (or any INotifyCollectionChanged). 
        // WinterHeatDemandData = new ObservableCollection<ObservableValue>();
        // SummerHeatDemandData = new ObservableCollection<ObservableValue>();

        // foreach (var point in dataVisualizer.sourceData.WinterData)
        // {
        //     if (point.HeatDemand.HasValue)
        //     {
        //         WinterHeatDemandData.Add(new ObservableValue(point.HeatDemand.Value));
        //     }
        // }

        // foreach (var point in dataVisualizer.sourceData.SummerData)
        // {
        //     if (point.HeatDemand.HasValue)
        //     {
        //         SummerHeatDemandData.Add(new ObservableValue(point.HeatDemand.Value));
        //     }
        // }

        // WinterSeries = new ObservableCollection<ISeries>
        // {
        //     new LineSeries<ObservableValue>
        //     {
        //         Values = WinterHeatDemandData,
        //         Fill = null,
        //         GeometryStroke = null,
        //         GeometryFill = null,
        //         LineSmoothness = 1 ,
        //     },
        // };
        SummerSeries = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = new ObservableCollection<DateTimePoint>(dataVisualizer.SummerHeatDemandData),
                // Fill = null,
                // GeometryStroke = null,
                // GeometryFill = null,
                // LineSmoothness = 1
            }
        };

        WinterSeries = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = new ObservableCollection<DateTimePoint>(dataVisualizer.WinterHeatDemandData),
                // Fill = null,
                // GeometryStroke = null,
                // GeometryFill = null,
                // LineSmoothness = 1
            }
        };

        AssetCount = assetManager.LoadUnits(assetManager.saveFileName).Count;
    }
}