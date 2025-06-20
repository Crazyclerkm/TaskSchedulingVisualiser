using System.Collections.Generic;
using System.Linq;

public class Schedule
{
    private readonly int NumProcessors;
    private readonly int[] ProcessorStartTime;

    private readonly Dictionary<TaskNode, (int, int)> TaskSchedule = [];

    public Schedule(int numProcessors)
    {
        NumProcessors = numProcessors;
        ProcessorStartTime = new int[NumProcessors];
    }

    public int EarliestStartTime(int processorNum, TaskNode task)
    {
        int startTime = ProcessorStartTime[processorNum];
        List<Edge> dependencies = task.Predecessors;

        foreach (Edge dependency in dependencies)
        {
            if (TaskSchedule.TryGetValue(dependency.From, out var value))
            {
                (int dependencyProcessor, int dependencyEndTime) = value;

                if (dependencyProcessor != processorNum)
                {
                    dependencyEndTime += dependency.Weight;
                }



                if (dependencyEndTime > startTime) startTime = dependencyEndTime;
            }
            else
            {
                return -1;
            }
        }

        return startTime;
    }

    public void AddTask(int processorNum, TaskNode task, int startTime)
    {
        int endTime = startTime + task.Weight;

        TaskSchedule[task] = (processorNum, endTime);
        ProcessorStartTime[processorNum] = endTime;
    }

    public void AddTask(int processorNum, TaskNode task)
    {
        int startTime = EarliestStartTime(processorNum, task);
        if (startTime < 0) throw new System.Exception("Task cannot be scheduled"); 
        AddTask(processorNum, task, startTime);
    }

    public override string ToString()
    {
        string outString = $"Number of processors: {NumProcessors}\n";

        foreach (var keyValue in TaskSchedule)
        {
            TaskNode task = keyValue.Key;
            (int processor, int endTime) = keyValue.Value;
            string dependencyString = "";
            if (task.Predecessors.Count > 0) {
                dependencyString = $" [Depends on {{{string.Join(", ", task.Predecessors.Select(x => x.From.Name))}}}]";
            }
            
            outString += $"{task.Name} (Weight={task.Weight}) scheduled on processor {processor+1} at time {endTime - task.Weight}{dependencyString}\n";
        }

        return outString;
    }
}