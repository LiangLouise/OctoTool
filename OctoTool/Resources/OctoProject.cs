using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Model;

namespace OctoTool
{
    public class OctoProject
    {
        public ProjectResource Curr { get; set; }
        public readonly string ProjectName;

        public OctoProject(ProjectResource project)
        {
            Curr = project;
            ProjectName = Curr.Name;
        }

        public OctoProject(string projectName)
        {
            Curr = WebClient.GetWebClientRef().GetProjectByName(projectName);
            ProjectName = Curr.Name;
        }

        public string GetProjectId()
        {
            return Curr.Id;
        }

        public bool IsDisabled()
        {
            return Curr.IsDisabled;
        }

        public IReadOnlyList<ReleaseResource> GetAllReleases()
        {
            return WebClient.GetWebClientRef().GetOctopusRepository().Projects.GetAllReleases(Curr);
        }

        public ResourceCollection<DeploymentResource> GetAllDeployment(ReleaseResource release)
        {
            return WebClient.GetWebClientRef().GetOctopusRepository().Releases
                .GetDeployments(release);
        }

        public ResourceCollection<DeploymentResource> GetAllDeployment(string releaseVersion)
        {
            return GetAllDeployment(GetReleaseByVersion(releaseVersion));
        }

        public ReleaseResource GetReleaseByVersion(string version)
        {
            return GetAllReleases().FirstOrDefault(release => release.Version == version);
        }

        public string GetReleaseIdByVersion(string version)
        {
            return GetReleaseByVersion(version).Id;
        }

        public ChannelResource GetChannelByName(string channelName)
        {
            var repo = WebClient.GetWebClientRef().GetOctopusRepository();
            
            return repo.Channels.FindByName(Curr, channelName);
            
        }
        

        /// <summary>
        /// Get the DeploymentResource at the input environment of specific release version.
        /// If the version is not specified, it will return the last deployment at the input environment
        /// </summary>
        /// <param name="env"></param>
        /// <param name="releaseResource"></param>
        /// <returns></returns>
        public DeploymentResource GetDeployment(EnvironmentResource env, ReleaseResource releaseResource = null)
        {
            
            if (releaseResource != null)
            {  
                 return GetAllDeployment(releaseResource).Items.FirstOrDefault(deployment => deployment.EnvironmentId
                  == env.Id);
            }

            DeploymentResource lastDeployment = null;
            foreach (var release in GetAllReleases().Take(5))
            {
                foreach (var deploy in GetAllDeployment(release).Items)
                {
                    if (env.Id.Equals(deploy.EnvironmentId))
                    {
                        if (lastDeployment == null)
                        {
                            lastDeployment = deploy;
                        }

                        if ((OctoTask.GetDeploymentTask(deploy).GetTaskFinishedTime() ?? DateTimeOffset.Now) >
                            (OctoTask.GetDeploymentTask(lastDeployment).GetTaskFinishedTime() ?? DateTimeOffset.Now))
                        {
                            lastDeployment = deploy;
                        }
                    }
                }
            }
            return lastDeployment;
        }

        public DeploymentResource GetDeployment(string environmentName, string releaseVersion = null)
        {
            var env = WebClient.GetWebClientRef().GetEnvironmentByName(environmentName);
            var release = releaseVersion is null ? null : GetReleaseByVersion(releaseVersion);
            return GetDeployment(env, release);
        }
        
        
        public string GetDeploymentProcessId()
        {
            return Curr.DeploymentProcessId;
        }
       
        public void CreateDeployment(ReleaseResource release, string targetEnvironment)
        {
            var repo = WebClient.GetWebClientRef().GetOctopusRepository();
            var environmentId = WebClient.GetWebClientRef().GetEnvironmentIdByName(targetEnvironment);
            //creating the deployment object
            var deployment = new DeploymentResource
            {
                ReleaseId = release.Id,
                ProjectId = GetProjectId(),
                EnvironmentId = environmentId
            };
            repo.Deployments.Create(deployment);
        }

        public void CreateDeployment(string releaseVersion, string targetEnvironment)
        {
            var release = GetReleaseByVersion(releaseVersion);
            CreateDeployment(release, targetEnvironment);
        }
    }
}