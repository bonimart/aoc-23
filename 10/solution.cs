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

        public Point(Point p)
        {
            this.x = p.x;
            this.y = p.y;
            this.type = p.type;
        }

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

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }
        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }
        public override bool Equals(object obj)
        {
            if (obj is Point)
                return this == (Point)obj;
            return false;
        }
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public void ResolveType(Point n1, Point n2)
        {
            if (n1.x == n2.x)
            {
                type = Type.NS;
            }
            else if (n1.y == n2.y)
            {
                type = Type.EW;
            }
            else if (x == n1.x)
            {
                if (y < n1.y)
                {
                    if (n2.x < x)
                        type = Type.NW;
                    else
                        type = Type.NE;
                }
                else
                {
                    if (n2.x < x)
                        type = Type.SW;
                    else
                        type = Type.SE;
                }
            }
            else
            {
                if (x < n1.x)
                {
                    if (n2.y < y)
                        type = Type.NW;
                    else
                        type = Type.SW;
                }
                else
                {
                    if (n2.y < y)
                        type = Type.NE;
                    else
                        type = Type.SE;
                }
            }
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
            map = new Point[height, width];
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

    class LoopPoint
    {
        public int x;
        public int y;
        public int direction_x;
        public int direction_y;
        public LoopPoint(int x, int y, int direction_x, int direction_y)
        {
            this.x = x;
            this.y = y;
            this.direction_x = direction_x;
            this.direction_y = direction_y;
        }
    }

    class Loop
    {
        public List<Point> points;
        public Loop(List<Point> points)
        {
            this.points = points;
        }

        public bool IsInside(Point p)
        {
            if (points.Contains(p))
                return false;
            int countLeft = points.Count(l => l.x < p.x && l.y == p.y && (l.type == Type.NS || l.type == Type.NE || l.type == Type.NW));
            return countLeft % 2 == 1;
        }

    }

    public static void Main()
    {
        string filePath = "input";

        string[] lines = File.ReadAllLines(filePath);

        int solution = 0;

        Map map = new Map(lines);
        HashSet<Point> visited = new HashSet<Point>();
        Dictionary<Point, Point> previous = new Dictionary<Point, Point>();
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
                previous[p] = neighbours[0];
                // reverse previous
                Point other_side = neighbours[1];
                Point tmp;
                while (p != map.start)
                {
                    tmp = new Point(previous[other_side]);
                    previous[other_side] = p;
                    p = other_side;
                    other_side = tmp;
                }
                map.start.ResolveType(previous[map.start], other_side);
                break;
            }
            foreach (Point neighbour in neighbours)
            {
                queue.Enqueue(new State(neighbour, distance + 1));
                if (!previous.ContainsKey(neighbour))
                    previous[neighbour] = p;
            }

        }

        Console.WriteLine($"Solution to the first part: {solution}");

        Loop loop = new Loop(GetLoop(map.start, previous));

        int area = 0;
        int size = 0;
        foreach (Point p in map.map)
        {
            size++;
            if (loop.IsInside(p))
                area++;
        }
        Console.WriteLine($"Size: {size}");

        Console.WriteLine($"Solution to the second part: {area}");
    }

    static List<Point> GetLoop(Point start, Dictionary<Point, Point> previous)
    {
        List<Point> loop = new List<Point>();
        Point p = start;
        Point previous_p = previous[p];
        while (true)
        {
            loop.Add(new Point(p));
            p = previous[p];
            if (p == start)
                break;
        }
        return loop;
    }
}
