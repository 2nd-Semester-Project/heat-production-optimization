using OfficeOpenXml; // dotnet add package EPPlus
using System.Globalization;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeatOptimiser
{
    public class SourceDataPoint
    {
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public double? HeatDemand { get; set; }
        public double? ElectricityPrice { get; set; }
    }

    public class SourceData
    {
        private const string defaultSavePath = "data/source_data.csv";
        public List<SourceDataPoint> LoadedData { get; set; }

        public SourceData()
        {
            string XLSXFIlePath = SettingsManager.GetSetting("XLSXFilePath");
            string columnstring = SettingsManager.GetSetting("Column");
            string rowstring = SettingsManager.GetSetting("Row");

            if (XLSXFIlePath == string.Empty || !File.Exists(XLSXFIlePath) || columnstring == string.Empty || rowstring == string.Empty)
            {
                SettingsManager.SaveSetting("DataLoaded", "False");
            }
            else {
                int column = int.TryParse(columnstring, out column) ? column : 4;
                int row = int.TryParse(rowstring, out row) ? row : 7;
                LoadedData = SourceDataManager.LoadXLSXFile(XLSXFIlePath, column, row);
                if (!(LoadedData.Count > 0))
                {
                    SettingsManager.SaveSetting("DataLoaded", "False");
                }
                else {
                    SourceDataManager.WriteToCSV(LoadedData, defaultSavePath);
                }
            }

        }
        public void LoadSourceData(string filePath, int rowStart, int columnStart)
        {
            LoadedData = SourceDataManager.LoadXLSXFile(filePath, rowStart, columnStart);
            if (!(LoadedData.Count > 0))
            {
                SettingsManager.SaveSetting("DataLoaded", "False");
            }
            SettingsManager.SaveSetting("XLSXFilePath", filePath);
            SettingsManager.SaveSetting("Row", rowStart.ToString());
            SettingsManager.SaveSetting("Column", columnStart.ToString());
            SettingsManager.SaveSetting("DataLoaded", "True");
            // Automatically write the CSV files
            SourceDataManager.WriteToCSV(LoadedData, defaultSavePath);
        }

    }
    public static class SourceDataManager
    {
        public static List<SourceDataPoint> LoadXLSXFile(string file, int rowStart, int columnStart, int workSheetNumber = 0)
        {
            var sourceList = new List<SourceDataPoint>();
            if (file != string.Empty || File.Exists(file) || rowStart >= 1 || columnStart >= 1)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // EPPlus license

                using (var package = new ExcelPackage(new FileInfo(file)))
                {
                    ExcelWorksheet worksheet;
                    try
                    {
                        worksheet = null ?? package.Workbook.Worksheets[0];
                        worksheet = package.Workbook.Worksheets[workSheetNumber];
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Worksheet not found: {e}");
                        return sourceList;
                    }

                    if (worksheet.Dimension == null)
                    {
                        Console.WriteLine("The worksheet is empty.");
                        return sourceList;
                    }

                    for (int row = rowStart; row <= worksheet.Dimension.End.Row; row++)
                    {
                        try
                        {
                            DateTime temp;
                            string[] formats = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy HH:mm:ss", "HH.mm.ss", "HH:mm:ss","dd.MM.yyyy HH:mm:ss" };
                            SourceDataPoint sourceData = new SourceDataPoint
                            {
                                TimeFrom = DateTime.TryParseExact(worksheet.Cells[row, columnStart].Value?.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp) ? temp : null,
                                TimeTo = DateTime.TryParseExact(worksheet.Cells[row, columnStart + 1].Value?.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp) ? temp : null,
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
            }
            return sourceList;
        }

        public static List<SourceDataPoint> GetDataInRange(SourceData data, DateTime startDate, DateTime endDate)
        {
            DateTime winterEnd = DateTime.ParseExact("31/03/2023", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            bool rangeExists = false;
            int startIndex = 0;
            List<SourceDataPoint> dataCollection = data.LoadedData;
            foreach (SourceDataPoint point in dataCollection)
            {
                if (point.TimeFrom.HasValue)
                {
                    DateTime dt = (DateTime)point.TimeFrom;
                    if (dt.Date == startDate.Date)
                    {
                        rangeExists = true;
                        break;
                    }
                    startIndex++;
                }
            }
            int endIndex = startIndex;
            if (rangeExists)
            {
                foreach (SourceDataPoint point in dataCollection.GetRange(startIndex, dataCollection.Count - startIndex))
                {
                    endIndex++;
                    DateTime dt = (DateTime)point.TimeTo!;
                    if (dt.Date > endDate.Date)
                    {
                        break;
                    }
                }
                return dataCollection.GetRange(startIndex, endIndex - startIndex);
            }
            return new List<SourceDataPoint>();
        }

        public static void WriteToCSV(List<SourceDataPoint> data, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("TimeFrom,TimeTo,HeatDemand,ElectricityPrice");
                foreach (var point in data)
                {
                    var line = $"{point.TimeFrom},{point.TimeTo},{point.HeatDemand},{point.ElectricityPrice}";
                    writer.WriteLine(line);
                }
            }
        }
    }
}
