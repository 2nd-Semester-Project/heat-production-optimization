namespace HeatOptimiser
{
    public interface IOptimiserModule
    {
        Schedule Optimise(DateTime startDate, DateTime endDate);
    }
}