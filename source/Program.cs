using System;

namespace ericmclachlan.Portfolio
{
    /// <summary>This class contains the entry-point to this application.</summary>
    public class Program
    {
        // Public Methods

        /// <summary>This method is the entry-point to this application.</summary>
        public static void Main(string[] args)
        {
            try
            {
                PerformanceHelper.Measure("\tTotal Execution Time", () =>
                {
                    CommandPlatform.Execute(args);
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: {0}", ex.Message);
            }
        }
    }
}
