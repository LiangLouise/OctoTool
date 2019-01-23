namespace OctoTool.SettingExtensions
{
    public class GroupPromotingSettings: SingleReleasePromotingSettings
    {
        public string[] ProjectsToExclude = new string[0];    
        public string[] SpecificProjectsToInclude = new string[0];

        public override bool WaitingForFinish { get; set; } = false;

        private new string[] SkipSteps;
        public GroupPromotingSettings(){}
    }
}