using Octopus.Client.Model;

namespace OctoTool
{
    public class Environments
    {
        public static EnvironmentResource GetEnvironmentByName(string environmentName)
        {
            var repo = WebClient.GetWebClientRef().GetOctopusRepository();
            return repo.Environments.FindByName(environmentName);
        }

        public static string GetEnvironmentIdByName(string environmentName)
        {
            return GetEnvironmentByName(environmentName).Id;
        }
    }
}