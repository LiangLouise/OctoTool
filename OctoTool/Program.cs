using System;
using System.Configuration;
using OctoTool.Data;
using OctoTool.SettingExtensions;

namespace OctoTool
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var client = createClient();
            var settins = new SingleProjectDeploymentSettings();
            Console.WriteLine(settins.UpdateVariableSetNow.ToString());
        }

         static WebClient createClient()
         {
             var reader = new AppSettingsReader();
             var url = ConfigurationManager.AppSettings["OctoBaseUrl"];
             var key = ConfigurationManager.AppSettings["OctoAPIKey"];
             return WebClient.CreateWebClientRef(url, key);
         }
         
         // Single project new release, new deployment example And promote release from one env to another
         static void SingleProjectDeployment()
         {
             var DevOpsTestProject = new OctoProject("DevOpsTest");
             var releaseVersion = "1.12-blabla.204-Sprint-1.0.0.356";
             var releaseSetting = new CreateReleaseSettings
             {
                 ReleaseChannel = "Sprint",
                 ReleaseVersion = releaseVersion,
                 ReleaseNotes = "Automation Tool Test"
             };
             Releases.CreateRelease(DevOpsTestProject.ProjectName, releaseSetting);
             var settings = new SingleProjectDeploymentSettings()
             {
                 TargetEnvironmentName = EnvironmentData.SPRINTDEV
             };
             var deployment =  Deployments.CreateDeployment(DevOpsTestProject.ProjectName, releaseVersion, 
                 settings);

             Deployments.PromoteRelease(DevOpsTestProject.ProjectName, EnvironmentData.SPRINTDEV,
                 EnvironmentData.SPRINTQA);
         }

         // projects in the group to promote the latest release from one env to another (Unspecified the release version)
         static void PromoteMultiProjects()
         {
             var settings = new GroupDeploymentSettings()
             {
                 DeployAt = new DateTime(2019,1, 18,0, 0,0),
                 ProjectsToExclude = new []{"WebApiMap", "", ""},
                 WaitingForFinish = false,
                 SourceEnvironmentName = EnvironmentData.SPRINTDEV,
                 TargetEnvironmentName = EnvironmentData.SPRINTQA
             };
             ChainDeployments.DeployProjectGroup("Data Services", settings);
         }
    }
}