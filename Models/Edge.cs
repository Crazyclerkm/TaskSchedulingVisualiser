using System.Collections.Generic;

public class Edge
{

    public int Weight { get; set; }
    public TaskNode To { get; set; }

    public TaskNode From { get; set; }

    public Dictionary<string, string> Attributes { get; set; } = [];

    public Edge(TaskNode from, TaskNode to)
    {
        From = from;
        To = to;
    }
    
}