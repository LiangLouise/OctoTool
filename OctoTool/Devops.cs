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
            ProvisionServers();
            PromoteGroups();
        }
            
        public void ProvisionServers()
        {
            var devops = Configs.Property("DevOpsProjects");
            if(devops == null) return;
            
            var devopsProjects = devops["DevOpsProjects"];
            
            var initial = devopsProjects["initial"];
            if (initial != null)
            {   
                var times = 3;
                for (int i = 0; i < times; i++)
                            {
                                Deployments.PromoteRelease(initial.ToString(), SourceEnvironmentName,
                                    TargetEnvironmentName);
                                ServersToReboot.RestartServer();
                            }
            }

            var others = devopsProjects["others"];
            if (others == null) return;
            var settings = new MultiReleasePromotingSettings
            {
                SourceEnvironmentName = SourceEnvironmentName,
                TargetEnvironmentName = TargetEnvironmentName,
                WaitingForFinish = true
            };
                            
            ChainDeployments.PromoteReleases(devopsProjects["others"].ToObject<string[]>(), settings);

        }

        public void PromoteGroups()
        {
            var groups = Configs.Property("ProjectsGroupsToDeploy");
            if (groups == null) return;
              
            foreach (var g in groups.Value["ProjectsGroups"])
            {
                var name = g["Name"].ToString();
                var time = g["WaitingForFinish"].ToString();
                var settings = new GroupPromotingSettings
                {
                    SourceEnvironmentName = SourceEnvironmentName,
                    
                    TargetEnvironmentName = TargetEnvironmentName,
                    
                    ProjectsToExclude = g["ProjectsToExclude"] == null ? new string[0] : g["ProjectsToExclude"]
                                                        .ToObject<string[]>(),
                    
                    SpecificProjectsToInclude = g["SpecificProjectsToInclude"] == null ? new string[0] :
                        g["SpecificProjectsToInclude"].ToObject<string[]>(),
                    
                    WaitingForFinish = g["WaitingForFinish"]?.ToObject<bool>() ?? false,
                    
                    UpdateVariableSetNow = g["UpdateVariableSetNow"]?.ToObject<bool>() ?? false,
                    
                    DeployAt =  DateTime.Parse(time)
                    
                };
                ChainDeployments.PromoteProjectGroup(name, settings);
            }
        }
    }
}