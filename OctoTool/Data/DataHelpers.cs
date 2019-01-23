using System.IO;
using Newtonsoft.Json.Linq;

namespace OctoTool.Data
{
    public static class DataHelpers
    {
        public static JObject GetJsonContent(string pathToJsonFile)
        {
            using (StreamReader r = new StreamReader(pathToJsonFile))
            {
                var json = r.ReadToEnd();
                var jobject = JObject.Parse(json);
                return jobject;
            }
        }
    }
}