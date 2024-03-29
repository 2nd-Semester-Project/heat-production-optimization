﻿namespace HeatOptimiser
{
    class Program {
        public static void Main()
        {
            AssetManager assets = new();
            assets.AddUnit("GB", "none", 5.0, 0, 1.1, 500, 215);
            assets.AddUnit("OB", "none", 4.0, 0, 1.2, 700, 265);

            SourceDataManager dataManager = new();
            Optimiser optimiser = new(dataManager, assets);
            string startDateStr = "12/02/2023";
            string endDateStr = "25/02/2023";
            DateTime startDate = DateTime.ParseExact(startDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(endDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            Schedule optimisedData = optimiser.Optimise(startDate, endDate);


            // Example on visualizing the data
            
            // foreach (ScheduleHour hour in optimisedData.schedule)
            // {
            //     Console.WriteLine(hour.Hour);
            //     foreach (ProductionAsset asset in hour.Assets)
            //     {
            //         Console.Write($"{asset.Name} ");
            //     }
            //     Console.WriteLine();
            //     foreach (double demand in hour.Demands)
            //     {
            //         Console.Write($"{demand} ");
            //     }
            //     Console.WriteLine("\n");
            // }

            new TextBasedUI().Interface();
        }
    }
}