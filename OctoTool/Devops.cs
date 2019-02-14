using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using OctoTool.Data;
using OctoTool.SettingExtensions;

namespace OctoTool.Scripts
{
    public class Devops
    {
        public string SourceEnvironmentName;
        public string TargetEnvironmentName;
        public JObject Configs;

        public Devops(string sourceEnvName, string targetEnvName, string pathToSettings = "DevopsProjects.json")
        {
            SourceEnvironmentName = sourceEnvName;
            TargetEnvironmentName = targetEnvName;
            Configs = DataHelpers.GetJsonContent(pathToSettings);            
        }

        public void CreateDeployment()
        {
            ProvisionServers();
            PromoteGroups();
            PromoteProjects();
        }
            
        public void ProvisionServers()
        {
            var devops = Configs.Property("DevOpsProjects");
            if(devops == null) return;
            
            var devopsProjects = devops.Value;
                       
            if (!devopsProjects.Contains("NeedReboot")) return;
            var needReboot = devopsProjects["NeedReboot"];
            var settings = new MultiReleasePromotingSettings()
            {
                SourceEnvironmentName = SourceEnvironmentName,
                TargetEnvironmentName = TargetEnvironmentName,
                WaitingForFinish = true,
                NeedRebootAfterDeployment = true
            };
            ChainDeployments.PromoteReleases(needReboot.ToObject<string[]>(), settings);

            if (!devopsProjects.Contains("others")) return;
            var others = devopsProjects["others"];
            settings.NeedRebootAfterDeployment = false;                            
            ChainDeployments.PromoteReleases(others.ToObject<string[]>(), settings);

        }

        public void PromoteGroups()
        {
            var groups = Configs.Property("ProjectsGroupsToDeploy");
            if (groups == null) return;

            foreach (var g in groups.Value)
            {
                var name = g["Name"];
                
                if(name is null){continue;}                
                Console.WriteLine($"Start to deploy ProjectsGroup {name}");
                var time = g["DeployAt"];
                var settings = new GroupPromotingSettings
                {
                    SourceEnvironmentName = SourceEnvironmentName,
                    
                    TargetEnvironmentName = TargetEnvironmentName,                  
                    
                    SpecificMachineNames = g["SpecificMachineNames"]?.ToObject<string[]>() ?? new string[0],
                    
                    ProjectsToExclude = g["ProjectsToExclude"]?.ToObject<string[]>() ?? new string[0],
                    
                    SpecificProjectsToInclude = g["SpecificProjectsToInclude"]?.ToObject<string[]>() ?? new string[0],
                    
                    WaitingForFinish = g["WaitingForFinish"]?.ToObject<bool>() ?? false,
                    
                    UseGuidedFailure = g["UseGuidedFailure"]?.ToObject<bool>() ?? false,
                    
                    UpdateVariableSetNow = g["UpdateVariableSetNow"]?.ToObject<bool>() ?? false,
                    
                    Force = g["Force"]?.ToObject<bool>() ?? false,
                    
                    DeployAt = time == null ? DateTime.Now  : DateTime.Parse(time.ToString())
                    
                };
                ChainDeployments.PromoteProjectGroup(name.ToString(), settings);
            }
        }

        public void PromoteProjects()
        {
            var projects = Configs.Property("ProjectsToDeploy");
            if (projects == null) return;
            Console.WriteLine("Start to deploy projects");
            foreach (var g in projects.Value)
            {
                var name = g["Name"];
                
                if(name is null){continue;}
                
                var time = g["DeployAt"];
                var settings = new SingleReleasePromotingSettings()
                {
                    SourceEnvironmentName = SourceEnvironmentName,
                    
                    TargetEnvironmentName = TargetEnvironmentName,
                    
                    SpecificMachineNames = g["SpecificMachineNames"]?.ToObject<string[]>() ?? new string[0],
                    
                    SkipSteps = g["SkipSteps"]?.ToObject<string[]>() ?? new string[0],
                    
                    WaitingForFinish = g["WaitingForFinish"]?.ToObject<bool>() ?? false,
                    
                    UpdateVariableSetNow = g["UpdateVariableSetNow"]?.ToObject<bool>() ?? false,
                    
                    Force = g["Force"]?.ToObject<bool>() ?? false,
                    
                    DeployAt =  time == null ? DateTime.Now  : DateTime.Parse(time.ToString())
                    
                };
                Deployments.PromoteRelease(name.ToString(), settings);
            }
        }
    }
}