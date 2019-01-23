namespace OctoTool.SettingExtensions
{
    public class MultiReleasePromotingSettings: SingleReleasePromotingSettings
    {
        
        private new string[] SkipSteps;

        public override bool WaitingForFinish { get; set; } = false;

        public MultiReleasePromotingSettings()
        {
            
        }
        
    }
}