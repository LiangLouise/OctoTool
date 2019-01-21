using System;
using Octopus.Client.Model;
using OctoTool.SettingExtensions;

namespace OctoTool
{
    public static class Deployments
    {
        public static DeploymentResource CreateDeployment(OctoProject project, ReleaseResource release, 
        SingleProjectDeploymentSettings settings)
        {   
            var client = WebClient.GetWebClientRef();
            // Check if the project has been disabled
            if (project.IsDisabled() || settings.Force)
            {
                return null;
            }

            // Update the variable set           
            if (settings.UpdateVariableSetNow)
            {
                release = client.GetReleaseRepo().SnapshotVariables(release);
            }
            
            //creating the deployment object
            var deployment = new DeploymentResource
            {
                ReleaseId = release.Id,
                ProjectId = project.GetProjectId(),
                EnvironmentId = Environments.GetEnvironmentIdByName(settings.TargetEnvironmentName),
                SpecificMachineIds = settings.ConvertMachineNames(),
                Comments = settings.Comments,
                UseGuidedFailure = settings.UseGuidedFailure,
                SkipActions = settings.ConvertSkipSteps()
                
            };
            // Schedule the deployment if deployment time is greater than now
            if (settings.DeployAt > DateTime.Now)
            {
                deployment.QueueTime = new DateTimeOffset(settings.DeployAt);
                settings.WaitingForFinish = false;
            }            
            deployment = client.GetDeploymentRepo().Create(deployment);
            
            Console.WriteLine($"{project.ProjectName} will {deployment.Name}");
            
            var task = new OctoTask(deployment);
            task.PrintCurrentState(settings.WaitingForFinish);
            return deployment;
        }

        public static DeploymentResource CreateDeployment(OctoProject project, ReleaseResource release,
            string targetEnvironmentName)
        {
            var settings = new SingleProjectDeploymentSettings {TargetEnvironmentName = targetEnvironmentName};
            return CreateDeployment(project, release, settings);
        }

        public static DeploymentResource CreateDeployment(string projectName, string releaseVersion,
            SingleProjectDeploymentSettings settings)
        {
            var project = new OctoProject(projectName);
            var release = project.GetReleaseByVersion(releaseVersion);
            return CreateDeployment(project, release, settings);
        }

        public static DeploymentResource CreateDeployment(string projectName, string releaseVersion, string targetEnvironmentName)
        {
            var project = new OctoProject(projectName);
            var release = project.GetReleaseByVersion(releaseVersion);
            var settings = new SingleProjectDeploymentSettings {TargetEnvironmentName = targetEnvironmentName};
            return CreateDeployment(project, release, settings);
        }
        
        /// <summary>
        /// Promote Release from one environment to another. If the releaseVersion is not specified.
        /// The release has the latest deployment at sourceEnv will be chosen to deploy to the targetEnv.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="sourceEnv"></param>
        /// <param name="targetEnv"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static DeploymentResource PromoteRelease(OctoProject project, EnvironmentResource sourceEnv, 
        EnvironmentResource targetEnv, SingleReleasePromotingSettings settings)
        {
            if (settings.Release == null)
            {
                var ReleaseRepo = WebClient.GetWebClientRef().GetOctopusRepository().Releases;
                var sourceDeploy = project.GetDeployment(sourceEnv);
                settings.Release = ReleaseRepo.Get(sourceDeploy.ReleaseId);
            }
            return CreateDeployment(project, settings.Release, targetEnv.Name);
        }
        
        public static DeploymentResource PromoteRelease(string projectName, string releaseVersion, SingleReleasePromotingSettings settings)
        {
            var project = new OctoProject(projectName);
            var sourceEnv = Environments.GetEnvironmentByName(settings.SourceEnvironmentName);
            var targetEnv = Environments.GetEnvironmentByName(settings.TargetEnvironmentName);
            settings.Release = releaseVersion is null ? null : project.GetReleaseByVersion(releaseVersion);
            return PromoteRelease(project, sourceEnv, targetEnv, settings);
        }
        
        public static DeploymentResource PromoteRelease(string projectName, string sourceEnvName, string targetEnvName,
                    string releaseVersion = null)
                {
                    var settings = new SingleReleasePromotingSettings()
                    {
                        SourceEnvironmentName = sourceEnvName,
                        TargetEnvironmentName = targetEnvName,
                    };
                    return PromoteRelease(projectName, releaseVersion, settings);
                }
        
        public static DeploymentResource PromoteRelease(string projectName, SingleReleasePromotingSettings settings)
        {
            return PromoteRelease(projectName, null, settings);
        }
        
        
    }
}