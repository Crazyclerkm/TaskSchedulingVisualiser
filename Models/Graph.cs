using System.Collections.Generic;

public class Graph
{
    public string Name;

    public bool isStrict;

    public Dictionary<string, TaskNode> Nodes { get; set; } = [];
    public Dictionary<(TaskNode, TaskNode), Edge> Edges { get; set; } = [];
    public Dictionary<string, string> Attributes { get; set; } = [];

    public Graph()
    {
        Name = "";
    }

    public Graph(string? name)
    {
        Name = name ?? "";
    }

    public List<TaskNode> TopologicalOrdering()
    {
        List<TaskNode> order = [];

        Dictionary<TaskNode, int> inDegree = [];
        Queue<TaskNode> available = [];

        foreach (TaskNode node in Nodes.Values) {
            if (node.Predecessors.Count == 0)
            {
                available.Enqueue(node);
            }
            else
            {
                inDegree[node] = node.Predecessors.Count;
            }
        }

        while (available.Count != 0)
        {
            TaskNode node = available.Dequeue();
            order.Add(node);

            foreach (Edge dependentEdge in node.Successors)
            {
                TaskNode dependent = dependentEdge.To;
                int count = inDegree[dependent] - 1;
                if (count == 0)
                {
                    available.Enqueue(dependent);
                    inDegree.Remove(dependent);
                }
                else
                {
                    inDegree[dependent] = count;
                }
            }
        }

        return order;
    }
    
}