using System;
using System.Linq;
using ReactiveUI;
using CommunityToolkit.Mvvm;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using HeatOptimiser;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Security.Cryptography.X509Certificates;

namespace UserInterface.ViewModels;

public class ResultsViewModel : ViewModelBase
{
    private readonly DataVisualizer dataVisualizer = new DataVisualizer();

    // public ObservableCollection<ObservableValue> WinterHeatDemandData = new ObservableCollection<ObservableValue>();
    // public ISeries[] Series {get; set;}


    // public ObservableCollection<ObservablePoint> WinterHeatDemandData { get; set; } = new ObservableCollection<ObservablePoint>();


    // public Axis[] XAxes { get; set; } =
    // {
    //     new Axis
    //     {
    //     LabelsRotation = 15,
    //     Labeler = value =>
    //     {
    //     try 
    //     {
    //         return new DateTime((long)value).ToString("MM-dd HH");
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error formatting date: {ex}");
    //         return "Invalid date";
    //     }
    //     },
    //         UnitWidth = TimeSpan.FromDays(1).Ticks // Sets the unit width to a day
    //     }
    // };

    // public Axis[] YAxes { get; set; } =
    // {
    //     new Axis
    //     {
    //         Name = "Heat Demand (kW)",
    //     }
    // };

    // public ResultsViewModel()
    // {
    //     dataVisualizer.AccessWinterData(); // Load winter data

    //     foreach (var point in dataVisualizer.sourceData.WinterData)
    //     {
    //         if (point.HeatDemand.HasValue)
    //         {
    //             WinterHeatDemandData.Add(new ObservableValue(point.HeatDemand.Value));
    //         }

    //     };


    // }


    // public void Initialize()
    // {
    //     WinterHeatDemandData.Add(new ObservableValue(1));
    //     WinterHeatDemandData.Add(new ObservableValue(2));
    //     WinterHeatDemandData.Add(new ObservableValue(3));
    //     WinterHeatDemandData.Add(new ObservableValue(4));
    //     WinterHeatDemandData.Add(new ObservableValue(5));
    //     WinterHeatDemandData.Add(new ObservableValue(6));

    //     Series = new ISeries[]
    //     {
    //         new LineSeries<ObservableValue>
    //         {
    //             Values = WinterHeatDemandData,
    //             // DataLabels = true,
    //             // LabelPoint = point => point.Value.ToString("N2"),
    //             // ScalesXAt = 0,
    //             // ScalesYAt = 0
    //         }
    //     };
    // }





    private readonly Random _random = new();
    private readonly ObservableCollection<ObservableValue> WinterHeatDemandData;
    public ObservableCollection<ISeries> Series { get; set; }

    public ResultsViewModel()
    {
        // Use ObservableCollections to let the chart listen for changes (or any INotifyCollectionChanged). 
        WinterHeatDemandData = new ObservableCollection<ObservableValue>();

        foreach (var point in dataVisualizer.sourceData.WinterData)
        {
            if (point.HeatDemand.HasValue)
            {
                WinterHeatDemandData.Add(new ObservableValue(point.HeatDemand.Value));
            }
        }


        Series = new ObservableCollection<ISeries>
        {
            new LineSeries<ObservableValue>
            {
                Values = WinterHeatDemandData,
                Fill = null
            }
        };
    }
}

