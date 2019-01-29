using System;
using Newtonsoft.Json.Linq;
using OctoTool.Data;
using OctoTool.SettingExtensions;

namespace OctoTool.Scripts
{
    public class Devops
    {
        public OctoMachines ServersToReboot;

        public string SourceEnvironmentName;
        public string TargetEnvironmentName;
        public JObject Configs;

        public Devops(string sourceEnvName, string targetEnvName, string pathToSettings = "DevopsProjects.json")
        {
            SourceEnvironmentName = sourceEnvName;
            TargetEnvironmentName = targetEnvName;
            Configs = DataHelpers.GetJsonContent(pathToSettings);
            ServersToReboot = OctoMachines.GetMachinesByEnvName(targetEnvName, new 
                []{RolesData.IIS, RolesData.MSMQ});
        }

        public void CreateDeployment()
        {
            PromoteGroups();
            PromoteProjects();
        }
            
        public void ProvisionServers()
        {
            var devops = Configs.Property("DevOpsProjects");
            if(devops == null) return;
            
            var devopsProjects = devops.Value;
            
            var needReboot = devopsProjects["NeedReboot"];
            if (needReboot == null) return;
            var settings = new MultiReleasePromotingSettings()
            {
                SourceEnvironmentName = SourceEnvironmentName,
                TargetEnvironmentName = TargetEnvironmentName,
                WaitingForFinish = true,
                NeedRebootAfterDeployment = true
            };
            ChainDeployments.PromoteReleases(needReboot.ToObject<string[]>(), settings);

            var others = devopsProjects["others"];            
            if (others == null) return;
            settings.NeedRebootAfterDeployment = false;                            
            ChainDeployments.PromoteReleases(devopsProjects["others"].ToObject<string[]>(), settings);

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