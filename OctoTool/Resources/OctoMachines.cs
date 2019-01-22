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

        private OctoMachines(List<MachineResource> machines)
        {
            Machines = machines;
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
            List<string> machineNames = new List<string>();
            machineNames.Add(machineName);
            return GetMachinesByMachineName(machineNames);
        }

        public List<string> GetMachinesIdList()
        {
            List<string> IdList = new List<string>();
            foreach (var machine in Machines)
            {
                IdList.Add(machine.Id);   
            }

            return IdList;
        }

        public List<string> GetMachinesNameList()
        {
            List<string> nameList = new List<string>();
            foreach (var machine in Machines)
            {
                nameList.Add(machine.Name);   
            }

            return nameList;
        }

        public OctoTask CheckConnectivityToMachines(string description = null)
        {
            var client = WebClient.GetWebClientRef();
            description = description ?? $"Checking Connectivity to {string.Join(", ", GetMachinesNameList().Take(4).ToArray())}";
            var idList = GetMachinesIdList();
            var repo = client.GetTaskRepo();
            var timeOutAfterMinutes = int.Parse(ConfigurationManager.AppSettings["TimeOutAfterMinutes"]);
            var machineTimeoutAfterMinutes = int.Parse(ConfigurationManager.AppSettings["MachineTimeoutAfterMinutes"]);
            var task = new OctoTask(repo.ExecuteHealthCheck(description, timeOutAfterMinutes, machineTimeoutAfterMinutes, 
                null,
                idList.ToArray()));
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
    }
}