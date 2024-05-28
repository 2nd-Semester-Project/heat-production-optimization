using System;
using System.Collections.Generic;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace HeatOptimiser
{

    public static class DataVisualizer
    {
        public static void VisualiseSourceData(List<List<DateTimePoint>> data, List<string> names)
        {
            List<SKColor> colors = [
                new SKColor(194, 36, 62),
                new SKColor(0, 92, 230)
            ];
            SourceDataManager.Series = [];
            SourceDataManager.XAxes = [];
            SourceDataManager.YAxes = new Axis[names.Count];
            if (data.Count != names.Count)
            {
                Console.WriteLine("Invalid arguments!");
                return;
            }
            for(int index = 0; index < data.Count; index++)
            {
                LineSeries<DateTimePoint> lineSeries = new()
                {
                    Values = data[index],
                    Name = names[index],
                    Fill = null,
                    GeometryStroke = null,
                    GeometryFill = null,
                    LineSmoothness = 1,
                    Stroke = new SolidColorPaint(colors[index%colors.Count])
                    {
                        StrokeThickness = 3
                    }
                };
                Axis axis = new()
                {
                    Name = names[index],
                    TextSize = 16,
                    NameTextSize = 18
                };
                if (index != 0)
                {
                    lineSeries.LineSmoothness = 2;
                    lineSeries.ScalesYAt = 1;
                    axis.Position = LiveChartsCore.Measure.AxisPosition.End;
                }
                SourceDataManager.Series.Add(
                    lineSeries
                );
                SourceDataManager.YAxes[index] = axis;
            }
            SourceDataManager.XAxes = [
                new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMMM dd HH:mm"))
            ];
        }
    }
}