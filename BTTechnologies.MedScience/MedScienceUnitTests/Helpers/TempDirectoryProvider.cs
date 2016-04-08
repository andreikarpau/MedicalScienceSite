using System;
using System.IO;

namespace MedScienceUnitTests.Helpers
{
    public class TempDirectoryProvider: IDisposable 
    {
        public string TempDirectoryPath { get; set; }

        public TempDirectoryProvider(string baseDirectory)
        {
            TempDirectoryPath = Path.Combine(baseDirectory, Guid.NewGuid().ToString());
            if (Directory.Exists(TempDirectoryPath))
                Directory.Delete(TempDirectoryPath, true);

            Directory.CreateDirectory(TempDirectoryPath);
        }

        public TempDirectoryProvider(): this(Path.GetTempPath())
        {
        }

        public void Dispose()
        {
            if (Directory.Exists(TempDirectoryPath))
                Directory.Delete(TempDirectoryPath, true);
        }
    }
}
