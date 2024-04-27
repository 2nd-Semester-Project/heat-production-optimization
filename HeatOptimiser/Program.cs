namespace HeatOptimiser
{
    class Program {
        public static void Main()
        {
            AssetManager.AddUnit("GB", "none", 5.0, 0, 1.1, 500, 215);
            AssetManager.AddUnit("OB", "none", 4.0, 0, 1.2, 700, 265);

            string startDateStr = "09/02/2023";
            string endDateStr = "14/02/2023";
            DateTime startDate = DateTime.ParseExact(startDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(endDateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            Schedule optimisedData = Optimiser.Optimise(startDate, endDate);

            ResultsDataManager.filePath = "data/resultdata.csv";
            ResultsDataManager.Save(optimisedData);

            Schedule readData = ResultsDataManager.Load(
            DateOnly.ParseExact("10/02/2023", "dd/MM/yyyy"),
            DateOnly.ParseExact("13/02/2023", "dd/MM/yyyy")
            );

            // Example on visualizing the data
            
            // foreach (ScheduleHour hour in readData.schedule)
            // {
            //     Console.WriteLine(hour.Hour);
            //     foreach (ProductionAsset asset in hour.Assets!)
            //     {
            //         Console.Write($"{asset.Name} ");
            //     }
            //     Console.WriteLine();
            //     foreach (double demand in hour.Demands!)
            //     {
            //         Console.Write($"{demand} ");
            //     }
            //     Console.WriteLine("\n");
            // }

            ResultsDataManager.Remove(DateOnly.ParseExact("10/02/2023", "dd/MM/yyyy"), DateOnly.ParseExact("11/02/2023", "dd/MM/yyyy"));

            //new TextBasedUI().Interface();
        }
    }
}