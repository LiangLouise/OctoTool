
using Octopus.Client.Model;

namespace OctoTool.SettingExtensions
{
    public class SingleReleasePromotingSettings : MultiProjectsDeploymentSettings
    {
        public string[] SkipSteps { get; set; }
        public string SourceEnvironmentName { get; set; }
        public ReleaseResource Release { get; set; }

        public SingleReleasePromotingSettings(){}
     
        public ReferenceCollection ConvertSkipSteps()
        {
            return SkipSteps is null ? null : new ReferenceCollection(ConvertList(SkipSteps));
        }
    }
}