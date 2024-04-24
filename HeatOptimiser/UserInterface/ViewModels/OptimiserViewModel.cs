using HeatOptimiser;
using ReactiveUI;
using System;
using Avalonia.Controls;
using System.Reactive;
using System.Data.SqlTypes;

namespace UserInterface.ViewModels;

public class OptimiserViewModel : ViewModelBase
{
    
    
    public SourceData sourcedata;
    public Optimiser optimiser;
    
    public DateTime _startingDate = new DateTime(2023,7,12,0,0,0);
    public DateTime _endingDate = new DateTime(2023,7,13,0,0,0);
    public DateTime StartingDate
    {
        get => _startingDate;
        set => this.RaiseAndSetIfChanged(ref _startingDate, value);
    }
    public DateTime EndingDate
    {
        get => _endingDate;
        set => this.RaiseAndSetIfChanged(ref _endingDate, value);
    }
    public ReactiveCommand<Unit, Unit> OptimiseCommand { get; }

    public void Optimise(DateTime start, DateTime end )//also add which category to optimise from later
        {
            Schedule schedule = optimiser.Optimise(start, end);
            Console.WriteLine("Optimised Schedule:");
            foreach (var hour in schedule.schedule)
            {
                Console.WriteLine($"Hour: {hour.Hour}, Assets: {string.Join(",", hour.Assets!)}, Demands: {string.Join(",", hour.Demands!)}");
            }
                }
            //return schedule;
            
        
        


    



    public OptimiserViewModel()
    {
        //OptimiseCommand=ReactiveCommand.Create(()=> Optimise(StartingDate,EndingDate)); needs to be fixed
    }
}