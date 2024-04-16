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
                    assetDemands.Add((double)assets[index].Heat!);
                    index += 1;
                }
                schedule.AddHour(hour.TimeFrom, assetsUsed, assetDemands);
            }
            return schedule;
        }
    }
}