#r "System.Runtime.Numerics.dll"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    public struct Node
    {
        public string Name { get; }
        public string Left { get; }
        public string Right { get; }

        public Node(string name, string left, string right)
        {
            Name = name;
            Left = left;
            Right = right;
        }
    }
    public static void Main()
    {
        string filePath = "input";

        // Read all lines from the file into an array
        string[] lines = File.ReadAllLines(filePath);

        Dictionary<string, Node> nodes = new Dictionary<string, Node>();

        string directions = "LRRRLRRRLRRLRLRRLRLRRLRRLRLLRRRLRLRLRRRLRRRLRLRLRLLRRLLRRLRRRLLRLRRRLRLRLRRRLLRLRRLRRRLRLRRRLLRLRRLRRRLRRLRRLRLRRLRRRLRLRRRLRRLLRRLRRLRLRRRLRRLRRRLRRRLRLRRLRLRRRLRLRRLRRLRRRLRRRLRRRLLRRLRRRLRLRLRLRRRLRLRLRRLRRRLRRRLRRLRRLLRLRRLLRLRRLRRLLRLLRRRLLRRLLRRLRRLRLRLRRRLLRRLRRRR";

        foreach (string line in lines)
        {
            string pattern = @"(\w+) = \((\w+), (\w+)\)";
            Match match = Regex.Match(line, pattern);
            Node node = new Node(
                    match.Groups[1].Value,
                    match.Groups[2].Value,
                    match.Groups[3].Value
                    );
            nodes[node.Name] = node;
        }

        int count = 0;
        string begin = "AAA";
        string end = "ZZZ";
        string current = begin;
        while (current != end)
        {
            foreach (char direction in directions)
            {
                if (current == end)
                {
                    break;
                }
                if (direction == 'L')
                {
                    current = nodes[current].Left;
                }
                else
                {
                    current = nodes[current].Right;
                }
                count++;
            }
        }

        Console.WriteLine($"Solution to the first part is {count}");
        
        List<string> starting_nodes = new List<string>();
        List<string> ending_nodes = new List<string>();
        foreach (KeyValuePair<string, Node> node in nodes)
        {
            string name = node.Key;
            if (name.EndsWith("A"))
            {
                starting_nodes.Add(name);
            }
            else if (name.EndsWith("Z"))
            {
                ending_nodes.Add(name);
            }
        }

        Dictionary<string, List<BigInteger>> periods = new Dictionary<string, List<BigInteger>>();
        foreach (string starting_node in starting_nodes)
        {
            foreach (string ending_node in ending_nodes)
            {
                ulong period = (ulong)FindPath(starting_node, ending_node, nodes, directions);
                if (!periods.ContainsKey(starting_node))
                {
                    periods[starting_node] = new List<BigInteger>();
                }
                periods[starting_node].Add(new BigInteger(period));
            }
        }

        List<List<BigInteger>> periods_list = periods.Values.ToList();
        BigInteger lowest_lcm = LowestLCM(periods_list);

        Console.WriteLine($"Solution to the second part is {lowest_lcm}");
    }

    public static BigInteger GCD(BigInteger a, BigInteger b)
    {
        while (b != 0)
        {
            BigInteger temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static BigInteger LCM(BigInteger a, BigInteger b)
    {
        BigInteger result = (a / GCD(a, b)) * b;
        return result;
    }

    public static BigInteger LowestLCM(List<List<BigInteger>> periods)
    {
        List<BigInteger> lcm = new List<BigInteger>();
        foreach (List<BigInteger> period in periods)
        {
            if (lcm.Count == 0)
            {
                lcm.AddRange(period);
                continue;
            }
            List<BigInteger> new_lcm = new List<BigInteger>();
            for (int i = 0; i < period.Count; i++)
            {
                for (int j = 0; j < lcm.Count; j++)
                {
                    new_lcm.Add(LCM(period[i], lcm[j]));
                }
            }
            lcm = new_lcm;
        }
        return lcm.Min();
    }

    public static int FindPath(string start, List<string> ends, Dictionary<string, Node> nodes, string directions)
    {
        int count = 0;
        string current = start;
        while (!ends.Contains(current))
        {
            for (int i = 0; i < directions.Length; i++)
            {
                char direction = directions[i];
                if (ends.Contains(current))
                {
                    break;
                }
                if (direction == 'L')
                {
                    current = nodes[current].Left;
                }
                else
                {
                    current = nodes[current].Right;
                }
                count++;
            }
        }
        return count;
    }

    public static ulong FindPath(string start, string end, Dictionary<string, Node> nodes, string directions)
    {
        ulong count = 0;
        string current = start;
        HashSet<Tuple<string, int>> visited = new HashSet<Tuple<string, int>>();
        while (current != end)
        {
            for (int i = 0; i < directions.Length; i++)
            {
                Tuple<string, int> state = new Tuple<string, int>(current, i);
                if (visited.Contains(state))
                {
                    return ulong.MaxValue;
                }
                visited.Add(state);
                char direction = directions[i];
                if (current == end)
                {
                    break;
                }
                if (direction == 'L')
                {
                    current = nodes[current].Left;
                }
                else
                {
                    current = nodes[current].Right;
                }
                count++;
            }
        }
        return count;
    }
}

Program.Main();
