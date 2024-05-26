using ReactiveUI;
using System.Reactive;
using HeatOptimiser;
using System.Collections.ObjectModel;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace UserInterface.ViewModels
{
    public class SourceDataViewModel : ViewModelBase
    {
        private string _selectedFilePath;
        private int _selectedRow;
        private int _selectedColumn;
        private string _errorFilePath;
        private bool _isErrorFilePathVisible;
        private string _errorSelectRow;
        private bool _isErrorSelectRowVisible;
        private string _errorSelectColumn;
        private bool _isErrorSelectColumnVisible;
        private string _sourceText;
        public static ObservableCollection<ISeries> Series { get; set; } = [];

        public static Axis[] XAxes { get; set; } = [];
        public static Axis[] YAxes { get; set; } = [];
        public string SourceText
        {
            get => _sourceText;
            set => this.RaiseAndSetIfChanged(ref _sourceText, value);
        }

        public ReactiveCommand<Unit, Unit> SourceDataCommand { get; }

        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set => this.RaiseAndSetIfChanged(ref _selectedFilePath, value);
        }

        public int SelectedRow
        {
            get => _selectedRow;
            set => this.RaiseAndSetIfChanged(ref _selectedRow, value);
        }

        public int SelectedColumn
        {
            get => _selectedColumn;
            set => this.RaiseAndSetIfChanged(ref _selectedColumn, value);
        }

        public string ErrorFilePath
        {
            get => _errorFilePath;
            set => this.RaiseAndSetIfChanged(ref _errorFilePath, value);
        }

        public bool IsErrorFilePathVisible
        {
            get => _isErrorFilePathVisible;
            set => this.RaiseAndSetIfChanged(ref _isErrorFilePathVisible, value);
        }

        public string ErrorSelectRow
        {
            get => _errorSelectRow;
            set => this.RaiseAndSetIfChanged(ref _errorSelectRow, value);
        }

        public bool IsErrorSelectRowVisible
        {
            get => _isErrorSelectRowVisible;
            set => this.RaiseAndSetIfChanged(ref _isErrorSelectRowVisible, value);
        }

        public string ErrorSelectColumn
        {
            get => _errorSelectColumn;
            set => this.RaiseAndSetIfChanged(ref _errorSelectColumn, value);
        }

        public bool IsErrorSelectColumnVisible
        {
            get => _isErrorSelectColumnVisible;
            set => this.RaiseAndSetIfChanged(ref _isErrorSelectColumnVisible, value);
        }

        public SourceDataViewModel()
        {
            _selectedFilePath = SettingsManager.GetSetting("XLSXFilePath");
            _selectedRow = int.TryParse(SettingsManager.GetSetting("Row"), out int row) ? row : 7;
            _selectedColumn = int.TryParse(SettingsManager.GetSetting("Column"), out int column) ? column : 4;
            SourceData sourceData = new SourceData();
            SourceDataCommand = ReactiveCommand.Create(LoadSourceData);

            // Initialize error properties
            ErrorFilePath = string.Empty;
            IsErrorFilePathVisible = false;
            ErrorSelectRow = string.Empty;
            IsErrorSelectRowVisible = false;
            ErrorSelectColumn = string.Empty;
            IsErrorSelectColumnVisible = false;
            
            if (SettingsManager.GetSetting("DataLoaded") == "True")
            {
                SourceDataManager.VisualiseData(sourceData);
                Series = SourceDataManager.Series;
                XAxes = SourceDataManager.XAxes;
                YAxes = SourceDataManager.YAxes;
            }
            else
            {
                _sourceText = "Source Data not loaded. \nPlease load the data.";
            }
        }

        private void LoadSourceData()
        {
            // Implement the logic to load the source data and handle errors
            bool hasError = false;

            if (string.IsNullOrWhiteSpace(SelectedFilePath))
            {
                ErrorFilePath = "File path cannot be empty.";
                IsErrorFilePathVisible = true;
                hasError = true;
            }
            else
            {
                IsErrorFilePathVisible = false;
            }

            if (SelectedRow <= 0)
            {
                ErrorSelectRow = "Row must be a positive number.";
                IsErrorSelectRowVisible = true;
                hasError = true;
            }
            else
            {
                IsErrorSelectRowVisible = false;
            }

            if (SelectedColumn <= 0)
            {
                ErrorSelectColumn = "Column must be a positive number.";
                IsErrorSelectColumnVisible = true;
                hasError = true;
            }
            else
            {
                IsErrorSelectColumnVisible = false;
            }

            if (!hasError)
            {
                // No errors, proceed with loading the source data
                SourceData sourceData = new SourceData();
                sourceData.LoadSourceData(SelectedFilePath, SelectedRow, SelectedColumn);
                SourceDataManager.VisualiseData(sourceData);
                Series = SourceDataManager.Series;
                XAxes = SourceDataManager.XAxes;
                YAxes = SourceDataManager.YAxes;

                _sourceText = "Source Data loaded.";
            }
        }
    }
}
