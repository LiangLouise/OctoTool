using System;
using System.Linq;
using Octopus.Client.Model;
using OctoTool.Exception;
using OctoTool.SettingExtensions;

namespace OctoTool
{
    public class Releases
    {
        public static ReleaseResource CreateRelease(string projectName,CreateReleaseSettings settings)
        {
            var client = WebClient.GetWebClientRef();
            var project = new OctoProject(projectName);
            var targetChannel = project.GetChannelByName(settings.ReleaseChannel);
            var process = client.GetDeploymentProcessRepo().Get(project.GetDeploymentProcessId());
            var template = client.GetDeploymentProcessRepo().GetTemplate(process,targetChannel);

            if (project.IsDisabled() && !settings.IgnoreChannelRules)
            {
                return null;
            }
            
            // If no version set up in the createReleaseSettings, Create a version number by using template
            if (string.IsNullOrEmpty(settings.ReleaseVersion)){
                settings.ReleaseVersion = template.NextVersionIncrement;
            }
            
            // Initialize the new release with basic settings
            var newRelease = new ReleaseResource{
                ProjectId = project.GetProjectId(),
                ChannelId = targetChannel.Id,
                Version = settings.ReleaseVersion,
                ReleaseNotes = settings.ReleaseNotes
            };
            
            Console.WriteLine("Create Release Version: {0} of {1} at {2}", settings.ReleaseVersion, projectName, targetChannel.Name);

            var feedRepo = client.GetFeedRepo();
            // Select packages 
            foreach (var package in template.Packages)
            {
                var actionName = package.ActionName;
                var packageId = package.PackageId;                
                var feedId = package.FeedId;
                var selectedPackage = new SelectedPackage(){ActionName = actionName};
                // If the package version is not specify, choose the one with the latest version at the current channel
                if (settings.Packages is null)
                {
                    var feed = feedRepo.Get(feedId);
                    var latestPackageVersion = feedRepo.GetVersions(feed, new[] { packageId }).FirstOrDefault();
                    
                    // When the package needed doesn't exist on Octopus repo
                    if (latestPackageVersion is null)
                    {               
                        throw new PackageNotFoundException(
                            $"Please upload package: {packageId} to Octopus repo");
                    }
                    selectedPackage.Version = latestPackageVersion.Version;
                    
                }
                else
                {
                    selectedPackage.Version = settings.Packages[package.PackageId];
                }
                
                Console.WriteLine("{0}: {1} {2}", actionName, packageId, selectedPackage.Version);
                
                newRelease.SelectedPackages.Add(selectedPackage);
            }
            
            return client.GetReleaseRepo().Create(newRelease, settings.IgnoreChannelRules);
        }
            
    }
}