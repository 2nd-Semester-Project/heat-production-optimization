namespace HeatOptimiser
{
    public class TextBasedUI: IUserInterface {
        public void example()
        {
            Console.WriteLine("Hello, GitHub!");

            SourceData sourceData = new SourceData();
            Console.WriteLine();
            Console.WriteLine("Here comes the stats from SourceDataManager data:");
            Console.WriteLine($"Summer data contains {sourceData.SummerData.Count} entries.");
            Console.WriteLine($"Winter data contains {sourceData.WinterData.Count} entries.");
        }
    }
}