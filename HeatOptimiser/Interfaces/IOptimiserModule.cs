using System;
using System.Collections.Generic;
using System.Linq;


namespace HeatOptimiser
{
    public interface IOptimiserModule
    {
        Schedule Optimise(DateTime startDate, DateTime endDate);
    }
}