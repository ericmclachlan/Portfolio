using System;
using System.Diagnostics;
using System.IO;

namespace ericmclachlan.Portfolio.Tests
{
    /// <summary>
    /// Represents a temporary file created by the system that will automatically cleanup after use.
    /// </summary>
    internal class TempFile : IDisposable
    {
        // Members

        /// <summary>This is the location of the physical temporary file.</summary>
        internal string Location { get; private set; }


        // Construction

        internal TempFile()
        {
            Location = Path.GetTempFileName();
        }

        internal TempFile(string contents)
        {
            Location = Path.GetTempFileName();
            File.WriteAllText(Location, contents);
        }


        // IDisposable

        public void Dispose()
        {
            if (Location != null && File.Exists(Location))
            {
                File.Delete(Location);
                Debug.Assert(!File.Exists(Location));
                Location = null;
            }
        }
    }
}
