using System.Collections.Generic;
using System.Configuration;
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

        public IList<DeploymentResource> GetAllDeployment(ReleaseResource release)
        {
            return WebClient.GetWebClientRef().GetOctopusRepository().Releases
                .GetDeployments(release).Items;
        }

        public IList<DeploymentResource> GetAllDeployment(string releaseVersion)
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
                 return GetAllDeployment(releaseResource).FirstOrDefault(deployment => deployment.EnvironmentId
                  == env.Id);
            }

            DeploymentResource lastDeployment = null;
            foreach (var release in GetAllReleases().Take(int.Parse(ConfigurationManager
            .AppSettings["ReleasesSelectionRange"])))
            {
                var deploy = GetAllDeployment(release).FirstOrDefault(deployment => deployment.EnvironmentId == env.Id);
                if (deploy == null) continue;
                if (lastDeployment == null) 
                {      
                    lastDeployment = deploy;    
                }                        
                if (deploy.Created > lastDeployment.Created)                        
                {                                                  
                    lastDeployment = deploy;                        
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

        public List<string> GetTargetRolesNameList()
        {
            var l = new List<string>();
            var process = WebClient.GetWebClientRef().GetDeploymentProcessRepo().Get(GetDeploymentProcessId());
            var steps = process.Steps;
    
            foreach (var s in steps)
            {
                if(!s.Properties.ContainsKey("Octopus.Action.TargetRoles")){continue;}
                var role = s.Properties["Octopus.Action.TargetRoles"].Value;
                if (!l.Contains(role) || role == null)
                {
                    l.Add(role);
                }
            }            
            return l;
        }
    }
}