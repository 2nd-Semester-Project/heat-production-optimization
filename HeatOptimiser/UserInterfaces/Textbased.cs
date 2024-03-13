namespace HeatOptimiser
{
    public class TextBasedUI: IUserInterface {
        public void example()
        {
            Console.WriteLine("Hello, GitHub!");
            SourceDataManager SourceManager = new SourceDataManager();

            List<SourceData> SourceList = SourceManager.LoadXLSXFile("data/SourceData.xlsx", 1, 3);
            foreach (SourceData source in SourceList)
            {
                Console.WriteLine(source.TimeFrom + " " + source.TimeTo + " " + source.HeatDemand + " " + source.ElectricityPrice);
            }


        }
    }
}