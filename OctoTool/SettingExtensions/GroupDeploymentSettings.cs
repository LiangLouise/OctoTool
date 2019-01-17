namespace OctoTool.SettingExtensions
{
    public class GroupDeploymentSettings: PromoteReleaseSettings
    {
        public string[] ProjectsToExclude = new string[0];    
        public string[] SpecificProjectsToInclude = new string[0];
        public GroupDeploymentSettings(){}
    }
}