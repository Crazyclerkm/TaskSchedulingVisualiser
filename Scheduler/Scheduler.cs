using System.Collections.Generic;

public class Scheduler
{

    private readonly Graph TaskGraph;
    private readonly int Processors;

    private readonly List<TaskNode> TopologicalOrder;

    public Scheduler(Graph taskGraph, int numProcessors)
    {
        TaskGraph = taskGraph;
        Processors = numProcessors;

        TopologicalOrder = TaskGraph.TopologicalOrdering();

        if (TopologicalOrder.Count != TaskGraph.Nodes.Count) {
            throw new System.Exception("Task graph contains cycles and is therefore unschedulable");
        }
    }


    public Schedule ListSchedule()
    {
        Schedule schedule = new(Processors);

        foreach (TaskNode task in TopologicalOrder)
        {
            int earliestStartTime = schedule.EarliestStartTime(0, task);
            int processor = 0;
            for (int i = 1; i < Processors; i++)
            {
                int processorStartTime = schedule.EarliestStartTime(i, task);

                if (processorStartTime < earliestStartTime || earliestStartTime == -1) { earliestStartTime = processorStartTime; processor = i; }
            }

            if (earliestStartTime != -1) {
                schedule.AddTask(processor, task, earliestStartTime);
            }
        }

        return schedule;
    }
}