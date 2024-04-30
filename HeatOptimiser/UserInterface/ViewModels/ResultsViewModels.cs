using System;
using ReactiveUI;
using System.Reactive;
using CommunityToolkit.Mvvm;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using HeatOptimiser;

namespace UserInterface.ViewModels;


public class ResultsViewModel : ViewModelBase
{
  public class ViewModel
    {
        public ISeries[] Series { get; set; } 
            = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 2, 1, 3, 5, 3, 4, 6 },
                    Fill = null
                }
            };
    }
}