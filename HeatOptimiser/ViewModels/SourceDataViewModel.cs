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
        private readonly ObservableCollection<DateTimePoint> _heatDemandData;
        private readonly ObservableCollection<DateTimePoint> _electricityPriceData;
       

        public ObservableCollection<ISeries> Series { get; set; }

        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }
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

            _heatDemandData = new ObservableCollection<DateTimePoint>();
            _electricityPriceData = new ObservableCollection<DateTimePoint>();
            _sourceText = "Source Data not loaded. \nPlease load the data.";

            if (SettingsManager.GetSetting("DataLoaded") == "True")
            {
                _sourceText = "Source Data loaded.";
                foreach (var point in DataVisualizer.sourceData.LoadedData)
                {
                    if (point.HeatDemand.HasValue && point.TimeFrom.HasValue && point.ElectricityPrice.HasValue)
                    {
                        _heatDemandData.Add(new DateTimePoint(point.TimeFrom.Value, point.HeatDemand.Value));
                        _electricityPriceData.Add(new DateTimePoint(point.TimeFrom.Value, point.ElectricityPrice.Value));

                        Console.WriteLine($"Added Point: TimeFrom={point.TimeFrom.Value}, HeatDemand={point.HeatDemand.Value}, ElectricityPrice={point.ElectricityPrice.Value}");
                    }
                }
            }

            Series = new ObservableCollection<ISeries>
            {
                new LineSeries<DateTimePoint>
                {
                    Values = _heatDemandData,
                    Name = "Heat Demand (MWh)",
                    Fill = null,
                    GeometryStroke = null,
                    GeometryFill = null,
                    LineSmoothness = 1,
                    Stroke = new SolidColorPaint(new SKColor(194, 36, 62))
                    {
                        StrokeThickness = 3 // Set the thickness of the Heat Demand line
                    }
                },
                new LineSeries<DateTimePoint>
                {
                    Values = _electricityPriceData,
                    Name = "Electricity Price (€/MWh)",
                    Fill = null,
                    GeometryStroke = null,
                    GeometryFill = null,
                    LineSmoothness = 2,
                    ScalesYAt = 1, // This tells the series to use the secondary Y-axis
                   Stroke = new SolidColorPaint(new SKColor(0, 92, 230)) // Set the color shade 
                    {
                        StrokeThickness = 3 // Set the thickness of the Electricity Price line
                    },
                }
            };

              XAxes = new Axis[]
            {
                new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMMM dd HH:mm"))
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Heat Demand (MWh)",
                    TextSize = 16,
                    NameTextSize = 18
                },
                new Axis
                {
                    Name = "Electricity Price (€/MWh)",
                    TextSize = 16,
                    NameTextSize = 18,
                    Position = LiveChartsCore.Measure.AxisPosition.End
                }
            };
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
            }
        }

        
    }
}
