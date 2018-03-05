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
        internal string Path { get; private set; }


        // Construction

        internal TempFile()
        {
            Path = System.IO.Path.GetTempFileName();
        }


        // IDisposable

        public void Dispose()
        {
            if (Path != null && File.Exists(Path))
            {
                File.Delete(Path);
                Debug.Assert(!File.Exists(Path));
            }
        }
    }
}
