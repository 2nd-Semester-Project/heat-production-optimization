namespace HeatOptimiser
{
    public interface IResultsDataManager
    {
        public void Save(Schedule schedule, string fileName);
        public void Remove(DateOnly dateFrom, DateOnly dateTo);
        public Schedule Load(DateOnly dateFrom, DateOnly dateTo, string fileNameToLoad);
    }
}