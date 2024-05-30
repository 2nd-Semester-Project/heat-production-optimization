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
                    newLine += $"{hour.Assets[i].Name}/";
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
            List<string> newLines = [];
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
                List<string> assetNames = line.Split(", ")[1].Split("/").ToList();
                ObservableCollection<ProductionAsset> assets = [];
                foreach (string assetName in assetNames)
                {
                    assets.Add(AssetManager.SearchUnits(assetName)[0]);
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
            if (schedule.Count != 0)
            {
                Schedule loadedSchedule = new Schedule((DateTime)schedule[0].Hour!, (DateTime)schedule[^1].Hour!);
            
                foreach(ScheduleHour hour in schedule)
                {
                    loadedSchedule.AddHour(hour.Hour, hour.Assets!, hour.Demands!);
                }

                return loadedSchedule;
            }
            else
            {
                // TODO: return empty schedule
                return new Schedule(DateTime.Now, DateTime.Now);
                //return new Schedule((DateTime)schedule[0].Hour!, (DateTime)schedule[^1].Hour!); // just a placeholder, this has to be changed to return empty schedule
            }
        }
    }
}
