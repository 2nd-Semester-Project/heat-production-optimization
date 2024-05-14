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
    public int _assetCount;
    public int AssetCount
    {
        get => _assetCount;
        set => this.RaiseAndSetIfChanged(ref _assetCount, value);
    }
    private readonly ObservableCollection<DateTimePoint> HeatDemandData;
    public ObservableCollection<ISeries> Series { get; set; }



    public HomepageViewModel()
    {
        // Use ObservableCollections to let the chart listen for changes (or any INotifyCollectionChanged). 
        HeatDemandData = new ObservableCollection<DateTimePoint>();

        foreach (var point in DataVisualizer.sourceData.LoadedData)
        {
            if (point.HeatDemand.HasValue && point.TimeFrom.HasValue)
            {
                HeatDemandData.Add(new DateTimePoint(point.TimeFrom.Value, point.HeatDemand.Value)); //Right now does not read the TimeFrom Value that is why the data is not displayed
            }
        }

        
        Series = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = HeatDemandData,
                Name = "Heat Demand (MWh)",
                Fill = null,
                GeometryStroke = null,
                GeometryFill = null,
                LineSmoothness = 1 ,
            },
        };

        AssetCount = AssetManager.LoadUnits(AssetManager.saveFileName).Count;
    }
    public Axis[] XAxes { get; set; } =
    {
        new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMMM dd HH:mm"))
    };
}