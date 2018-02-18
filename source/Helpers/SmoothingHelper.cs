using System;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    public static class SmoothingHelper
    {
        /// <summary>Returns <c>count</c>/<c>total</c> after applying Add-Delta Smoothing.</summary>
        public static double GetAddDeltaProbability(double count, double total, double delta, int groupSize)
        {
            double numerator = delta + count;
            double denominator = (groupSize * delta) + total;
            if (denominator == 0) throw new DivideByZeroException("Division by zero during Add-Delta Smoothing.");
            return numerator / denominator;
        }
    }
}
