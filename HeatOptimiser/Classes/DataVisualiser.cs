using System;
using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Collections.ObjectModel;
using SkiaSharp;
using System.Linq;
using System.Reactive.Linq;

namespace HeatOptimiser
{

    public static class DataVisualiser
    {
        readonly static List<SKColor> colors = [
            new SKColor(194, 36, 62),
            new SKColor(0, 92, 230),
            new SKColor(100, 87, 195)
        ];
        readonly static List<SKColor> alphaColors = [
            new SKColor(194, 36, 62, 40),
            new SKColor(0, 92, 230, 40),
            new SKColor(100, 87, 195, 40)
        ];
        public static ObservableCollection<ISeries> Series = [];
        public static Axis[] XAxes = [];
        public static Axis[] YAxes = [];
        // Generates chart data for source data (heat demand and electricity price)
        public static void VisualiseSourceData(List<List<DateTimePoint>> data, List<string> names)
        {
            List<SKColor> colors = [
                new SKColor(194, 36, 62),
                new SKColor(0, 92, 230)
            ];
            SourceDataManager.Series = [];
            SourceDataManager.XAxes = [];
            SourceDataManager.YAxes = new Axis[names.Count];
            if (data.Count != names.Count)
            {
                return;
            }
            for(int index = 0; index < data.Count; index++)
            {
                LineSeries<DateTimePoint> lineSeries = new()
                {
                    Values = data[index],
                    Name = names[index],
                    Fill = null,
                    GeometryStroke = null,
                    GeometryFill = null,
                    LineSmoothness = 1,
                    Stroke = new SolidColorPaint(colors[index%colors.Count])
                    {
                        StrokeThickness = 3
                    }
                };
                Axis axis = new()
                {
                    Name = names[index],
                    TextSize = 16,
                    NameTextSize = 18
                };
                if (index != 0)
                {
                    lineSeries.LineSmoothness = 2;
                    lineSeries.ScalesYAt = 1;
                    axis.Position = LiveChartsCore.Measure.AxisPosition.End;
                }
                SourceDataManager.Series.Add(
                    lineSeries
                );
                SourceDataManager.YAxes[index] = axis;
            }
            SourceDataManager.XAxes = [
                new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMMM dd HH:mm"))
            ];
        }
        // Generates chart data for production asset usage throughout the schedule.
        public static void VisualiseUsageData()
        {
            Schedule results = ResultsDataManager.Load();
            List<List<double>> demandsList = [];
            List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
            
            foreach (ProductionAsset asset in assets)
            {
                List<double> demands = [];
                foreach (ScheduleHour hour in results.schedule)
                {
                    if (hour.Assets!.Contains(asset))
                    {
                        demands.Add(hour.Demands![hour.Assets.IndexOf(asset)]);
                    }
                    else
                    {
                        demands.Add(0);
                    }
                }
                demandsList.Add(demands);
            }
            List<string> AssetNames = [];
            foreach (ProductionAsset asset in assets)
            {
                AssetNames.Add(asset.Name!);
            }

            Series = [];

            foreach(List<double> demands in demandsList)
            {
                StackedStepAreaSeries<double> ssaSeries = new()
                {
                    Values = demands,
                    Name = AssetNames[demandsList.IndexOf(demands)],
                    Stroke = null
                };
                Series.Add(ssaSeries);
            }
            
            List<double> demandValues = [];
            foreach(ScheduleHour hour in results.schedule)
            {
                double demand = (double)SourceDataManager.GetDataByDateTime(hour.Hour!.Value)!.HeatDemand!;
                demandValues.Add(demand);
            }
            LineSeries<double> demandSeries = new()
            {
                Values = demandValues,
                Name = "Total demand",
                LineSmoothness = 1,
                Fill =null,
                GeometryStroke = null,
                GeometryFill = null,
                Stroke = new SolidColorPaint(new SKColor(0, 0, 0))
                {
                    StrokeThickness = 3
                }
            };
            Series.Add(demandSeries);
            
            YAxes = [
                new()
                {
                    Name = "Usage (MWh)",
                    TextSize = 16,
                    NameTextSize = 18
                }
            ];
        
            List<DateTime> hours = [];
            foreach (ScheduleHour hour in results.schedule)
            {
                hours.Add(hour.Hour!.Value);
            }

            XAxes =
            [
                new Axis
                {
                    Labels = hours.Select(hour => hour.ToString("dd/MM/yyyy HH:mm")).ToArray()
                }
            ];
        }
        // Generates chart data for different costs throughout the schedule.
        public static void VisualiseCostsData()
        { 
            Schedule results = ResultsDataManager.Load();
            List<List<double>> costList = [[], [], []]; //Asset, Electricity, Total
            List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
            foreach (ScheduleHour hour in results.schedule)
            {
                List<double> hourlyCosts = [0, 0];
                foreach (ProductionAsset asset in assets)
                {
                    if (hour.Assets!.Contains(asset))
                    {
                        hourlyCosts[0] += hour.Demands![hour.Assets.IndexOf(asset)]*(double)asset.Cost!;
                        hourlyCosts[1] += hour.Demands![hour.Assets.IndexOf(asset)]*-(double)asset.Electricity!/(double)asset.Heat!*(double)SourceDataManager.GetDataByDateTime((DateTime)hour.Hour!)!.ElectricityPrice!;
                    }
                }
                costList[0].Add(hourlyCosts[0]);
                costList[1].Add(hourlyCosts[1]);
                costList[2].Add(hourlyCosts[0]+hourlyCosts[1]);
            }
            List<string> CostNames = ["Assets", "Electricity", "Total Cost"];

            Series = [];
            foreach(List<double> costs in costList)
            {
                LineSeries<double> lineSeries = new()
                {
                    Values = costs,
                    LineSmoothness = 1,
                    Name = CostNames[costList.IndexOf(costs)],
                    Stroke = null
                };
                Series.Add(lineSeries);
            }
            
            YAxes = [
                new()
                {
                    Name = "Cost (€)",
                    TextSize = 16,
                    NameTextSize = 18
                }
            ];
        
            List<DateTime> hours = [];
            foreach (ScheduleHour hour in results.schedule)
            {
                hours.Add(hour.Hour!.Value);
            }

            XAxes =
            [
                new Axis
                {
                    Labels = hours.Select(hour => hour.ToString("dd/MM/yyyy HH:mm")).ToArray()
                }
            ];
        }
        // Generates chart data for emission amount throughout the schedule.
        public static void VisualiseEmissionsData()
        {
            Schedule results = ResultsDataManager.Load();
            List<double> emissions = [];
            List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
            foreach (ScheduleHour hour in results.schedule)
            {
                double hourlyEmissions = 0;
                foreach (ProductionAsset asset in assets)
                {
                    if (hour.Assets!.Contains(asset))
                    {
                        hourlyEmissions += hour.Demands![hour.Assets.IndexOf(asset)]*(double)asset.CarbonDioxide!;
                    }
                }
                emissions.Add(hourlyEmissions);
            }

            Series = [new LineSeries<double>()
            {
                Values = emissions,
                LineSmoothness = 1,
                Stroke = null
            }];
            
            YAxes = [
                new()
                {
                    Name = "Amount (t)",
                    TextSize = 16,
                    NameTextSize = 18
                }
            ];
        
            List<DateTime> hours = [];
            foreach (ScheduleHour hour in results.schedule)
            {
                hours.Add(hour.Hour!.Value);
            }

            XAxes =
            [
                new Axis
                {
                    Labels = hours.Select(hour => hour.ToString("dd/MM/yyyy HH:mm")).ToArray()
                }
            ];
        }
        // Generates chart data for electricity usage and price throughout the schedule.
        public static void VisualiseElectricityData()
        {
            Schedule results = ResultsDataManager.Load();
            List<double> usage = [];
            List<double> price = [];
            List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
            foreach (ScheduleHour hour in results.schedule)
            {
                double hourlyUsage = 0;
                foreach (ProductionAsset asset in assets)
                {
                    if (hour.Assets!.Contains(asset))
                    {
                        hourlyUsage += hour.Demands![hour.Assets.IndexOf(asset)]*-(double)asset.Electricity!/(double)asset.Heat!;
                    }
                }
                usage.Add(hourlyUsage);
                price.Add((double)SourceDataManager.GetDataByDateTime((DateTime)hour.Hour!)!.ElectricityPrice!);
            }

            Series = [];
            LineSeries<double> usageSeries = new()
            {
                Values = usage,
                Name = "Usage",
                LineSmoothness = 1,
                Stroke = null
            };
            Series.Add(usageSeries);

            LineSeries<double> priceSeries = new()
            {
                Values = price,
                Name = "Price",
                Stroke = null,
                LineSmoothness = 1,
                ScalesYAt = 1
            };
            Series.Add(priceSeries);
        
            YAxes = [
                new()
                {
                    Name = "Usage (MWh)",
                    TextSize = 16,
                    NameTextSize = 18,
                },
                new()
                {
                    Name = "Price (€/MWh)",
                    TextSize = 16,
                    NameTextSize = 18,
                    Position = LiveChartsCore.Measure.AxisPosition.End,
                }
            ];

            List<DateTime> hours = [];
            foreach (ScheduleHour hour in results.schedule)
            {
                hours.Add(hour.Hour!.Value);
            }

            XAxes =
            [
                new Axis
                {
                    Labels = hours.Select(hour => hour.ToString("dd/MM/yyyy HH:mm")).ToArray()
                }
            ];
        }
        // Generates chart data for total cost by different optimisation scenarios.
        public static void VisualiseCostByOptimisationData()
        {
            Schedule results = ResultsDataManager.Load();
            List<List<double>> costList = [[], [], []]; //NetCost, Cost, Emissions
            List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
            Schedule NetOptimised = Optimiser.NetOptimise(results.startDate, results.endDate);
            Schedule CostOptimised = Optimiser.Optimise(results.startDate, results.endDate, OptimisationChoice.Cost);
            Schedule EmissionOptimised = Optimiser.Optimise(results.startDate, results.endDate, OptimisationChoice.Emissions);
            for (int i = 0; i < results.schedule.Count; i++)
            {
                List<double> hourlyCosts = [0, 0, 0];
                foreach (ProductionAsset asset in assets)
                {
                    if (NetOptimised.schedule[i].Assets!.Contains(asset))
                    {
                        ScheduleHour hour = NetOptimised.schedule[i];
                        hourlyCosts[0] += hour.Demands![hour.Assets!.IndexOf(asset)]*(double)asset.Cost!;
                        hourlyCosts[0] += hour.Demands![hour.Assets.IndexOf(asset)]*-(double)asset.Electricity!/(double)asset.Heat!*(double)SourceDataManager.GetDataByDateTime((DateTime)hour.Hour!)!.ElectricityPrice!;
                    }
                    if (CostOptimised.schedule[i].Assets!.Contains(asset))
                    {
                        ScheduleHour hour = CostOptimised.schedule[i];
                        hourlyCosts[1] += hour.Demands![hour.Assets!.IndexOf(asset)]*(double)asset.Cost!;
                        hourlyCosts[1] += hour.Demands![hour.Assets.IndexOf(asset)]*-(double)asset.Electricity!/(double)asset.Heat!*(double)SourceDataManager.GetDataByDateTime((DateTime)hour.Hour!)!.ElectricityPrice!;
                    }
                    if (EmissionOptimised.schedule[i].Assets!.Contains(asset))
                    {
                        ScheduleHour hour = EmissionOptimised.schedule[i];
                        hourlyCosts[2] += hour.Demands![hour.Assets!.IndexOf(asset)]*(double)asset.Cost!;
                        hourlyCosts[2] += hour.Demands![hour.Assets.IndexOf(asset)]*-(double)asset.Electricity!/(double)asset.Heat!*(double)SourceDataManager.GetDataByDateTime((DateTime)hour.Hour!)!.ElectricityPrice!;
                    }
                }
                for (int j = 0; j < costList.Count; j++)
                {
                    costList[j].Add(hourlyCosts[j]);
                }
            }
            List<string> CostNames = ["Net Cost", "Total Cost", "Emissions"];

            Series = [];
            int colorIndex = 0;
            foreach(List<double> costs in costList)
            {
                LineSeries<double> lineSeries = new()
                {
                    Values = costs,
                    LineSmoothness = 1,
                    Fill = new SolidColorPaint(alphaColors[colorIndex%colors.Count])
                    {
                        StrokeThickness = 3
                    },
                    GeometryStroke = null,
                    GeometryFill = null,
                    Name = CostNames[costList.IndexOf(costs)],
                    Stroke = new SolidColorPaint(colors[colorIndex%colors.Count])
                    {
                        StrokeThickness = 3
                    }
                };
                colorIndex = (colorIndex + 1) < colors.Count ? colorIndex + 1 : 0;
                Series.Add(lineSeries);
            }
            
            YAxes = [
                new()
                {
                    Name = "Cost (€)",
                    TextSize = 16,
                    NameTextSize = 18
                }
            ];
        
            List<DateTime> hours = [];
            foreach (ScheduleHour hour in results.schedule)
            {
                hours.Add(hour.Hour!.Value);
            }

            XAxes =
            [
                new Axis
                {
                    Labels = hours.Select(hour => hour.ToString("dd/MM/yyyy HH:mm")).ToArray()
                }
            ];
        }
        // Generates chart data for emissions by different optimisation scenarios.
        public static void VisualiseEmissionsByOptimisationData()
        {
            Schedule results = ResultsDataManager.Load();
            List<List<double>> emissionList = [[], [], []]; //NetCost, Cost, Emissions
            List<ProductionAsset> assets = AssetManager.GetAllUnits().ToList();
            Schedule NetOptimised = Optimiser.NetOptimise(results.startDate, results.endDate);
            Schedule CostOptimised = Optimiser.Optimise(results.startDate, results.endDate, OptimisationChoice.Cost);
            Schedule EmissionOptimised = Optimiser.Optimise(results.startDate, results.endDate, OptimisationChoice.Emissions);
            for (int i = 0; i < NetOptimised.schedule.Count; i++)
            {
                List<double> hourlyEmissions = [0, 0, 0]; //NetCost, Cost, Emissions
                foreach (ProductionAsset asset in assets)
                {
                    if (NetOptimised.schedule[i].Assets!.Contains(asset))
                    {
                        ScheduleHour hour = NetOptimised.schedule[i];
                        hourlyEmissions[0] += hour.Demands![hour.Assets!.IndexOf(asset)]*(double)asset.CarbonDioxide!;
                    }
                    if (CostOptimised.schedule[i].Assets!.Contains(asset))
                    {
                        ScheduleHour hour = CostOptimised.schedule[i];
                        hourlyEmissions[1] += hour.Demands![hour.Assets!.IndexOf(asset)]*(double)asset.CarbonDioxide!;
                    }
                    if (EmissionOptimised.schedule[i].Assets!.Contains(asset))
                    {
                        ScheduleHour hour = EmissionOptimised.schedule[i];
                        hourlyEmissions[2] += hour.Demands![hour.Assets!.IndexOf(asset)]*(double)asset.CarbonDioxide!;
                    }
                }
                for (int j = 0; j < emissionList.Count; j++)
                {
                    emissionList[j].Add(hourlyEmissions[j]);
                }
            }
            List<string> CostNames = ["Net Cost", "Cost", "Emissions"];

            Series = [];
            int colorIndex = 0;
            foreach(List<double> emissions in emissionList)
            {
                LineSeries<double> lineSeries = new()
                {
                    Values = emissions,
                    LineSmoothness = 1,
                    Fill = new SolidColorPaint(alphaColors[colorIndex%colors.Count])
                    {
                        StrokeThickness = 3
                    },
                    GeometryStroke = null,
                    GeometryFill = null,
                    Name = CostNames[emissionList.IndexOf(emissions)],
                    Stroke = new SolidColorPaint(colors[colorIndex%colors.Count])
                    {
                        StrokeThickness = 3
                    }
                };
                colorIndex = (colorIndex + 1) < colors.Count ? colorIndex + 1 : 0;
                Series.Add(lineSeries);
            }
            
            YAxes = [
                new()
                {
                    Name = "Emissions (t)",
                    TextSize = 16,
                    NameTextSize = 18
                }
            ];
        
            List<DateTime> hours = [];
            foreach (ScheduleHour hour in results.schedule)
            {
                hours.Add(hour.Hour!.Value);
            }

            XAxes =
            [
                new Axis
                {
                    Labels = hours.Select(hour => hour.ToString("dd/MM/yyyy HH:mm")).ToArray()
                }
            ];
        }
    }
}