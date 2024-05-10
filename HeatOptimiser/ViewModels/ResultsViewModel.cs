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
    private readonly ObservableCollection<DateTimePoint> WinterHeatDemandData;
    public ObservableCollection<ISeries> Series { get; set; }

    private readonly ObservableCollection<DateTimePoint> SummerHeatDemandData;

    public ResultsViewModel()
    {
        // Use ObservableCollections to let the chart listen for changes (or any INotifyCollectionChanged). 
        WinterHeatDemandData = new ObservableCollection<DateTimePoint>();
        SummerHeatDemandData = new ObservableCollection<DateTimePoint>();

        foreach (var point in DataVisualizer.sourceData.WinterData)
        {
            if (point.HeatDemand.HasValue)
            {
                WinterHeatDemandData.Add(new DateTimePoint(point.TimeFrom.Value, point.HeatDemand.Value));
            }
        }

        foreach (var point in DataVisualizer.sourceData.SummerData)
        {
            if (point.HeatDemand.HasValue)
            {
                SummerHeatDemandData.Add(new DateTimePoint(point.TimeFrom.Value, point.HeatDemand.Value));
            }
        }

        Series = new ObservableCollection<ISeries>
    {
        new LineSeries<DateTimePoint>
        {
            Values = WinterHeatDemandData,
            Name = "Heat Demand",
            Fill = null,
            GeometryStroke = null,
            GeometryFill = null,
            LineSmoothness = 1
        },
        // new LineSeries<ObservableValue>
        // {
        //     Values = SummerHeatDemandData,
        //     Fill = null
        // }
    };
    
    
    }
    public Axis[] XAxes { get; set; } =
    {
        new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMMM dd HH:mm"))
    };
}



