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
        public int X;
        public int Y;

        public Vec2(int X, int Y)
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
    }

    static readonly Dictionary<char, Vec2> directions = new Dictionary<char, Vec2>()
    {
        { 'U', new Vec2(0, -1) },
        { 'D', new Vec2(0, 1) },
        { 'L', new Vec2(-1, 0) },
        { 'R', new Vec2(1, 0) }
    };

    class Excavation
    {
        Vec2 min, max;
        Vec2 digger;
        HashSet<Vec2> dug;

        public Excavation()
        {
            min = new Vec2(0, 0);
            max = new Vec2(0, 0);
            digger = new Vec2(0, 0);
            dug = new HashSet<Vec2>();
            dug.Add(digger);
        }

        public void Commands(string[] commands)
        {
            foreach (string command in commands)
                Command(command);
        }

        public void Command(string command)
        {
            Vec2 direction;
            int distance;
            Parse(command, out direction, out distance);
            Dig(direction, distance);
        }

        void Parse(string command, out Vec2 direction, out int distance)
        {
            string[] parts = command.Split(' ');
            direction = directions[parts[0][0]];
            distance = int.Parse(parts[1]);
        }

        void Dig(Vec2 direction, int distance)
        {
            for (int i = 0; i < distance; i++)
            {
                digger += direction;
                min = new Vec2( 
                        Math.Min(min.X, digger.X),
                        Math.Min(min.Y, digger.Y));
                max = new Vec2(
                    Math.Max(max.X, digger.X),
                    Math.Max(max.Y, digger.Y));
                dug.Add(digger);
            }
        }

        public int DigOutInterior()
        {
            HashSet<Vec2> left = new HashSet<Vec2>();
            for (int y = min.Y; y <= max.Y; ++y)
            {
                DigOutBlock(new Vec2(min.X, y), left);
                DigOutBlock(new Vec2(max.X, y), left);
            }
            for (int x = min.X; x <= max.X; ++x)
            {
                DigOutBlock(new Vec2(x, min.Y), left);
                DigOutBlock(new Vec2(x, max.Y), left);
            }
            return (Math.Abs(max.X - min.X) + 1) * (Math.Abs(max.Y - min.Y) + 1) - left.Count;
        }

        void DigOutBlock(Vec2 position, HashSet<Vec2> seen)
        {
            Queue<Vec2> queue = new Queue<Vec2>();
            queue.Enqueue(position);
            while (queue.Count > 0)
            {
                Vec2 current = queue.Dequeue();
                if (seen.Contains(current) || dug.Contains(current))
                    continue;
                seen.Add(current);
                foreach (Vec2 direction in directions.Values)
                {
                    Vec2 next = current + direction;
                    if (InBorders(next))
                        queue.Enqueue(next);
                }
            }

        }

        bool InBorders(Vec2 position)
        {
            return position.X >= min.X && position.X <= max.X
                && position.Y >= min.Y && position.Y <= max.Y;
        }

        public void Print()
        {
            Console.WriteLine($"min: {min.X}, {min.Y}");
            Console.WriteLine($"max: {max.X}, {max.Y}");
            for (int y = min.Y; y <= max.Y; ++y)
            {
                for (int x = min.X; x <= max.X; ++x)
                {
                    char c = dug.Contains(new Vec2(x, y)) ? '#' : '.';
                    Console.Write(c);
                }
                Console.WriteLine();
            }
        }
    }

    public static void Main()
    {
        string filePath = "input";

        string[] lines = File.ReadAllLines(filePath);

        Excavation excavation = new Excavation();
        excavation.Commands(lines);
        int result = excavation.DigOutInterior();
        Console.WriteLine($"Solution to the first part: {result}");

    }
}

