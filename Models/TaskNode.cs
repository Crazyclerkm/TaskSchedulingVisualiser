using System.Collections.Generic;

public class TaskNode
{
    public string Name;
    public int Weight { get; set; }

    public Dictionary<string, string> Attributes { get; set; } = [];

    public TaskNode(string name)
    {
        Name = name;
    }
    
}