using Octopus.Client.Model;

namespace OctoTool.SettingExtensions
{
    public sealed class SingleProjectDeploymentSettings : MultiProjectsDeploymentSettings
    {
        public string[] SkipSteps { get; set; }
        public SingleProjectDeploymentSettings(){}


        public SingleProjectDeploymentSettings(SingleReleasePromotingSettings settings)
        {
            SkipSteps = settings.SkipSteps;
            TargetEnvironmentName = settings.TargetEnvironmentName;
            SpecificMachineNames = settings.SpecificMachineNames;
            DeployAt = settings.DeployAt;
            WaitingForFinish = settings.WaitingForFinish;
            Force = settings.Force;
            UseGuidedFailure = settings.UseGuidedFailure;
            UpdateVariableSetNow = settings.UpdateVariableSetNow;
            Comments = settings.Comments;
        }

        public ReferenceCollection ConvertSkipSteps()
        {
            return SkipSteps is null ? null : new ReferenceCollection(ConvertList(SkipSteps));
        }
    }
}