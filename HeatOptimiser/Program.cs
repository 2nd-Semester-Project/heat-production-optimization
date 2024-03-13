namespace HeatOptimiser
{
    class Program {
        public static void Main()
        {
            SourceDataManager SourceManager = new SourceDataManager();

            List<SourceData> SourceList = SourceManager.LoadXLSXFile("D:\\DK\\DK studies\\SDU\\2nd Semester\\Semester Project\\heat-production-optimization\\HeatOptimiser\\SourceData.xlsx", 1, 3);
            foreach (SourceData source in SourceList)
            {
                Console.WriteLine(source.TimeFrom + " " + source.TimeTo + " " + source.HeatDemand + " " + source.ElectricityPrice);
            }
            new TextBasedUI().example();
        }   
    }
}