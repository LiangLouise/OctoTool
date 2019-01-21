using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace OctoTool.SettingExtensions
{
    public class MultiProjectsDeploymentSettings
    {
        public string TargetEnvironmentName { get; set; }
        public string[] SpecificMachineNames { get; set; }
        public DateTime DeployAt { get; set; }
        
        public virtual bool WaitingForFinish { get; set; } = true;
        public bool Force { get; set; }
        public bool UseGuidedFailure { get; set; }
        public bool UpdateVariableSetNow { get; set; }
        public string Comments { get; set; }
        

        public MultiProjectsDeploymentSettings(){}

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

        public IEnumerable<string> ConvertList(string[] list)
        {
            return list;
        }
        
        
    }
}