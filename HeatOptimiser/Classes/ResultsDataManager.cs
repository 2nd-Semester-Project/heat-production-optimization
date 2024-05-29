using System.Text;
using System.IO;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HeatOptimiser
{
    public class ResultsDataManager
    {
        public static readonly string filePath = "data/resultdata.csv";
        private static Schedule? _resultsSchedule;
        public static Schedule ResultsSchedule
        {
            get
            {
                if (_resultsSchedule == null)
                {
                    _resultsSchedule = Load();
                }
                else
                {
                    Schedule AnotherSchedule = Load();
                    
                    if (_resultsSchedule != AnotherSchedule && AnotherSchedule.schedule.Count > 0)
                    {
                        _resultsSchedule = AnotherSchedule;
                    }
                }
                return _resultsSchedule;
            }
            set {
                _resultsSchedule = value;
            }
        }
        public static void Save(Schedule schedule)
        {
            var csv = new StringBuilder();
            foreach (ScheduleHour hour in schedule.schedule)
            {
                string l = hour.Hour!.Value.ToString("dd/MM/yyyy HH:mm");
                string newLine = $"{l}, ";
                double? producedHeat = 0;
                double? producedElectricity = 0;
                double? consumedElectricity = 0;
                double? productionCosts = 0;
                double? energyConsumption = 0;
                double? producedCarbonDioxide = 0;
                
                for (int i = 0; i < hour.Assets!.Count; i++)
                {
                    newLine += $"{hour.Assets[i].ID}/";
                    producedHeat += hour.Assets[i].Heat * hour.Demands![i];
                    if (hour.Assets[i].Electricity > 0)
                        producedElectricity += hour.Assets[i].Electricity * hour.Demands[i];
                    else
                        consumedElectricity += hour.Assets[i].Electricity * hour.Demands[i];
                    productionCosts += hour.Assets[i].Cost * hour.Demands[i];
                    energyConsumption += hour.Assets[i].Energy * hour.Demands[i];
                    producedCarbonDioxide += hour.Assets[i].CarbonDioxide * hour.Demands[i];
                }
                newLine = newLine.TrimEnd('/') + ", ";

                for (int i = 0; i < hour.Demands!.Count; i++)
                {
                    newLine += $"{hour.Demands[i]}/";
                }
                newLine = newLine.TrimEnd('/') + ", ";

                newLine += $"{producedElectricity}, {consumedElectricity}, {productionCosts}, {energyConsumption}, {producedCarbonDioxide}";
                csv.AppendLine(newLine);
            }

            File.WriteAllText(filePath, csv.ToString());
        }
        public static void Remove(DateOnly dateFrom, DateOnly dateTo)
        {
            List<string> lines = File.ReadAllLines(filePath).ToList();
            List<int> removableIndexes = new List<int>();
            int counter = 0;
            foreach (string line in lines)
            {
                if (DateTime.ParseExact(line.Split(',')[0], "dd/MM/yyyy HH.mm", CultureInfo.InvariantCulture) == dateFrom.ToDateTime(TimeOnly.Parse("00:00")) ||
                    DateTime.ParseExact(line.Split(',')[0], "dd/MM/yyyy HH.mm", CultureInfo.InvariantCulture) == dateTo.ToDateTime(TimeOnly.Parse("23:00")))
                {
                    removableIndexes.Add(counter);
                }
                counter++;
            }
            List<string> newLines = new List<string>();
            for (int i = 0; i < lines.Count; i++)
            {
                if (!removableIndexes.Contains(i))
                {
                    newLines.Add(lines[i]);
                }
            }

            File.WriteAllLines(filePath, newLines);
        }
        public static Schedule Load()
        {
            List<ScheduleHour> schedule = [];

            CultureInfo provider = CultureInfo.InvariantCulture;

            foreach (string line in File.ReadAllLines(filePath))
            {
                List<string> assetIDs = line.Split(", ")[1].Split("/").ToList();
                ObservableCollection<ProductionAsset> assets = [];
                foreach (string assetIDString in assetIDs)
                {
                    Guid assetID = Guid.Parse(assetIDString);
                    Console.WriteLine(AssetManager.GetAllUnits().Count);
                    assets.Add(AssetManager.SearchUnits(assetID)[0]);
                }

                List<string> demandStrings = line.Split(", ")[2].Split("/").ToList();
                ObservableCollection<double> demands = [];
                foreach (string demandString in demandStrings)
                {
                    demands.Add(double.Parse(demandString));
                }

                schedule.Add(
                    new()
                    {
                        Hour = DateTime.ParseExact(line.Split(", ")[0], "dd/MM/yyyy HH:mm", provider),
                        Assets = assets,
                        Demands = demands
                    }
                );
            }
            
            Schedule loadedSchedule = new Schedule((DateTime)schedule[0].Hour!, (DateTime)schedule[^1].Hour!);
        
            foreach(ScheduleHour hour in schedule)
            {
                loadedSchedule.AddHour(hour.Hour, hour.Assets!, hour.Demands!);
            }

            return loadedSchedule;
        }
        public static Schedule Load2()
        {

            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = false
            };

            using var streamReader = File.OpenText(filePath);
            using var csvReader = new CsvReader(streamReader, csvConfig);

            var lines = File.ReadLines(filePath).Skip(1);
            var dates = lines.Select(line => DateTime.Parse(line.Split(',')[0]));

            DateTime dateFrom = dates.Min();
            DateTime dateTo = dates.Max();

            bool reading = false;
            Schedule schedule = new(dateFrom, dateTo);

        
            while (csvReader.Read())
            {
                List<string> line = [];
                for (int i = 0; csvReader.TryGetField<string>(i, out string value); i++)
                {
                    line.Add(value);
                }
                if((DateTime.ParseExact(line[0], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) == dateFrom) || (DateTime.ParseExact(line[0], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) == dateTo))
                    reading = !reading;
                if(reading)
                {
                    DateTime hour = DateTime.ParseExact(line[0], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                
                    ObservableCollection<ProductionAsset> assets = [];
                    ObservableCollection<double> demands = [];
                    foreach(string assetID in line[1].Trim().Split('/'))
                    {
                        var unit = AssetManager.GetAllUnits().FirstOrDefault(x => x.ID.ToString() == assetID);
                        if (unit != null)
                        {
                            assets.Add(unit);
                        }
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
