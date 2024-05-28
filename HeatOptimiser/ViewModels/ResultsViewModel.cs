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
    public ISeries[] Series { get; set; }
    public Axis[] XAxes { get; set; }

    public ResultsViewModel()
    {
        
        Schedule results = ResultsDataManager.LoadAll();
        List<List<double>> demandsList = new();
        List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
        int assetCount = assets.Count;
        foreach (ProductionAsset asset in assets)
        {
            List<double> demands = new();
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
        List<string> AssetNames = new();
        foreach (ProductionAsset asset in assets)
        {
            AssetNames.Add(asset.Name);
        }

        Series = demandsList.Select(demands => new StackedStepAreaSeries<double>
        {
            Values = demands,
            Name = AssetNames[demandsList.IndexOf(demands)],
            Stroke = null
        }).ToArray();
    
        List<DateTime> hours = new();
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