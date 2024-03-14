namespace HeatOptimiser
{
    public interface ISourceDataManager
    {
        public List<SourceDataPoint> LoadXLSXFile(string filePath, int rowStartd, int columnStart, int workSheetNumber);
    }
}