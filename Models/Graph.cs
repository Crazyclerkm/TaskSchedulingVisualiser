using System.Collections.Generic;

public class Graph
{
    public string Name;

    public bool isStrict;

    public Dictionary<string, TaskNode> Nodes { get; set; } = [];
    public List<Edge> Edges { get; set; } = [];

    public Dictionary<string, string> Attributes { get; set; } = [];
    //private Dictionary<String, > Edges = [];

    public Graph()
    {
        Name = "";
    }

    public Graph(string? name)
    {
        Name = name ?? "";
    }
    
}