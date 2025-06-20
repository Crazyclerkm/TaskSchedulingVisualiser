using System.Collections.Generic;

public class Edge
{

    public TaskNode To { get; set; }

    public TaskNode From { get; set; }

    public Dictionary<string, string> Attributes { get; set; } = [];

    public int Weight
    {
        get => int.TryParse(Attributes.GetValueOrDefault("Weight"), out var result) ? result : 0;
        set => Attributes["Weight"] = value.ToString();
    }

    public Edge(TaskNode from, TaskNode to)
    {
        From = from;
        To = to;
    }
    
}