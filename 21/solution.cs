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

        public Garden(Vec2 Elf, HashSet<Vec2> Plots)
        {
            this.Elf = Elf;
            this.Plots = Plots;
            Width = Plots.Max(p => p.X) + 1;
            Height = Plots.Max(p => p.Y) + 1;
        }

        public BigInteger Reachable(int steps)
        {
            HashSet<Vec2> visited = new HashSet<Vec2>();
            HashSet<Vec2> reachable = new HashSet<Vec2>();
            Queue<State> toVisit = new Queue<State>();
            toVisit.Enqueue(new State(Elf, 0));

            while (toVisit.Count > 0)
            {
                State state = toVisit.Dequeue();
                if (visited.Contains(state.pos) || state.steps > steps)
                {
                    continue;
                }
                visited.Add(state.pos);
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

        public List<Vec2> Neighbours(Vec2 pos)
        {
            List<Vec2> neighbours = new List<Vec2>();
            foreach (Vec2 dir in directions.Values)
            {
                Vec2 neighbour = pos + dir;
                if (Plots.Contains(neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }
            return neighbours;
        }
    }

    static void Run(string filename, int steps, out BigInteger firstPart, out BigInteger secondPart)
    {
        string[] lines = File.ReadAllLines(filename);
        Garden garden = ParseGarden(lines);
        firstPart = garden.Reachable(steps);
        secondPart = 0;
    }

    const char ROCK = '#';
    const char START = 'S';
    const char PLOT = '.';
    static Garden ParseGarden(string[] lines)
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
                        break;
                    case PLOT:
                        plots.Add(new Vec2(x, y));
                        break;
                    default:
                        throw new Exception($"Unknown character {lines[y][x]}");
                }
            }
        }
        return new Garden(start, plots);
    }

    public static void Main()
    {
        BigInteger firstPart, secondPart;

        var P1 = 16;
        Run("test", 6, out firstPart, out secondPart);
        if (firstPart != P1)
        {
            throw new Exception($"Part 1, test failed, expected {P1}, got {firstPart}");
        }

        Run("input", 64, out firstPart, out secondPart);

        Console.WriteLine($"Part 1: {firstPart}");
    }
}

