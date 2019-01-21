using System;
using Octopus.Client.Model;

namespace OctoTool
{
    public class OctoTask
    {
        public TaskResource Task;
        public TaskDetailsResource TaskDetails;
        
        public OctoTask(string taskId)
        {
            var taskRepo = WebClient.GetWebClientRef().GetTaskRepo();
            Task = taskRepo.Get(taskId);
            TaskDetails = taskRepo.GetDetails(Task);
        }

        public OctoTask(DeploymentResource deployment)
        {
            Task = WebClient.GetWebClientRef().GetOctopusRepository().Tasks.Get(deployment.TaskId);
        }

        public OctoTask(TaskResource task)
        {
           Task = task;
        }
        
        public static OctoTask GetDeploymentTask(DeploymentResource deployment)
        {
            return new OctoTask(deployment);
        }

        public DateTimeOffset? GetTaskFinishedTime()
        {
            return Task.CompletedTime;
        }
        
        public void PrintCurrentState(bool waitForCompletion = true)
        {
            var startTime = Task.StartTime ?? DateTimeOffset.Now;
            var taskRepo = WebClient.GetWebClientRef().GetOctopusRepository().Tasks;
            Console.WriteLine($"{Task.Name} Starts will start at {startTime}");
            
            if (!waitForCompletion) return;

            taskRepo.WaitForCompletion(Task);

            Task = taskRepo.Refresh(Task);
            var endTime = DateTimeOffset.Now;
            switch (Task.State)
            {
                case TaskState.Success:
                    Console.WriteLine("Task is finished successfully at {0}, taking {1}", endTime.LocalDateTime,
                        endTime - startTime);
                    break;
                case TaskState.Failed:
                    Console.WriteLine("Task failed at {0}, taking {1}", endTime.LocalDateTime,
                        endTime - startTime);
                    Console.WriteLine(Task.ErrorMessage);
                    break;
                case TaskState.Canceled:
                    Console.WriteLine("Task got canceled at {0}, taking {1}", endTime.LocalDateTime,
                        endTime - startTime);
                    break;
            }
        }

        
    }
}