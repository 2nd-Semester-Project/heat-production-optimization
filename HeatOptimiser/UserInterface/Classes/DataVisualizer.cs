
using System;
using System.Collections.Generic;

namespace HeatOptimiser
{

public class DataPoint
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }
    }

    public class OptimizationResult
    {
        public List<DataPoint> EnergySavings { get; set; } = new List<DataPoint>();
        public List<DataPoint> CostEfficiencies { get; set; } = new List<DataPoint>();
        public List<DataPoint> CarbonReductions { get; set; } = new List<DataPoint>();
    }

}

