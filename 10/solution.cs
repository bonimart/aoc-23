using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

class Program
{
    enum Type
    {
        NS,
        NE,
        NW,
        SE,
        SW,
        EW,
        SS,
        GG,
    }

    struct Point
    {
        public int x;
        public int y;
        public Type type;

        public Point(int y, int x, char type)
        {
            switch(type)
            {
                case '|':
                    this.type = Type.NS;
                    break;
                case '-':
                    this.type = Type.EW;
                    break;
                case 'S':
                    this.type = Type.SS;
                    break;
                case '.':
                    this.type = Type.GG;
                    break;
                case 'L':
                    this.type = Type.NE;
                    break;
                case 'F':
                    this.type = Type.SE;
                    break;
                case '7':
                    this.type = Type.SW;
                    break;
                case 'J':
                    this.type = Type.NW;
                    break;
                default:
                    throw new Exception("Invalid type");
            }
            this.x = x;
            this.y = y;
        }
        public bool North(){
            if (type == Type.NS || type == Type.NE || type == Type.NW || type == Type.SS)
                return true;
            return false;
        }
        public bool South(){
            if (type == Type.NS || type == Type.SE || type == Type.SW || type == Type.SS)
                return true;
            return false;
        }
        public bool East(){
            if (type == Type.EW || type == Type.NE || type == Type.SE || type == Type.SS)
                return true;
            return false;
        }
        public bool West(){
            if (type == Type.EW || type == Type.NW || type == Type.SW || type == Type.SS)
                return true;
            return false;
        }
    }

    class Map
    {
        public Point[,] map;
        public int width;
        public int height;
        public Point start;

        public Map(string[] lines)
        {
            width = lines[0].Length;
            height = lines.Length;
            map = new Point[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    map[i, j] = new Point(i, j, lines[i][j]);
                    if (lines[i][j] == 'S')
                        start = map[i, j];
                }
            }
        }

        public List<Point> GetNeighbours(Point p)
        {
            List<Point> neighbours = new List<Point>();
            Point other = new Point();
            if (p.North() && p.y > 0)
            {
                other = map[p.y - 1, p.x];
                if (other.South())
                    neighbours.Add(other);
            }
            if (p.South() && p.y < height - 1)
            {
                other = map[p.y + 1, p.x];
                if (other.North())
                    neighbours.Add(other);
            }
            if (p.East() && p.x < width - 1)
            {
                other = map[p.y, p.x + 1];
                if (other.West())
                    neighbours.Add(other);
            }
            if (p.West() && p.x > 0)
            {
                other = map[p.y, p.x - 1];
                if (other.East())
                    neighbours.Add(other);     
            }
            return neighbours;
        }

        public Point this[int y, int x]
        {
            get
            {
                return map[y, x];
            }
        }
    }

    struct State
    {
        public Point p;
        public int distance;
        public State(Point p, int distance)
        {
            this.p = p;
            this.distance = distance;
        }
    }

    public static void Main()
    {
        string filePath = "input";

        // Read all lines from the file BigIntegero an array
        string[] lines = File.ReadAllLines(filePath);

        int solution = 0;

        Map map = new Map(lines);
        HashSet<Point> visited = new HashSet<Point>();
        Queue<State> queue = new Queue<State>();
        queue.Enqueue(new State(map.start, 0));
        while (queue.Count > 0)
        {
            State state = queue.Dequeue();
            Point p = state.p;
            int distance = state.distance;
            if (visited.Contains(p))
                continue;
            visited.Add(p);
            List<Point> neighbours = map.GetNeighbours(p);
            if (neighbours.Count == 2 && visited.Contains(neighbours[0]) && visited.Contains(neighbours[1]))
            {
                solution = distance;
                break;
            }
            foreach (Point neighbour in neighbours)
            {
                queue.Enqueue(new State(neighbour, distance + 1));
            }

        }

        Console.WriteLine($"Solution to the first part: {solution}");

    }
}
