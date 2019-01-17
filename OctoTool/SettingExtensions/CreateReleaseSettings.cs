using System.Collections.Generic;

namespace OctoTool.SettingExtensions
{
    public class CreateReleaseSettings
    {
        public string ReleaseChannel{ get; set;}
        public string ReleaseVersion{ get; set;}
        // Package Name: Package Version
        public Dictionary<string, string> Packages { get; set;}

        public string ReleaseNotes { get; set; }
        public bool IgnoreChannelRules { get; set; }

        public CreateReleaseSettings()
        {
          
        }
    }
    
   
}