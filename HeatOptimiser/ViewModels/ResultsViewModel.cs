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
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace UserInterface.ViewModels;

public class ResultsViewModel : ViewModelBase
{
    private readonly ObservableCollection<DateTimePoint> HeatDemandData;
    public ObservableCollection<ISeries> Series { get; set; }

    public ResultsViewModel()
    {
        // Use ObservableCollections to let the chart listen for changes (or any INotifyCollectionChanged). 
        HeatDemandData = new ObservableCollection<DateTimePoint>();

        foreach (var point in DataVisualizer.sourceData.LoadedData)
        {
            if (point.HeatDemand.HasValue)
            {
                HeatDemandData.Add(new DateTimePoint(point.TimeFrom!.Value, point.HeatDemand.Value));
            }
        }

        Schedule schedule = ResultsDataManager.LoadAll();
        foreach (var hour in schedule.schedule)
        {
            foreach (var asset in hour.Assets!)
            {
                HeatDemandData.Add(new DateTimePoint(hour.Hour!.Value, asset.Heat));
            }
        }


        Series = new ObservableCollection<ISeries>
    {
        new LineSeries<DateTimePoint>
        {
            Values = HeatDemandData,
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



