using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    struct Vec2
    {
        public long X;
        public long Y;

        public Vec2(long X, long Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Vec2(Vec2 vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2 operator *(Vec2 a, long b)
        {
            return new Vec2(a.X * b, a.Y * b);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec2)
            {
                Vec2 other = (Vec2)obj;
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Vec2 a, Vec2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vec2 a, Vec2 b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    static long mod(long x, long m) {
        long r = x%m;
        return r<0 ? r+m : r;
    }


    static readonly Dictionary<char, Vec2> directions = new Dictionary<char, Vec2>()
    {
        { 'U', new Vec2(0, -1) },
        { 'D', new Vec2(0, 1) },
        { 'L', new Vec2(-1, 0) },
        { 'R', new Vec2(1, 0) }
    };

    class Garden
    {
        struct State
        {
            public Vec2 pos;
            public int steps;
            public State(Vec2 pos, int steps)
            {
                this.pos = pos;
                this.steps = steps;
            }
        }

        Vec2 Elf;
        HashSet<Vec2> Plots;
        long Width, Height;
        bool Infinite;

        public Garden(Vec2 Elf, HashSet<Vec2> Plots, bool infinite = false)
        {
            this.Elf = Elf;
            this.Plots = Plots;
            Width = Plots.Max(p => p.X) + 1;
            Height = Plots.Max(p => p.Y) + 1;
            Infinite = infinite;
        }

        public BigInteger Reachable(int steps)
        {
            HashSet<Vec2> reachable = new HashSet<Vec2>();
            Queue<State> toVisit = new Queue<State>();
            toVisit.Enqueue(new State(Elf, 0));

            while (toVisit.Count > 0)
            {
                State state = toVisit.Dequeue();
                if (state.steps > steps)
                {
                    continue;
                }
                if (reachable.Contains(state.pos))
                {
                    continue;
                }
                if (state.steps % 2 == steps % 2)
                {
                    reachable.Add(state.pos);
                }
                foreach (Vec2 neighbour in Neighbours(state.pos))
                {
                    toVisit.Enqueue(new State(neighbour, state.steps + 1));
                }
            }
            return reachable.Count;
        }

        public List<Vec2> ShortestPath(Vec2 goal)
        {
            HashSet<Vec2> visited = new HashSet<Vec2>();
            Dictionary<Vec2, Vec2> cameFrom = new Dictionary<Vec2, Vec2>();
            Queue<Vec2> toVisit = new Queue<Vec2>();
            toVisit.Enqueue(Elf);
            while(toVisit.Count > 0)
            {
                Vec2 pos = toVisit.Dequeue();
                if (pos == goal)
                {
                    break;
                }
                if (visited.Contains(pos))
                {
                    continue;
                }
                visited.Add(pos);
                foreach (Vec2 neighbour in Neighbours(pos))
                {
                    if (!visited.Contains(neighbour))
                    {
                        cameFrom[neighbour] = pos;
                        toVisit.Enqueue(neighbour);
                    }
                }
            }
            List<Vec2> path = new List<Vec2>();
            Vec2 current = goal;
            while (current != Elf)
            {
                path.Add(current);
                current = cameFrom[current];
            }
            path.Reverse();
            return path;
        }

        public List<Vec2> Neighbours(Vec2 pos)
        {
            List<Vec2> neighbours = new List<Vec2>();
            foreach (Vec2 dir in directions.Values)
            {
                Vec2 neighbour = pos + dir;
                if (Infinite)
                {
                    neighbour.X = mod(neighbour.X, Width);
                    neighbour.Y = mod(neighbour.Y, Height);
                }
                if (Plots.Contains(neighbour))
                {
                    neighbours.Add(pos + dir);
                }
            }
            return neighbours;
        }

        void Print(HashSet<Vec2> visited)
        {
            for (long y = visited.Min(p => p.Y); y < visited.Max(p => p.Y) + 1; y++)
            {
                string line = "";
                for (long x = visited.Min(p => p.X); x < visited.Max(p => p.X) + 1; x++)
                {
                    Vec2 pos = new Vec2(x, y);
                    Vec2 posRelative = pos;
                    if (Infinite)
                    {
                        posRelative.X = mod(posRelative.X, Width);
                        posRelative.Y = mod(posRelative.Y, Height);
                    }
                    if (pos == Elf)
                    {
                        line += "E";
                    }
                    else if (Plots.Contains(posRelative))
                    {
                        line += visited.Contains(pos) ? "O" : ".";
                    }
                    else
                    {
                        line += "#";
                    }
                }
                Console.WriteLine(line);
            }
        }
    }

    static BigInteger Run(string filename, int steps, bool infinite = false)
    {
        string[] lines = File.ReadAllLines(filename);
        Garden garden = ParseGarden(lines, infinite);
        return garden.Reachable(steps);
    }

    const char ROCK = '#';
    const char START = 'S';
    const char PLOT = '.';
    static Garden ParseGarden(string[] lines, bool infinite = false)
    {
        Vec2 start = new Vec2();
        HashSet<Vec2> plots = new HashSet<Vec2>();
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                switch (lines[y][x])
                {
                    case ROCK:
                        break;
                    case START:
                        start = new Vec2(x, y);
                        plots.Add(start);
                        break;
                    case PLOT:
                        plots.Add(new Vec2(x, y));
                        break;
                    default:
                        throw new Exception($"Unknown character {lines[y][x]}");
                }
            }
        }
        return new Garden(start, plots, infinite);
    }

    public static List<BigInteger> SimplifiedLagrange(BigInteger x0, BigInteger x1, BigInteger x2)
    {
        BigInteger a = x0 / 2 - x1 + x2 / 2;
        BigInteger b = -3 * (x0 / 2) + 2 * x1 - x2 / 2;
        BigInteger c = x0;
        return new List<BigInteger>() { a, b, c };
    }

    public static void Main()
    {
        BigInteger result;

        var P1 = 16;
        result = Run("test", 6);
        if (result != P1)
        {
            throw new Exception($"Part 1, test failed, expected {P1}, got {result}");
        }

        // part 1
        result = Run("input", 64);
        Console.WriteLine($"Part 1: {result}");

        // part 2
        int steps = 26501365;
        int diameter = 131;
        int radius = diameter / 2;
        int target = (steps - radius) / diameter;
        BigInteger x0 = Run("input", radius, true);
        BigInteger x1 = Run("input", radius + diameter, true);
        BigInteger x2 = Run("input", radius + 2 * diameter, true);
        var lagrange = SimplifiedLagrange(x0, x1, x2);
        result = lagrange[0] * target * target + lagrange[1] * target + lagrange[2];
        Console.WriteLine($"Part 2: {result}");

    }
}

