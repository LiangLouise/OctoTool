using System.Configuration;
using OctoTool.Scripts;

namespace OctoTool
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 3) return;
            var sourEnvName = args[0];
            var targetEnvName = args[1];
            var pathToConfigs = args[2];
                
            var client = CreateClient();
            var devops = new Devops(sourEnvName, targetEnvName, pathToConfigs);
                
            devops.CreateDeployment();
        }

         static WebClient CreateClient()
         {
             var reader = new AppSettingsReader();
             var url = ConfigurationManager.AppSettings["OctoBaseUrl"];
             var key = ConfigurationManager.AppSettings["OctoAPIKey"];
             return WebClient.CreateWebClientRef(url, key);
         }    
    }
}