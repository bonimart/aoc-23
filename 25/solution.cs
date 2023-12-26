using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    struct Node
    {
        public string Name;
        public override bool Equals(object obj)
        {
            if (obj is Node)
            {
                return Name == ((Node)obj).Name;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public static bool operator ==(Node a, Node b)
        {
            return a.Name == b.Name;
        }
        public static bool operator !=(Node a, Node b)
        {
            return a.Name != b.Name;
        }
    }
    struct Edge
    {
        public Node From;
        public Node To;
    }
    class Graph
    {
        public List<Node> Nodes;
        public Dictionary<Node, List<Node>> Edges;
        public Graph()
        {
            Nodes = new List<Node>();
            Edges = new Dictionary<Node, List<Node>>();
        }
        public Graph(Graph g)
        {
            Nodes = new List<Node>();
            Nodes.AddRange(g.Nodes);
            Edges = new Dictionary<Node, List<Node>>();
            foreach(var kvp in g.Edges)
            {
                Edges[kvp.Key] = new List<Node>();
                Edges[kvp.Key].AddRange(kvp.Value);
            }
        }
        public Graph Remove(Edge e)
        {
            var g = new Graph(this);
            g.Edges[e.From].Remove(e.To);
            g.Edges[e.To].Remove(e.From);
            return g;
        }
        public Graph Karger()
        {
            var g = new Graph(this);
            var rnd = new Random();
            Dictionary<Node, int> merged = new Dictionary<Node, int>();
            foreach (var node in g.Nodes)
            {
                merged[node] = 1;
            }
            while (g.Nodes.Count > 2)
            {
                var from_idx = rnd.Next(g.Nodes.Count);
                var from = g.Nodes[from_idx];
                var to_idx = rnd.Next(g.Edges[from].Count);
                var to = g.Edges[from][to_idx];
                merged[from] += merged[to];
                foreach (var n in g.Edges[to])
                {
                    if (n == from)
                    {
                        continue;
                    }
                    for (int i = 0; i < g.Edges[n].Count; i++)
                    {
                        if (g.Edges[n][i] == to)
                        {
                            g.Edges[n][i] = from;
                            g.Edges[from].Add(n);
                        }
                    }
                }
                g.Nodes.Remove(to);
                g.Edges.Remove(to);
                g.Edges[from].RemoveAll(n => n == to);
            }
            int count = g.Nodes.Count;
            for (int i = 0; i < count; i++)
            {
                var node = g.Nodes[i];
                for (int j = 1; j < merged[node]; j++)
                {
                    g.Nodes.Add(new Node { Name = node.Name });
                }
            }
            return g;
        }
        public void Print()
        {
            foreach(var node in Nodes)
            {
                Console.Write($"{node.Name}: ");
                if (!Edges.ContainsKey(node))
                {
                    Console.WriteLine();
                    continue;
                }
                foreach(var to in Edges[node])
                {
                    Console.Write($"{to.Name} ");
                }
                Console.WriteLine();
            }
        }
    }

    static Graph Parse(string[] lines)
    {
        Graph g = new Graph();
        foreach(string line in lines)
        {
            var parts = line.Split(": ");
            var from = parts[0];
            var to = parts[1].Split(' ');
            var node = new Node { Name = from };
            if (!g.Edges.ContainsKey(node))
            {
                g.Nodes.Add(node);
                g.Edges[node] = new List<Node>();
            }
            foreach(var n in to)
            {
                var toNode = new Node { Name = n };
                if (!g.Edges.ContainsKey(toNode))
                {
                    g.Nodes.Add(toNode);
                    g.Edges[toNode] = new List<Node>();
                }
                g.Edges[node].Add(toNode);
                g.Edges[toNode].Add(node);
            }
        }
        return g;
    }

    static int Run(string filename)
    {
        var lines = File.ReadAllLines(filename);
        var g = Parse(lines);
        var min = int.MaxValue;
        var min_g = new Graph();
        while(min != 3)
        {
            var g2 = g.Karger();
            var count = g2.Edges[g2.Nodes[0]].Count;
            if (count < min)
            {
                min = count;
                min_g = g2;
            }
        }

        int result = 1;
        var groups = min_g.Nodes.GroupBy(n => n.Name);
        foreach(var group in groups)
        {
            result *= group.Count();
        }

        return result;
    }

    public static void Main()
    {
        BigInteger result;

        var P1 = 54;
        result = Run("test");
        if (result != P1)
        {
            throw new Exception($"Part 1, test failed, expected {P1}, got {result}");
        }

        result = Run("input");
        Console.WriteLine($"Part 1: {result}");
    }
}

