using System;
using System.Collections.ObjectModel;
using DynamicData;
using HeatOptimiser;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SkiaSharp;

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

        private readonly ObservableCollection<DateTimePoint> _heatDemandData;
        private readonly ObservableCollection<DateTimePoint> _electricityPriceData;
        private string _sourceText;

        public string SourceText
        {
            get => _sourceText;
            set => this.RaiseAndSetIfChanged(ref _sourceText, value);
        }

        public ObservableCollection<ISeries> Series { get; set; }

        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        public HomepageViewModel()
        {
            _heatDemandData = new ObservableCollection<DateTimePoint>();
            _electricityPriceData = new ObservableCollection<DateTimePoint>();
            _sourceText = "Source Data not loaded. \nPlease load the data.";
            _assetCount = AssetManager.LoadUnits(AssetManager.saveFileName).Count;

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
    }
}


