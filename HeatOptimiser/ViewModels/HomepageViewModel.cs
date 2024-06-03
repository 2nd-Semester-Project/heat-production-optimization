using HeatOptimiser;
using ReactiveUI;
using System.IO;

namespace UserInterface.ViewModels
{
    public class HomepageViewModel : ViewModelBase
    {
        public int _assetCount;
        public int AssetCount
        {
            get => _assetCount;
            set => this.RaiseAndSetIfChanged(ref _assetCount, value);
        }

        public HomepageViewModel()
        {
            string XLSXFilePath = SettingsManager.GetSetting("XLSXFilePath");
            string columnstring = SettingsManager.GetSetting("Column");
            string rowstring = SettingsManager.GetSetting("Row");

            // Checks if the XLSX file path is empty, the file does not exist, or the column and row strings are empty.
            if (XLSXFilePath == string.Empty || !File.Exists(XLSXFilePath) || columnstring == string.Empty || rowstring == string.Empty)
            {
                SettingsManager.SaveSetting("DataLoaded", "False");
            }
            else {
                int column = int.TryParse(columnstring, out column) ? column : 4;
                int row = int.TryParse(rowstring, out row) ? row : 7;
                SourceDataManager.LoadedData = SourceDataManager.LoadXLSXFile(XLSXFilePath, column, row);
                if (!(SourceDataManager.LoadedData.Count > 0))
                {
                    SettingsManager.SaveSetting("DataLoaded", "False");
                }
                else {
                    SourceDataManager.WriteToCSV(SourceDataManager.LoadedData, SourceDataManager.defaultSavePath);
                }
            }
            _assetCount = AssetManager.LoadUnits().Count;
        }
    }
}


