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
    private readonly Random _random = new();
    private readonly ObservableCollection<ObservableValue> WinterHeatDemandData;
    public ObservableCollection<ISeries> Series { get; set; }

    private readonly ObservableCollection<ObservableValue> SummerHeatDemandData;

    public ResultsViewModel()
    {
        // Use ObservableCollections to let the chart listen for changes (or any INotifyCollectionChanged). 
        WinterHeatDemandData = new ObservableCollection<ObservableValue>();
        SummerHeatDemandData = new ObservableCollection<ObservableValue>();

        foreach (var point in dataVisualizer.sourceData.WinterData)
        {
            if (point.HeatDemand.HasValue)
            {
                WinterHeatDemandData.Add(new ObservableValue(point.HeatDemand.Value));
            }
        }

        foreach (var point in dataVisualizer.sourceData.SummerData)
        {
            if (point.HeatDemand.HasValue)
            {
                SummerHeatDemandData.Add(new ObservableValue(point.HeatDemand.Value));
            }
        }

        Series = new ObservableCollection<ISeries>
    {
        new LineSeries<ObservableValue>
        {
            Values = WinterHeatDemandData,
            Fill = null
        },
        // new LineSeries<ObservableValue>
        // {
        //     Values = SummerHeatDemandData,
        //     Fill = null
        // }
    };
    }
}



