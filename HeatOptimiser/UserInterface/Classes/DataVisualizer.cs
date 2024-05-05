using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using LiveChartsCore.Defaults;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HeatOptimiser
{

    public class DataVisualizer
{
    public SourceData sourceData = new SourceData(); 

    // public List<DateTime> SummerTimes = new List<DateTime>(); 
    // public List<double?> SummerHeatDemands = new List<double?>(); 
    // public List<double?> SummerElectricityPrices = new List<double?>(); 
    // public List<DateTime> WinterTimes = new List<DateTime>(); 
    // public List<double?> WinterHeatDemands = new List<double?>(); 
    // public List<double?> WinterElectricityPrices = new List<double?>(); 

    public readonly ObservableCollection<DateTimePoint>? WinterHeatDemandData = new ObservableCollection<DateTimePoint>();
    public readonly ObservableCollection<DateTimePoint>? SummerHeatDemandData = new ObservableCollection<DateTimePoint>();

    public void AccessSummerData()
    {
        // Accessing the summer data
        foreach (var point in sourceData.SummerData)
        {
            if (point.TimeFrom.HasValue) // Ensuring the time is not null
            {
                SummerHeatDemandData!.Add(new DateTimePoint(point.TimeFrom.Value, point.HeatDemand));
                // SummerElectricityPrices.Add(point.ElectricityPrice);
            }
        }
    }

      public void AccessWinterData()
    {
        // Accessing the summer data
        foreach (var point in sourceData.WinterData)
        {
            if (point.TimeFrom.HasValue) // Ensuring the time is not null
            {
                WinterHeatDemandData!.Add(new DateTimePoint(point.TimeFrom.Value, point.HeatDemand));
                // SummerElectricityPrices.Add(point.ElectricityPrice);
            }
        }
    }

//     public void AccessWinterData()
//     {
//         // Accessing the winter data
//         foreach (var point in sourceData.WinterData)
//         {
//             if (point.TimeFrom.HasValue) // Ensuring the time is not null
//             {
//                 WinterTimes.Add(point.TimeFrom.Value);
//                 WinterHeatDemands.Add(point.HeatDemand);
//                 WinterElectricityPrices.Add(point.ElectricityPrice);
//             }
//         }
//     }
     }
}
