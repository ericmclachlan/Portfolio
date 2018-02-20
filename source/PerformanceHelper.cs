using System;
using System.Diagnostics;

namespace ericmclachlan.Portfolio
{
    public static class PerformanceHelper
    {
        public static void Measure(string label, Action action)
        {
            Stopwatch sw = Stopwatch.StartNew();
            action.Invoke();
            sw.Stop();
            string timeAsText;
            if (sw.Elapsed.TotalSeconds < 10)
                timeAsText = string.Format("{0:0.###} seconds", sw.Elapsed.TotalSeconds);
            else
                timeAsText = string.Format("{0:00}:{1:00}:{2:00}", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds);
            Console.Error.WriteLine("{0}:   {1}", label, timeAsText);
        }
    }
}
