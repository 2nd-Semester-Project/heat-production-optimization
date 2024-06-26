using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace HeatOptimiser
{
    public enum OptimisationChoice
    {
        Cost,
        Emissions
    }
    public class Schedule(DateTime start, DateTime end)
    {
        public DateTime startDate = start;
        public DateTime endDate = end;
        public List<ScheduleHour> schedule = [];

        // Adds a new hour to the schedule with the specified assets and demands.
        public void AddHour(DateTime? dateTime, ObservableCollection<ProductionAsset> assets, ObservableCollection<double> demands)
        {
            schedule.Add(new ScheduleHour
            {
                Hour = dateTime,
                Assets = assets,
                Demands = demands
            });
            if (dateTime > endDate)
            {
                endDate = (DateTime)dateTime;
            }
            if (dateTime < startDate)
            {
                startDate = (DateTime)dateTime;
            }
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
        // Optimises the schedule by only looking at net cost of each production asset.
        public static Schedule NetOptimise(DateTime startDate, DateTime endDate)
        {
            Schedule schedule = new(startDate, endDate);
            ObservableCollection<ProductionAsset> assets = AssetManager.GetSelectedUnits();

            List<ProductionAsset> orderedAssets = assets.OrderBy(x => x.Cost).ToList();

            foreach (SourceDataPoint hour in SourceDataManager.GetDataInRange(startDate, endDate))
            {
                double producedHeat = 0;
                int index = 0;
                ObservableCollection<ProductionAsset> assetsUsed = [];
                ObservableCollection<double> assetDemands = [];
                while (producedHeat < hour.HeatDemand && index < orderedAssets.Count)
                {
                    assetsUsed.Add(orderedAssets[index]);
                    double assetUsed;
                    if ((double)orderedAssets[index].Heat! > (hour.HeatDemand - producedHeat))
                    {
                        assetUsed = (double)hour.HeatDemand-producedHeat;
                    }
                    else
                    {
                        assetUsed = (double)orderedAssets[index].Heat!;
                    }
                    assetDemands.Add(assetUsed);
                    producedHeat += assetUsed;
                    index += 1;
                }
                schedule.AddHour(hour.TimeFrom, assetsUsed, assetDemands);
            }
            return schedule;
        }
        // Optimises the schedule based on the specified criteria (total cost/CO2 emissions)
        public static Schedule Optimise(DateTime startDate, DateTime endDate, OptimisationChoice optimisationChoice)
        {
            Schedule schedule = new(startDate, endDate);

            ObservableCollection<ProductionAsset> assets = AssetManager.GetSelectedUnits();
            if(assets.Count != 0)
            {
                if (optimisationChoice == OptimisationChoice.Cost)
                {
                    Dictionary<ProductionAsset, double?> netCosts = [];

                    for (int i = 0; i < assets.Count; i++)
                    {
                        netCosts.Add(assets[i], assets[i].Cost);
                    }

                    foreach (SourceDataPoint hour in SourceDataManager.GetDataInRange(startDate, endDate))
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
                        while (producedHeat < hour.HeatDemand && index < sortedCosts.Count)
                        {
                            ProductionAsset currentAsset = sortedCosts.Keys.ToList()[index];
                            assetsUsed.Add(currentAsset);
                            if (currentAsset.Heat > (hour.HeatDemand - producedHeat))
                            {
                                assetDemands.Add(hour.HeatDemand.Value - producedHeat);
                                producedHeat = hour.HeatDemand.Value;
                            }
                            else
                            {
                                assetDemands.Add(currentAsset.Heat!.Value);
                                producedHeat += currentAsset.Heat!.Value;
                            }
                            index += 1;
                        }
                        //Check if enough heat has been produced
                        schedule.AddHour(hour.TimeFrom, assetsUsed, assetDemands);
                    }
                }
                
                else if (optimisationChoice == OptimisationChoice.Emissions)
                {
                    Dictionary<ProductionAsset, double?> emissions = [];

                    for (int i = 0; i < assets.Count; i++)
                    {
                        emissions.Add(assets[i], assets[i].CarbonDioxide);
                    }

                    foreach (SourceDataPoint hour in SourceDataManager.GetDataInRange(startDate, endDate))
                    {
                        Dictionary<ProductionAsset, double?> sortedEmissions = emissions.OrderBy(x => x.Value).ToDictionary();
                        double producedHeat = 0;
                        int index = 0;
                        ObservableCollection<ProductionAsset> assetsUsed = [];
                        ObservableCollection<double> assetDemands = [];
                        while (producedHeat < hour.HeatDemand && index < sortedEmissions.Count)
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
            else
            {
                Console.WriteLine("No units selected!");
            }
            return schedule;
        }
    }
}