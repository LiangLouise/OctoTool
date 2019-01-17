using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace OctoTool.SettingExtensions
{
    public class SingleProjectDeploymentSettings : MultiProjectsDeploymentSettings
    {
        public string[] SkipSteps { get; set; }
        public SingleProjectDeploymentSettings(){}

        public ReferenceCollection ConvertSkipSteps()
        {
            return SkipSteps is null ? null : new ReferenceCollection(ConvertList(SkipSteps));
        }
    }
}