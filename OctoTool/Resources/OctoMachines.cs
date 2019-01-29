using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Octopus.Client.Model;

namespace OctoTool
{
    public class OctoMachines
    {
        public List<MachineResource> Machines;
        public List<string> IdList;
        public List<string> NameList;

        private OctoMachines(List<MachineResource> machines)
        {
            Machines = machines;
            GetLists();
        }
               
        public static OctoMachines GetMachinesByEnvName(string environmentName, List<string> roles = null)
        {

            var env = WebClient.GetWebClientRef().GetEnvironmentByName(environmentName);
            var machines = WebClient.GetWebClientRef().GetEnvironmentRepo().GetMachines(env);
            List<MachineResource> res = new List<MachineResource>();
            if (roles == null)
            {
                res = machines;
            }
            else
            {   
                
                foreach (var machine in machines)
                {
                    if (machine.Roles.Overlaps(roles))
                    {
                        res.Add(machine);
                    }
                }
            }
            return new OctoMachines(res);
        }

        public static OctoMachines GetMachinesByEnvName(string environmentName, string[] roles)
        {
            return GetMachinesByEnvName(environmentName, roles.ToList());
        }

        public static OctoMachines GetMachinesByMachineName(List<string> machineNames)
        {
            List<MachineResource> res = new List<MachineResource>();
            foreach (var machineName in machineNames)
            {
                res.Add(WebClient.GetWebClientRef().GetMachineByName(machineName));
            }
            return new OctoMachines(res);
        }

        public static OctoMachines GetMachinesByMachineName(string machineName)
        {
            var machineNames = new List<string> {machineName};
            return GetMachinesByMachineName(machineNames);
        }        

        private void GetLists()
        {
            IdList = new List<string>();
            NameList = new List<string>();
            foreach (var machine in Machines)
            {
                NameList.Add(machine.Name);
                IdList.Add(machine.Id);   
            }
        }

        public OctoTask CheckConnectivityToMachines(string description = null)
        {
            var repo = WebClient.GetWebClientRef().GetTaskRepo();
            description = description ?? $"Checking Connectivity to {string.Join(", ", NameList.Take(4).ToArray())}";
            var timeOutAfterMinutes = int.Parse(ConfigurationManager.AppSettings["TimeOutAfterMinutes"]);
            var machineTimeoutAfterMinutes = int.Parse(ConfigurationManager.AppSettings["MachineTimeoutAfterMinutes"]);
            var task = new OctoTask(repo.ExecuteHealthCheck(description, timeOutAfterMinutes, machineTimeoutAfterMinutes, 
                null,
                IdList.ToArray()));
            task.PrintCurrentState();
            return task;
        }

        public bool WaitForMachinesBackOnline(int checkInterval = 2, int runningTime = 10)
        {
            var task = CheckConnectivityToMachines();
            var i = 0;
            while (task.GetResultState() != TaskState.Success && i < runningTime)
            {
                double time = TimeSpan.FromMinutes(checkInterval).TotalMilliseconds;
                Thread.Sleep(Convert.ToInt32(time));
                i += 1;
                task.ReRun();
            }

            return task.GetResultState() == TaskState.Success;
        }

        public OctoTask ExecuteScripts(string scriptBody, string description = null, string syntaxType = "PowerShell")
        {
            var repo = WebClient.GetWebClientRef().GetTaskRepo();
            description = description ?? $"Running Scripts against {string.Join(", ", NameList.Take(4).ToArray())}";
            var task = new OctoTask(repo.ExecuteAdHocScript(scriptBody, IdList.ToArray(), null, null, description, syntaxType));
            task.PrintCurrentState();
            return task;
        }
        /// <summary>
        /// Restart the servers now. And waiting for it bo back online
        /// </summary>
        /// <returns></returns>
        public bool RestartServer()
        {
            Console.Out.WriteLine($"Start to Restarts Servers: {string.Join(", ", NameList.Take(4).ToArray())}");
            ExecuteScripts(ConfigurationManager.AppSettings["RestartScript"]);
            return WaitForMachinesBackOnline();
        }       
    }
}