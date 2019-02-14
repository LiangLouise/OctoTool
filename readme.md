# OctoTool

A Console App Used to help octopus deployment automation.
Useful when preparing a new environment.

## Usage

At CMD Console
  ```
  OctoTool.exe [SourceEnvironmentName] [TargetEnvironmentName] <ConfigurationJsonFilePath>
  ```
* `SourceEnvironmentName`: The Environment that has been deployed the releases need to be promoted;
* `TargetEnvironmentName`: The Target Environment that will be deployed by the release from the source environment;
* `ConfigurationJsonFilePath`: the path to Json file that control the deployments, by default, the name is 
`DeploymentsConfigs.json`

## Config
1. `DevOpsProjects` section:
    ```json
     "DevOpsProjects": {
        "NeedReboot": [
        "<Projecet Name>",
        "<Projecet Name>"
        ],
        "others": [
        "<Projecet Name>",
        "<Projecet Name>"
        ]
     }
   ```
   In this section, each project will start after the previous one finished deployment, so they will deployed in order
   * In section `NeedReboot`, after each project finishes deployment, the deployment target will reboot. Only after 
   every target machine back online, the next one will start.
   * In section `others`, every projects will deployed in order but won't reboot the machines.
   
2. `ProjectsGroupsToDeploy` section:
    ```json
     "ProjectsGroupsToDeploy": [
       {
         "Name": "<Group Name>",
         "SpecificMachineNames": ["<Machine Name>"],
         "ProjectsToExclude": ["<Projecet Name>", "<Projecet Name>"],
         "SpecificProjectsToInclude": ["<Projecet Name>"],
         "WaitingForFinish": "false",
         "UpdateVariableSetNow": "true",
         "UseGuidedFailure": "false",
         "Force": "false",
         "DeployAt": "1/30/2019 00:00:00"
       },
       {                  
         "Name": "<Group Name>",             
         "WaitingForFinish": false,
         "UpdateVariableSetNow": false,
         "UseGuidedFailure": "false",
         "DeployAt": "1/30/2019 00:00:00"
       },
       {                  
         "Name": "<Group Name>"                                
       }      
     ]
    ```
    In this section, projects belong to the groups in the Json Array `"ProjectsGroupsToDeploy"` will be deployed. 
    
    If the project in the group has no release deployed to the source environment or is disabled, it will be 
    skipped by default. The deployments start by following the alphabetical order of projects' name.
    
    Each group's deployment is controlled by following options:
    * `SpecificMachineNames`: If specify, the projects will be only deployed to the machines in the list.
    * `ProjectsToExclude`: If specify, the projects in the list will be exclude from being deployed.
    * `SpecificProjectsToInclude`: If specify, only the projects in the list will be deployed.
    * `WaitingForFinish`: Wait for each project finishing then start next project's deployment, by default, it will 
    be set `"false"` or `false`.
    * `UpdateVariableSetNow`: Update the release variables snapshot before starting the deployment, by default, it will 
    be set `"false"` or `false`.
    * `Force`: Force the disabled projects to deploy to the target environment, by default, it will 
    be set `"false"` or `false`.
    * `UseGuidedFailure`: Use guided failure mode to deploy to the target environment, by default, it will be set 
    `"false"` or `false`.
    * `DeployAt`: Schedule the deployment start time. If the time is smaller then now, the deployment will start 
    immediately. If the time is greater than now, after Scheduling deployment, program will start next project 
    deployment.

3. `ProjectsToDeploy` section:
    ```json
     "ProjectsToDeploy": [
       { 
         "Name": "<Projecet Name>",
         "SpecificMachineNames": ["<Machine Name>"],
         "SkipSteps": ["<Step Name>", "<Step Name>"],
         "WaitingForFinish": false,
         "UpdateVariableSetNow": false,
         "Force": false,
         "DeployAt": "1/30/2019 00:00:00"
       },
       {
         "Name": "<Projecet Name>"
       }
     ] 
    ```
    In this section, projects in the Json Array `"ProjectsToDeploy"` will be deployed. 
        
    If the project has no release deployed to the source environment or is disabled, it will be 
    skipped by default. The deployments start from the first one to the last one in the list.
    
    Each project deployment is controlled by following options:
    * `SpecificMachineNames`: If specify, the projects will be only deployed to the machines in the list.
    * `SkipSteps`: If specify, the steps in the list will be exclude from being executed.
    * `WaitingForFinish`: Wait for each project finishing then start next project's deployment, by default, it will 
    be set `"false"` or `false`.
    * `UpdateVariableSetNow`: Update the release variables snapshot before starting the deployment, by default, it will 
    be set `"false"` or `false`.
    * `Force`: Force the disabled projects to deploy to the target environment, by default, it will 
    be set `"false"` or `false`.
    * `UseGuidedFailure`: Use guided failure mode to deploy to the target environment, by default, it will be set 
    `"false"` or `false`.
    * `DeployAt`: Schedule the deployment start time. If the time is smaller then now, the deployment will start 
    immediately. If the time is greater than now, after Scheduling deployment, program will start next project 
    deployment.
    
    
    