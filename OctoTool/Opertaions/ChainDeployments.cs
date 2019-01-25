using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Octopus.Client.Model;
using OctoTool.Exception;
using OctoTool.SettingExtensions;

namespace OctoTool
{
    public class ChainDeployments
    {
        /// <summary>
        /// Deploy every project in a project group. It will promote releases of every project in the input group with
        /// given source Env to Target Env. By default, the deployment will start immediately, skip the projects are
        /// disabled and doesn't have the deployment at the input env and follow the alphabetical order of projects'
        /// name.
        /// The mandatory vars in settings are SourceEnvironmentName and TargetEnvironmentName
        /// </summary>
        /// <param name="groupName">The project group needed to be pushed</param>
        /// <param name="settings">Use GroupDeploymentSettings to pass special requirement. i.e. queue time</param>
        /// <returns>a list of the deploymentRecourse</returns>
        /// <exception cref="DeploymentSettingsMissingException">If one of Env name is missing</exception>
        public static List<DeploymentResource> PromoteProjectGroup(string groupName, GroupPromotingSettings 
        settings)
        {
            var group = WebClient.GetWebClientRef().GetGroupByName(groupName);
            var projects = WebClient.GetWebClientRef().GetOctopusRepository().ProjectGroups.GetProjects(group);
            var deployments = new List<DeploymentResource>();

            if (settings.SourceEnvironmentName == null || settings.TargetEnvironmentName == null)
            {
                throw new DeploymentSettingsMissingException("Please make sure both envs name are set up");
            }
            
            if(settings.SpecificProjectsToInclude.Length == 0)
            {
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
        
        /// <summary>
        /// Promote Releases of a variety of project in the list projectNames from sourceEnv To targetEnv.
        /// By default, it will begin to deploy every projects immediately.
        /// The mandatory vars in settings are SourceEnvironmentName and TargetEnvironmentName
        /// </summary>
        /// <param name="projectNames">The list of projects name</param>
        /// <param name="settings">MultiReleasePromotingSettings to control the deployment</param>
        /// <returns>a list of the deploymentRecourse</returns>
        public static List<DeploymentResource> PromoteReleases(string[] projectNames, 
                                                               MultiReleasePromotingSettings settings)
        {    
            if (settings.SourceEnvironmentName == null || settings.TargetEnvironmentName == null)
            {
                throw new DeploymentSettingsMissingException("Please make sure both envs name are set up");
            }
            
            var deployments = new List<DeploymentResource>();
            
            foreach (var projectName in projectNames)
            {
                deployments.Add(Deployments.PromoteRelease(projectName, settings));
            }

            return deployments;
        }

        /// <summary>
        /// Deploy a list of projects with different version. By default, it will follow the Channel rule, so it will
        /// fail to deploy the release not following the rules.
        /// The mandatory var in settings is TargetEnvironmentName
        /// </summary>
        /// <param name="projectsDict">An OrderedDictionary where each pair's key is project name and</param>
        /// <param name="settings">a MultiProjectsDeploymentSettings instance to control the deployments</param>
        /// <returns>a list of the deploymentRecourse</returns>
        public static List<DeploymentResource> DeployProjects(OrderedDictionary projectsDict, 
            MultiProjectsDeploymentSettings settings)
        {

            if (settings.TargetEnvironmentName == null)
            {
                throw new DeploymentSettingsMissingException("Please make sure TargetEnvironmentName is set up");
            }
            
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