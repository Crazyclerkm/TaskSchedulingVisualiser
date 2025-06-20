using System.Collections.Generic;

public class TaskNode
{
    public string Name;

    public List<Edge> Predecessors { get; } = [];
    public List<Edge> Successors { get; } = [];

    public Dictionary<string, string> Attributes { get; set; } = [];

    public int Weight
    {
        get => int.TryParse(Attributes.GetValueOrDefault("Weight"), out var result) ? result : 0;
        set => Attributes["Weight"] = value.ToString();
    }

    public TaskNode(string name)
    {
        Name = name;
    }
    
}