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
    private PlotModel _plotModel;
    public PlotModel PlotModel
    {
        get => _plotModel;
        set => this.RaiseAndSetIfChanged(ref _plotModel, value);
    }

    public ReactiveCommand<Unit, Unit> LoadResultsCommand { get; }

    public ResultsViewModel()
    {
        _plotModel = new PlotModel { Title = "Energy Savings" };
        LoadResultsCommand = ReactiveCommand.Create(LoadResults);
        LoadResults();
    }

    private void LoadResults()
    {
        var lineSeries = new LineSeries
        {
            Title = "Energy",
            MarkerType = MarkerType.Circle
        };

        lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(-1)), 100));
        lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), 150));

        _plotModel.Series.Add(lineSeries);
        _plotModel.InvalidatePlot(true);
    }
}