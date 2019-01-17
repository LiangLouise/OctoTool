using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Octopus.Client.Model;
using OctoTool.SettingExtensions;

namespace OctoTool
{
    public class ChainDeployments
    {
        public static List<DeploymentResource> DeployProjectGroup(string groupName, GroupDeploymentSettings settings)
        {
            var group = WebClient.GetWebClientRef().GetGroupByName(groupName);
            var projects = WebClient.GetWebClientRef().GetOctopusRepository().ProjectGroups.GetProjects(group);
            var deployments = new List<DeploymentResource>();
            bool notSpecific = !(settings.SpecificProjectsToInclude.Length > 0);
            if(notSpecific){
                foreach (var project in projects)
                {
                    var projectName = project.Name;
                    if (!settings.ProjectsToExclude.Contains(projectName))
                    {
                        deployments.Add(Deployments.PromoteRelease(projectName, settings));
                    }
                }
            }
            else
            {
                foreach (var projectName in settings.SpecificProjectsToInclude)
                {
                    deployments.Add(Deployments.PromoteRelease(projectName, settings));
                }
            }

            return deployments;
        }

        public static List<DeploymentResource> PromoteReleases(string[] projectNames, string sourceEnvName, string targetEnvName,
         bool waitingForFinish = true)
        {    
            var deployments = new List<DeploymentResource>();

            var settings = new PromoteReleaseSettings()
            {
                SourceEnvironmentName = sourceEnvName,
                TargetEnvironmentName = targetEnvName,
                WaitingForFinish = waitingForFinish
            };
            
            foreach (var projectName in projectNames)
            {
                deployments.Add(Deployments.PromoteRelease(projectName, settings));
            }

            return deployments;
        }

        public static List<DeploymentResource> DeployProjects(OrderedDictionary projectsDict, 
            MultiProjectsDeploymentSettings settings)
        {
            var deployments = new List<DeploymentResource>();
            
            foreach (DictionaryEntry entry in projectsDict)
            {
                deployments.Add(Deployments.CreateDeployment(entry.Key.ToString(), entry.Value.ToString(), 
                                (SingleProjectDeploymentSettings) settings));
            }

            return deployments;
        }
    }
}