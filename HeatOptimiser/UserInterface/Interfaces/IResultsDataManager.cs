using System;

namespace HeatOptimiser
{
    public interface IResultsDataManager
    {
        public void Save(Schedule schedule);
        public void Remove(DateOnly dateFrom, DateOnly dateTo);
        public Schedule Load(DateOnly dateFrom, DateOnly dateTo);
    }
}