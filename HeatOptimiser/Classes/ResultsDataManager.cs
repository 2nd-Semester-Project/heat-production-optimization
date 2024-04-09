using System.Text;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace HeatOptimiser
{
    public class ResultsDataManager : IResultsDataManager
    {
        private string filePath;
        private AssetManager am;
        public ResultsDataManager(string passedFilePath, AssetManager assetManager)
        {
            filePath = passedFilePath;
            am = assetManager;
        }
        public void Save(Schedule schedule, string? fileName)
        {
            var csv = new StringBuilder();
            foreach (ScheduleHour hour in schedule.schedule)
            {
                string l = hour.Hour.Value.ToString("dd/MM/yyyy HH:mm");
                string newLine = $"{l}, ";
                double? producedHeat = 0;
                double? producedElectricity = 0;
                double? consumedElectricity = 0;
                double? productionCosts = 0;
                double? energyConsumption = 0;
                double? producedCarbonDioxide = 0;
                
                for (int i = 0; i < hour.Assets.Count; i++)
                {
                    newLine += $"{hour.Assets[i].ID}/";
                    producedHeat += hour.Assets[i].Heat * hour.Demands[i];
                    if (hour.Assets[i].Electricity > 0)
                        producedElectricity += hour.Assets[i].Electricity * hour.Demands[i];
                    else
                        consumedElectricity += hour.Assets[i].Electricity * hour.Demands[i];
                    productionCosts += hour.Assets[i].Cost * hour.Demands[i];
                    energyConsumption += hour.Assets[i].Energy * hour.Demands[i];
                    producedCarbonDioxide += hour.Assets[i].CarbonDioxide * hour.Demands[i];
                }
                newLine = newLine.TrimEnd('/') + ", ";

                for (int i = 0; i < hour.Demands.Count; i++)
                {
                    newLine += $"{hour.Demands[i]}/";
                }
                newLine = newLine.TrimEnd('/') + ", ";

                newLine += $"{producedElectricity}, {consumedElectricity}, {productionCosts}, {energyConsumption}, {producedCarbonDioxide}";
                csv.AppendLine(newLine);
            }
            File.WriteAllText(filePath, csv.ToString());
        }
       public void Remove(DateOnly dateFrom, DateOnly dateTo)
        {
        List<string> lines = File.ReadAllLines(filePath).ToList();
        List<int> removableIndexes = new List<int>(); // Initialize the list explicitly
        int counter = 0;
        foreach (string line in lines)
        {
            if (DateTime.ParseExact(line.Split(',')[0], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) == dateFrom.ToDateTime(TimeOnly.Parse("00:00")) ||
            DateTime.ParseExact(line.Split(',')[0], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) == dateTo.ToDateTime(TimeOnly.Parse("23:00")))
            {
                removableIndexes.Add(counter);
            }
            counter++;
        }
         // Check if removableIndexes has at least two elements before accessing them
        if (removableIndexes.Count >= 2)
        {
            List<string> newLines = lines.GetRange(0, removableIndexes[0]);
            newLines.AddRange(lines.GetRange(removableIndexes[1] + 1, lines.Count - (removableIndexes[1] + 1)));

            File.WriteAllLines(filePath, newLines);
        }
        else
        {
            // Handle the case where removableIndexes doesn't have enough elements
            Console.WriteLine("Unable to remove lines: Not enough indexes found.");
        }
        }

        public Schedule Load(DateOnly dateFrom, DateOnly dateTo, string fileNameToLoad)
        {
            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = false
            };

            using var streamReader = File.OpenText(fileNameToLoad);
            using var csvReader = new CsvReader(streamReader, csvConfig);

            bool reading = false;
            Schedule schedule = new Schedule(dateFrom.ToDateTime(TimeOnly.Parse("00:00")), dateTo.ToDateTime(TimeOnly.Parse("00:00")));

        
            while (csvReader.Read())
            {
                List<string> line = new List<string>();
                for (int i = 0; csvReader.TryGetField<string>(i, out string value); i++)
                {
                    line.Add(value);
                }
                if((DateTime.ParseExact(line[0], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) == dateFrom.ToDateTime(TimeOnly.Parse("00:00"))) || (DateTime.ParseExact(line[0], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) == dateTo.ToDateTime(TimeOnly.Parse("00:00"))))
                        reading = !reading;
                if(reading)
                {
                    DateTime hour = DateTime.ParseExact(line[0], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    List<ProductionAsset> assets = [];
                    List<double> demands = [];
                    foreach(string assetID in line[1].Trim().Split('/'))
                    {
                        assets.Add(am.GetAllUnits().Find(x => x.ID.ToString() == assetID));
                    }
                    foreach(string demand in line[2].Split('/'))
                    {
                        demands.Add(Convert.ToDouble(demand));
                    }
                    schedule.AddHour(hour, assets, demands);
                }
            }
                
            return schedule;
        }
    }
}