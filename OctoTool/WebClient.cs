using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Repositories;
using OctoTool.Exception;

namespace OctoTool
{
    public class WebClient
    {
        private static WebClient _reference;

        private OctopusRepository _repo;
        private OctopusClient _client;
        
        private WebClient(string url, string apiKey)
        {
            var endpoint = new OctopusServerEndpoint(url, apiKey);
            _repo = new OctopusRepository(endpoint);
            _client = (OctopusClient) _repo.Client;
        }

        public static WebClient CreateWebClientRef(string url, string apiKey)
        {
            return _reference ?? (_reference = new WebClient(url, apiKey));
        }

        public static WebClient GetWebClientRef()
        {
            if (_reference is null)
            {
                throw new OctopusRepoNotConnectedException("Please Connect to Your Octo Server first!");
            }
            return _reference;
        }

        public OctopusRepository GetOctopusRepository()
        {
            return _repo;
        }

        public OctopusClient GetOctopusClient()
        {
            return _client;
        }

        public IProjectRepository GetProjectRepo()
        {
            return _repo.Projects;
        }

        public ProjectResource GetProjectByName(string projectName)
        {
            return GetProjectRepo().FindByName(projectName);
        }

        public ProjectGroupResource GetGroupByName(string groupName)
        {
            return _repo.ProjectGroups.FindByName(groupName);
        }

        public MachineResource GetMachineByName(string machineName)
        {
            return _repo.Machines.FindByName(machineName);
        }
        
        public IMachineRepository GetMachineRepo()
        {
            return _repo.Machines;
        }
      
        public IReleaseRepository GetReleaseRepo()
        {
            return _repo.Releases;
        }

        public IDeploymentRepository GetDeploymentRepo()
        {
            return _repo.Deployments;
        }

        public IDeploymentProcessRepository GetDeploymentProcessRepo()
        {
            return _repo.DeploymentProcesses;
        }

        public IFeedRepository GetFeedRepo()
        {
            return _repo.Feeds;
        }

        public ITaskRepository GetTaskRepo()
        {
            return _repo.Tasks;
        }

        public IEnvironmentRepository GetEnvironmentRepo()
        {
            return _repo.Environments;
        }

        public IChannelRepository GetChannelRepo()
        {
            return _repo.Channels;
        }
        
        public EnvironmentResource GetEnvironmentByName(string environmentName)
        {
            return GetEnvironmentRepo().FindByName(environmentName);
        }

        public string GetEnvironmentIdByName(string environmentName)
        {
            return GetEnvironmentByName(environmentName).Id;
        }
    }
}