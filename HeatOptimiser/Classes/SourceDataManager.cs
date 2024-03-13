using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Microsot.Interop.Excel;

namespace HeatOptimiser
{
    public class SourceDataManager : ISourceDataManager 
    {
        private string FilePath;
        public List<string> SummerData = new List<string>();
        public List<string> WinterData = new List<string>();


        public SourceDataManager(string filePath)
        {
            FilePath = filePath;
        }
        
        public List<String> SummerData (string? filePath)
        {
            public string filePath = "HeatOptimiser/2024 Heat Production Optimization - Danfoss Deliveries - Source Data Manager.xlsx";


            return SummerData;
        }
       public List<string> WinterData(string? filePath)
        {
           

            return electricityPriceData;
        } 

    }
}