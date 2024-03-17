using System.Security.Cryptography;

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
        public void AddHour(DateTime? dateTime, List<ProductionAsset> assets, List<double> demands)
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
        public List<ProductionAsset>? Assets { get; set; }
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
            ProductionAsset gasBoiler = am.SearchUnits("GB")[0];
            ProductionAsset oilBoiler = am.SearchUnits("OB")[0];

            foreach (SourceDataPoint hour in sd.GetDataInRange(data, startDate, endDate))
            {
                if (hour.HeatDemand <= gasBoiler.Heat)
                {
                    double gasDemand = (double)hour.HeatDemand;
                    schedule.AddHour(hour.TimeFrom, [gasBoiler], [gasDemand]);
                }
                else
                {
                    double gasCapacity = (double)gasBoiler.Heat;
                    double hourDemand = (double)hour.HeatDemand;
                    double oilDemand = hourDemand - gasCapacity;
                    schedule.AddHour(hour.TimeFrom, [gasBoiler, oilBoiler], [gasCapacity, oilDemand]);
                }
            }
            return schedule;
        }
    }
}