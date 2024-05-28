using OfficeOpenXml; // dotnet add package EPPlus
using System.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using LiveChartsCore.Defaults;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HeatOptimiser
{
    public class SourceDataPoint
    {
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public double? HeatDemand { get; set; }
        public double? ElectricityPrice { get; set; }
    }
    public static class SourceDataManager
    {
        public const string defaultSavePath = "data/source_data.csv";
        public static ObservableCollection<SourceDataPoint> LoadedData { get; set; } = [];

        public static void LoadSourceData(string filePath, int columnStart, int rowStart)
        {

            LoadedData.Clear();
            LoadedData = LoadXLSXFile(filePath, columnStart, rowStart);
            if (!(LoadedData.Count > 0))
            {
                SettingsManager.SaveSetting("DataLoaded", "False");
            }
            else
            {
                SettingsManager.SaveSetting("XLSXFilePath", filePath);
                SettingsManager.SaveSetting("Row", rowStart.ToString());
                SettingsManager.SaveSetting("Column", columnStart.ToString());
                SettingsManager.SaveSetting("DataLoaded", "True");
                
                // Automatically write the CSV files
                WriteToCSV(LoadedData, defaultSavePath);
            }
        }
        private static List<DateTimePoint> _heatDemandData;
        private static List<DateTimePoint> _electricityPriceData;
        public static ObservableCollection<ISeries> Series { get; set; }

        public static Axis[] XAxes { get; set; }
        public static Axis[] YAxes { get; set; }
        public static ObservableCollection<SourceDataPoint> LoadXLSXFile(string file, int columnStart, int rowStart, int workSheetNumber = 0)
        {
            var sourceObservableCollection = new ObservableCollection<SourceDataPoint>();
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
                    return sourceObservableCollection;
                }

                    if (worksheet.Dimension == null)
                    {
                        return sourceObservableCollection;
                    }

                    for (int row = rowStart; row <= worksheet.Dimension.End.Row; row++)
                    {
                        try
                        {
                            DateTime temp;
                            string[] formats = {  "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy HH:mm:ss", "HH.mm.ss", "HH:mm:ss","dd.MM.yyyy HH:mm:ss" };
                            SourceDataPoint sourceData = new SourceDataPoint
                            {
                                TimeFrom = DateTime.TryParseExact(worksheet.Cells[row, columnStart].Value?.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp) ? temp : null,
                                TimeTo = DateTime.TryParseExact(worksheet.Cells[row, columnStart + 1].Value?.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp) ? temp : null,
                                HeatDemand = worksheet.Cells[row, columnStart + 2]?.Value != null ? double.Parse(worksheet.Cells[row, columnStart + 2].Value.ToString()!) : null,
                                ElectricityPrice = worksheet.Cells[row, columnStart + 3]?.Value != null ? double.Parse(worksheet.Cells[row, columnStart + 3].Value.ToString()!) : null
                            };
                            sourceObservableCollection.Add(sourceData);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
            return sourceObservableCollection;
        }

        public static SourceDataPoint? GetDataByDateTime(DateTime dateTime)
        {
            foreach(SourceDataPoint sdp in LoadedData)
            {
                if(sdp.TimeFrom == dateTime)
                {
                    return sdp;
                };
            }
            return null;
        }

        public static ObservableCollection<SourceDataPoint> GetDataInRange(DateTime startDate, DateTime endDate)
        {
            bool rangeExists = false;
            int startIndex = 0;
            ObservableCollection<SourceDataPoint> dataCollection = LoadedData;
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
                ObservableCollection<SourceDataPoint> range = new ObservableCollection<SourceDataPoint>();
                for (int i = startIndex; i < dataCollection.Count; i++)
                {
                    SourceDataPoint point = dataCollection[i];
                    endIndex++;
                    DateTime dt = (DateTime)point.TimeTo!;
                    if (dt.Date > endDate.Date)
                    {
                        break;
                    }
                    range.Add(point);
                }
                return new ObservableCollection<SourceDataPoint>(range);
            }
            return new ObservableCollection<SourceDataPoint>();
        }

        public static void WriteToCSV(ObservableCollection<SourceDataPoint> data, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var point in data)
                {
                    var line = $"{point.TimeFrom},{point.TimeTo},{point.HeatDemand},{point.ElectricityPrice}";
                }
            }
        }

        public static void VisualiseData()
        {
            _heatDemandData = [];
            _electricityPriceData = [];
            
            foreach (var point in LoadedData)
            {
                if (point.HeatDemand.HasValue && point.TimeFrom.HasValue && point.ElectricityPrice.HasValue)
                {
                    _heatDemandData.Add(new DateTimePoint(point.TimeFrom.Value, point.HeatDemand.Value));
                    _electricityPriceData.Add(new DateTimePoint(point.TimeFrom.Value, point.ElectricityPrice.Value));
                }
            }

            List<List<DateTimePoint>> data = [_heatDemandData, _electricityPriceData];
            List<string> names = ["Heat Deamand (MWh)", "Electricity Price (€/MWh)"];
            DataVisualizer.VisualiseSourceData(data, names);
        }
    }
}