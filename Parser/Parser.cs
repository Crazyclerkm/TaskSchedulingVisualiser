using System;
using System.Collections.Generic;
using System.Linq;

class ParsedNode(string id, Dictionary<string, string> attributes)
{
    public string Id = id;
    public Dictionary<string, string> Attributes = attributes;
}

class ParsedEdge(string from, string to, Dictionary<string, string> attributes)
{
    public string From = from;
    public string To = to;
    public Dictionary<string, string> Attributes = attributes;
}

public class Parser
{
    private readonly Tokeniser DotTokeniser;
    private Token Current;

    public Parser(string input)
    {
        DotTokeniser = new Tokeniser(input);
        Current = DotTokeniser.NextToken();
    }

    private void Consume()
    {
        Current = DotTokeniser.NextToken();
    }

    private void Expect(TokenType type, string? value = null)
    {
        bool valueIsNull = (value == null);
        if (Current.Type != type || (!valueIsNull && Current.Value != value))
        {
            throw new Exception(
                $@"Expected token of type {type} {(valueIsNull ? "" : $" with value {value}")},
                 got {Current.Type} {(valueIsNull ? "" : $" with value {Current.Value}")} at {Current.Line} : {Current.Column}"
            );
        }
        Consume();
    }

    private bool Match(TokenType type, string? value = null)
    {
        return Current.Type == type && (value == null || Current.Value == value);
    }

    public Graph Parse()
    {
        bool isStrict = false;
        if (Current.Type == TokenType.Keyword && Current.Value == "strict")
        {
            isStrict = true;
            Consume();
        }

        if (Current.Type != TokenType.Keyword || Current.Value != "digraph")
        {
            throw new Exception("Task graph must be a digraph");
        }

        Consume();

        string? graphName = null;

        if (Current.Type == TokenType.Identifier || Current.Type == TokenType.String)
        {
            graphName = Current.Value;
            Consume();
        }

        Expect(TokenType.LeftBrace);

        Dictionary<string, ParsedNode> nodeMap = [];
        // TODO: Change to a dictionary for faster processing of larger graphs
        List<ParsedEdge> edgeList = [];
        Dictionary<string, string> graphAttributes = [];

        while (!Match(TokenType.RightBrace)) ParseStatement(nodeMap, edgeList, graphAttributes);

        Expect(TokenType.RightBrace);

        Graph graph = new(graphName);
        graph.isStrict = isStrict;

        foreach (ParsedNode node in nodeMap.Values)
        {
            TaskNode taskNode = new(node.Id);
            taskNode.Attributes = node.Attributes;

            graph.Nodes[taskNode.Name] = taskNode;
        }

        foreach (ParsedEdge edge in edgeList)
        {
            if (!graph.Nodes.TryGetValue(edge.From, out var fromNode) || !graph.Nodes.TryGetValue(edge.To, out var toNode))
            {
                throw new Exception($"Edge from {edge.From} to {edge.To} refers to invalid node(s).");
            }

            Edge graphEdge = new(fromNode, toNode);
            graphEdge.Attributes = edge.Attributes;

            if (graph.isStrict)
            {
                var existingEdge = graph.Edges.FirstOrDefault(e => e.From.Name == edge.From && e.To.Name == edge.To);
                if (existingEdge != null)
                {
                    foreach (var keyValue in edge.Attributes) existingEdge.Attributes[keyValue.Key] = keyValue.Value;
                }
                else
                {
                    graph.Edges.Add(graphEdge);
                }
            }
            else
            {
                graph.Edges.Add(graphEdge);
            }
        }

        graph.Attributes = graphAttributes;
        
        return graph;
    }

    private void ParseStatement( Dictionary<string, ParsedNode> nodeMap, List<ParsedEdge> edgeList, Dictionary<string, string> graphAttributes)
    {
        if (Current.Type == TokenType.Identifier || Current.Type == TokenType.String)
        {
            string id = Current.Value;
            Consume();

            if (Match(TokenType.Arrow))
            {
                Consume();
                if (Current.Type != TokenType.Identifier && Current.Type != TokenType.String)
                {
                    throw new Exception($"Expected node id after '->' at {Current.Line} : {Current.Column}");
                }

                string id2 = Current.Value;
                Consume();

                Dictionary<string, string> attributes = [];

                if (Match(TokenType.LeftBracket)) attributes = ParseAttributeList();
                if (Match(TokenType.Semicolon)) Consume();
                ParsedEdge edge = new(id, id2, attributes);

                edgeList.Add(edge);
            }
            else
            {
                Dictionary<string, string> attributes = [];
               

                if (Match(TokenType.LeftBracket)) attributes = ParseAttributeList();
                if (Match(TokenType.Semicolon)) Consume();

                ParsedNode node = nodeMap.TryGetValue(id, out ParsedNode? value) ? value : new ParsedNode(id, attributes);

                nodeMap[id] = node;
            }
        }
        else if (Current.Type == TokenType.Semicolon)
        {
            Consume();
        }
        else if (Match(TokenType.Keyword, "graph"))
        {
            Consume();
            Dictionary<string, string> attributes = ParseAttributeList();

            foreach (var keyValue in attributes) graphAttributes[keyValue.Key] = keyValue.Value;

            if (Match(TokenType.Semicolon)) Consume();
        }
        else
        {
            throw new Exception($"Unexpected token {Current.Value} at {Current.Line} : {Current.Column}");
        }
    }

    private Dictionary<string, string> ParseAttributeList()
    {
        Expect(TokenType.LeftBracket);

        Dictionary<string, string> attributes = [];

        while (!Match(TokenType.RightBracket))
        {
            if (Current.Type != TokenType.Identifier && Current.Type != TokenType.String)
            {
                throw new Exception($"Expected attribute key at {Current.Line} : {Current.Column}, got {Current.Value}");
            }

            string key = Current.Value;
            Consume();

            Expect(TokenType.Equals);

            if (Current.Type != TokenType.Identifier && Current.Type != TokenType.String)
            {
                throw new Exception($"Expected attribute key at {Current.Line} : {Current.Column}, got {Current.Value}");
            }

            string value = Current.Value;
            Consume();

            attributes[key] = value;

            if (Match(TokenType.Comma) || Match(TokenType.Semicolon)) Consume();
        }

        Expect(TokenType.RightBracket);
        return attributes;
    }
}