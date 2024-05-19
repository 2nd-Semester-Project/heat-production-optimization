using System.Security.Cryptography;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;

namespace HeatOptimiser
{
    public enum OptimisationChoice
    {
        Cost,
        Emissions
    }
    public class Schedule
    {
        public DateTime startDate;
        public DateTime endDate;
        public List<ScheduleHour> schedule;
        public Schedule(DateTime start, DateTime end)
        {
            startDate = start;
            endDate = end;
            schedule = [];
        }
        public void AddHour(DateTime? dateTime, ObservableCollection<ProductionAsset> assets, ObservableCollection<double> demands)
        {
            schedule.Add(new ScheduleHour
            {
                Hour = dateTime,
                Assets = assets,
                Demands = demands
            });
        }
    }
    public class ScheduleHour
    {
        public DateTime? Hour { get; set; }
        public ObservableCollection<ProductionAsset>? Assets { get; set; }
        public ObservableCollection<double>? Demands { get; set; }
    }
    public static class Optimiser
    {
        public static Schedule Optimise(DateTime startDate, DateTime endDate, OptimisationChoice optimisationChoice)
        {
            SourceData data = new();
            Schedule schedule = new(startDate, endDate);

            ObservableCollection<ProductionAsset> assets = AssetManager.GetAllUnits();

            if (optimisationChoice == OptimisationChoice.Cost)
            {
                Dictionary<ProductionAsset, double?> netCosts = new();

                for (int i = 0; i < assets.Count; i++)
                {
                    netCosts.Add(assets[i], assets[i].Cost);
                }

                foreach (SourceDataPoint hour in SourceDataManager.GetDataInRange(data, startDate, endDate))
                {
                    Dictionary<ProductionAsset, double?> costs = new(netCosts);
                    foreach(ProductionAsset asset in costs.Keys)
                    {
                        costs[asset] -= asset.Electricity / asset.Heat * hour.ElectricityPrice;
                    }

                    Dictionary<ProductionAsset, double?> sortedCosts = costs.OrderBy(x => x.Value).ToDictionary();
                    double producedHeat = 0;
                    int index = 0;
                    ObservableCollection<ProductionAsset> assetsUsed = [];
                    ObservableCollection<double> assetDemands = [];
                    while (producedHeat < hour.HeatDemand)
                    {
                        assetsUsed.Add(sortedCosts.Keys.ToList()[index]);
                        if (sortedCosts.Keys.ToList()[index].Heat > (hour.HeatDemand - producedHeat))
                        {
                            assetDemands.Add(hour.HeatDemand.Value - producedHeat);
                            producedHeat = hour.HeatDemand.Value;
                        }
                        else
                        {
                            assetDemands.Add(sortedCosts.Keys.ToList()[index].Heat!.Value);
                            producedHeat += sortedCosts.Keys.ToList()[index].Heat!.Value;
                        }
                        index += 1;
                    }
                    schedule.AddHour(hour.TimeFrom, assetsUsed, assetDemands);
                }
            }
            
            else if (optimisationChoice == OptimisationChoice.Emissions)
            {
                Dictionary<ProductionAsset, double?> emissions = new();

                for (int i = 0; i < assets.Count; i++)
                {
                    emissions.Add(assets[i], assets[i].CarbonDioxide);
                }

                foreach (SourceDataPoint hour in SourceDataManager.GetDataInRange(data, startDate, endDate))
                {
                    Dictionary<ProductionAsset, double?> sortedEmissions = emissions.OrderBy(x => x.Value).ToDictionary();
                    double producedHeat = 0;
                    int index = 0;
                    ObservableCollection<ProductionAsset> assetsUsed = [];
                    ObservableCollection<double> assetDemands = [];
                    while (producedHeat < hour.HeatDemand)
                    {
                        assetsUsed.Add(sortedEmissions.Keys.ToList()[index]);
                        if (sortedEmissions.Keys.ToList()[index].Heat > (hour.HeatDemand - producedHeat))
                        {
                            assetDemands.Add(hour.HeatDemand.Value - producedHeat);
                            producedHeat = hour.HeatDemand.Value;
                        }
                        else
                        {
                            assetDemands.Add(sortedEmissions.Keys.ToList()[index].Heat!.Value);
                            producedHeat += sortedEmissions.Keys.ToList()[index].Heat!.Value;
                        }
                        index += 1;
                    }
                    schedule.AddHour(hour.TimeFrom, assetsUsed, assetDemands);
                }
            }
            return schedule;
        }
    }
}