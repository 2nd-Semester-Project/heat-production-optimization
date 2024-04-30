using System.Security.Cryptography;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;

namespace HeatOptimiser
{
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
        public void AddHour(DateTime? dateTime, ObservableCollection<ProductionAsset> assets, List<double> demands)
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
        public List<double>? Demands { get; set; }
    }
    public class Optimiser: IOptimiserModule
    {
        private ISourceDataManager sd;
        private IAssetManager am;
        public Optimiser(ISourceDataManager sourceDataManager, IAssetManager assetManager)
        {
            sd = sourceDataManager;
            am = assetManager;
        }

        public Schedule Optimise(DateTime startDate, DateTime endDate)
        {
            SourceData data = new();
            Schedule schedule = new(startDate, endDate);

            List<ProductionAsset> assets = am.GetAllUnits();

            for (int i = 0; i < assets.Count; i++)
            {
                for (int j = 0; j > assets.Count - i; i++)
                {
                    if (assets[i].Cost > assets[j].Cost)
                    {
                        (assets[i], assets[j]) = (assets[j], assets[i]);
                    }
                }
            }

            foreach (SourceDataPoint hour in sd.GetDataInRange(data, startDate, endDate))
            {
                double producedHeat = 0;
                int index = 0;
                List<ProductionAsset> assetsUsed = [];
                List<double> assetDemands = [];
                while (producedHeat < hour.HeatDemand)
                {
                    assetsUsed.Add(assets[index]);
                    assetDemands.Add(assets[index].Heat!.Value);
                    producedHeat += assets[index].Heat!.Value;
                    index += 1;
                }
                schedule.AddHour(hour.TimeFrom, assetsUsed, assetDemands);
            }
            return schedule;
        }

        public Schedule Optimise2(DateTime startDate, DateTime endDate)
        {
            SourceData data = new();
            Schedule schedule = new(startDate, endDate);

            List<ProductionAsset> assets = am.GetAllUnits();

            Dictionary<ProductionAsset, double?> netCosts = new();

            for (int i = 0; i < assets.Count; i++)
            {
                netCosts.Add(assets[i], assets[i].Cost);
            }

            foreach (SourceDataPoint hour in sd.GetDataInRange(data, startDate, endDate))
            {
                Dictionary<ProductionAsset, double?> costs = new(netCosts);
                foreach(ProductionAsset asset in costs.Keys)
                {
                    costs[asset] -= asset.Electricity / asset.Heat * hour.ElectricityPrice;
                }

                Dictionary<ProductionAsset, double?> sortedCosts = costs.OrderBy(x => x.Value).ToDictionary();
                double producedHeat = 0;
                int index = 0;
                List<ProductionAsset> assetsUsed = [];
                List<double> assetDemands = [];
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
            return schedule;
        }
    }
}