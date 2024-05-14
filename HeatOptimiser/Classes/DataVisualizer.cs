using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using LiveChartsCore.Defaults;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HeatOptimiser
{

    public static class DataVisualizer
    {
        public static SourceData sourceData = new SourceData(); 

        public static readonly ObservableCollection<DateTimePoint>? HeatDemandData = new ObservableCollection<DateTimePoint>();
        public static void AccessSummerData()
        {
            // Accessing the summer data
            foreach (var point in sourceData.LoadedData)
            {
                if (point.TimeFrom.HasValue) // Ensuring the time is not null
                {
                    HeatDemandData?.Add(new DateTimePoint(point.TimeFrom.Value, point.HeatDemand));
                    // SummerElectricityPrices.Add(point.ElectricityPrice);
                }
            }
        }
    }
}