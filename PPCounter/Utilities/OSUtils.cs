using Newtonsoft.Json;
using System.IO;

namespace PPCounter.Utilities
{
    public static class OSUtils
    {
        public static void WriteFile<T>(T data, string fileName)
        {
            lock (data)
            {
                if (!File.Exists(fileName))
                {
                    (new FileInfo(fileName)).Directory.Create();
                }
                File.WriteAllText(fileName, JsonConvert.SerializeObject(data));
            }
        }
    }
}
