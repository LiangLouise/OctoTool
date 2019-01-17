using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Repositories;
using OctoTool.Exception;
using ITaskRepository = Octopus.Client.Repositories.Async.ITaskRepository;

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

        public ProjectResource GetProjectByName(string projectName)
        {
            return _repo.Projects.FindByName(projectName);
        }

        public ProjectGroupResource GetGroupByName(string groupName)
        {
            return _repo.ProjectGroups.FindByName(groupName);
        }

        public MachineResource GetMachineByName(string machineName)
        {
            return _repo.Machines.FindByName(machineName);
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

    }
}