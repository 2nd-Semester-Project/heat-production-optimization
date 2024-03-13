namespace HeatOptimiser
{
    public interface ISourceDataManager
    {
        public List<SourceData> LoadXLSXFile(string filePath, int rowStartd, int columnStart);
    }
}