using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace OctoTool.SettingExtensions
{
    public abstract class DeploymentSettings
    {
        public string TargetEnvironmentName { get; set; }
        public string[] SpecificMachineNames { get; set; }
        public string[] SkipSteps { get; set; }
        public DateTime DeployAt { get; set; }
        
        public bool Force { get; set; }
        public bool UseGuidedFailure { get; set; }
        public bool WaitingForFinish = true;
        public string Comments { get; set; }
        

        public DeploymentSettings(){}

        public ReferenceCollection ConvertMachineNames()
        {
            if (SpecificMachineNames is null)
            {
                return null;
            }
            var client = WebClient.GetWebClientRef();
            string[] idList = SpecificMachineNames;
            for (var i = 0; i < idList.Length; i++)
            {
                idList[i] = client.GetMachineByName(SpecificMachineNames[i]).Id;
            }
            return new ReferenceCollection(ConvertList(idList));
        }

        public ReferenceCollection ConvertSkipSteps()
        {
            return SkipSteps is null ? null : new ReferenceCollection(ConvertList(SkipSteps));
        }

        private IEnumerable<string> ConvertList(string[] list)
        {
            return list;
        }
    }
}