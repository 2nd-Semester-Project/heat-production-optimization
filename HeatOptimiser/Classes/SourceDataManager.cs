using Microsoft.Office.Interop.Excel;

namespace HeatOptimiser
{
    public class SourceData
    {
        public DateTime? TimeFrom;
        public DateTime? TimeTo;
        public double? HeatDemand;
        public double? ElectricityPrice;
    }
    public class SourceDataManager : ISourceDataManager
    {
        public List<SourceData> LoadXLSXFile(string file, int rowStart, int columnStart)
        {

            List<SourceData> DataRows = new List<SourceData>();
            Application excelApp = new Application();
            Workbook excelWB = excelApp.Workbooks.Open(file);
            _Worksheet excelWS = (_Worksheet)excelWB.Sheets[1];
            Microsoft.Office.Interop.Excel.Range excelRange = excelWS.UsedRange;
            int rowCount = excelRange.Rows.Count;
            int columnCount = columnStart + 3;


            for (int i = rowStart; i <= rowCount; i++) // insert readlines for indexes
            {
                DateTime TimeNow = DateTime.UtcNow;
                DateTime? localTimeFrom = DateTime.UtcNow;
                DateTime? localTimeTo = DateTime.UtcNow;
                double? localHeatDemand = null;
                double? localElectricityPrice = null;
                for (int j = columnStart; j <= columnCount; j++)
                {
                    if (j == columnStart)
                    {
                        localTimeFrom = DateTime.Parse(excelRange.Cells[i, j].ToString()!);
                    }
                    else if (j == columnStart + 1)
                    {
                        localTimeTo = DateTime.Parse(excelRange.Cells[i, j].ToString()!);
                    }
                    else if (j == columnStart + 2)
                    {
                        localHeatDemand = double.Parse(excelRange.Cells[i, j].ToString()!);
                    }
                    else if (j == columnStart + 3)
                    {
                        localElectricityPrice = double.Parse(excelRange.Cells[i, j].ToString()!);
                    }   
                }
                if (localTimeFrom == TimeNow || localTimeTo == TimeNow || localHeatDemand == null || localElectricityPrice == null)
                {
                    Console.WriteLine("Error: One or more of the values are null");
                    //throw new Exception("Error: One or more of the values are null");
                }
                else
                {
                SourceData localSourceData = new SourceData
                {
                    TimeFrom = localTimeFrom,
                    TimeTo = localTimeTo,
                    HeatDemand = localHeatDemand,
                    ElectricityPrice = localElectricityPrice
                };
                }
            }
            return DataRows;
        }
    }
}