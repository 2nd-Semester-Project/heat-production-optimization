using OfficeOpenXml; // dotnet add package EPPlus
using System.Globalization;

using Microsoft.Office.Interop.Excel; // dotnet add package Microsoft.Office.Interop.Excel

namespace HeatOptimiser
{
    public class SourceDataPoint
    {
        public DateTime? TimeFrom;
        public DateTime? TimeTo;
        public double? HeatDemand;
        public double? ElectricityPrice;
    }
    public class SourceData {
        public List<SourceDataPoint> SummerData;
        public List<SourceDataPoint> WinterData;
        public SourceData(){
            SourceDataManager sourceManager = new SourceDataManager();
            SummerData = sourceManager.LoadXLSXFile("data/sourcedata.xlsx", 4, 2);
            WinterData = sourceManager.LoadXLSXFile("data/sourcedata.xlsx", 4, 7);
        }
    }
    public class SourceDataManager : ISourceDataManager
    {
        // Example usage: List<SourceDataPoint> SourceList = SourceManager.LoadXLSXFile("data/sourcedata.xlsx", 4, 2); Columns and Rows start with 1!!!!
        public List<SourceDataPoint> LoadXLSXFile(string file, int rowStart, int columnStart, int workSheetNumber = 0)
        {
            var sourceList = new List<SourceDataPoint>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // EPPlus license

            using (var package = new ExcelPackage(new FileInfo(file)))
            {
                var worksheet = package.Workbook.Worksheets[workSheetNumber];

                if (worksheet.Dimension == null)
                {
                    Console.WriteLine("The worksheet is empty.");
                    return sourceList;
                }
                for (int row = rowStart; row <= worksheet.Dimension.End.Row; row++)
                {
                    try {
                        DateTime temp;
                        string[] formats = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy HH:mm:ss", "HH.mm.ss", "HH:mm:ss" };
                        SourceDataPoint sourceData = new SourceDataPoint
                        {
                            TimeFrom = DateTime.TryParseExact(worksheet.Cells[row, columnStart].Value?.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp) ? temp : (DateTime?)null,
                            TimeTo = DateTime.TryParseExact(worksheet.Cells[row, columnStart + 1].Value?.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp) ? temp : (DateTime?)null,
                            HeatDemand = worksheet.Cells[row, columnStart + 2]?.Value != null ? double.Parse(worksheet.Cells[row, columnStart + 2].Value.ToString()!) : null,
                            ElectricityPrice = worksheet.Cells[row, columnStart + 3]?.Value != null ? double.Parse(worksheet.Cells[row, columnStart + 3].Value.ToString()!) : null
                        };
                        sourceList.Add(sourceData);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e}");
                    }
                }
            }

            return sourceList; 
        }
    }
}