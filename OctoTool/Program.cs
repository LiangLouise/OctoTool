using System;
using System.Configuration;
using OctoTool.Data;
using OctoTool.Scripts;

namespace OctoTool
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            work(new[] {EnvironmentData.STAGING, EnvironmentData.PERFORMANCE, "DevopsProjects.json"});
        }

        public static void getlist()
        {
            var mongoId = "LibraryVariableSets-381";
            var redisId = "LibraryVariableSets-501";
            var rabbitId = "LibraryVariableSets-244";
            var client = CreateClient();
            var repo = client.GetProjectRepo();
            foreach (var project in repo.GetAll())
            {
                
                if(project.IsDisabled){ continue;}
                
                var p = new OctoProject(project);
                if (p.GetDeployment(EnvironmentData.PERFORMANCE) == null)
                {
                    continue;
                }
                
                var setList = project.IncludedLibraryVariableSetIds;
                string line = project.Name;             
                
                if (setList.Contains(mongoId))
                {
                    line += " MongoDB ";                  
                } 
                
                if (setList.Contains(redisId))
                {
                    line += " Redis ";
                }
                
                
                if (setList.Contains(rabbitId))                                                          
                {                                                     
                    line += " RabbitMQ ";
                }

                if (line.Length != project.Name.Length)
                {
                    Console.WriteLine(line);
                }
            }
        }
        
        
        
        
        
        public static void work(string[] args)
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