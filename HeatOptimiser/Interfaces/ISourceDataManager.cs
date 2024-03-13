namespace HeatOptimiser
{
    public interface ISourceDataManager
    {
        List<String> SummerData (string? filePath); 
        List<String> WinterData (string? filePath);
    }
}