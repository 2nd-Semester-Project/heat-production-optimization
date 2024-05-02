using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HeatOptimiser
{

    public class DataVisualizer
{
    public SourceData sourceData = new SourceData(); // This already loads your summer and winter data.

    public List<DateTime> Times = new List<DateTime>(); // For X-axis
    public List<double?> HeatDemands = new List<double?>(); // For Y-axis (heat demand)
    public List<double?> ElectricityPrices = new List<double?>(); // For Y-axis (electricity price)

    public void AccessSummerData()
    {
        // Accessing the summer data
        foreach (var point in sourceData.SummerData)
        {
            if (point.TimeFrom.HasValue) // Ensuring the time is not null
            {
                Times.Add(point.TimeFrom.Value);
                HeatDemands.Add(point.HeatDemand);
                ElectricityPrices.Add(point.ElectricityPrice);
            }
        }
    }

    public void AccessWinterData()
    {
        // Accessing the winter data
        foreach (var point in sourceData.WinterData)
        {
            if (point.TimeFrom.HasValue) // Ensuring the time is not null
            {
                Times.Add(point.TimeFrom.Value);
                HeatDemands.Add(point.HeatDemand);
                ElectricityPrices.Add(point.ElectricityPrice);
            }
        }
    }
}
}
