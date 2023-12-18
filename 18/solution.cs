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

    static readonly Dictionary<char, char> colors = new Dictionary<char, char>()
    {
        { '3', 'U'},
        { '1', 'D'},
        { '2', 'L'},
        { '0', 'R'}
    };

    class Excavation
    {
        Vec2 digger;
        List<Vec2> dug;

        public Excavation()
        {
            digger = new Vec2(0, 0);
            dug = new List<Vec2>();
            dug.Add(digger);
        }

        public void Commands(string[] commands, bool fromColor = false)
        {
            foreach (string command in commands)
                Command(command, fromColor);
        }

        public void Command(string command, bool fromColor = false)
        {
            Vec2 direction;
            long distance;
            if (fromColor)
                ParseColor(command, out direction, out distance);
            else
                Parse(command, out direction, out distance);
            Dig(direction, distance);
        }

        void Parse(string command, out Vec2 direction, out long distance)
        {
            string[] parts = command.Split(' ');
            direction = directions[parts[0][0]];
            distance = long.Parse(parts[1]);
        }

        void ParseColor(string command, out Vec2 direction, out long distance)
        {
            string[] parts = command.Split(' ');
            string part = parts[2];
            string color = part.Substring(2, part.Length - 3);
            distance = Convert.ToInt64(color.Substring(0, color.Length - 1), 16);
            direction = directions[colors[color[color.Length - 1]]];
        }

        void Dig(Vec2 direction, long distance)
        {
            digger = digger + direction * distance;
            dug.Add(digger);
        }

        public BigInteger DigOutInterior()
        {
            BigInteger result = 0;
            for (int i = 0; i < dug.Count - 1; i++)
            {
                Vec2 a = dug[i];
                Vec2 b = dug[i + 1];
                result += a.X * b.Y - a.Y * b.X;
                result += Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
            }
            result += 2;
            return result / 2;
        }
    } 

    public static void Main()
    {
        string filePath = "input";

        string[] lines = File.ReadAllLines(filePath);

        Excavation excavation = new Excavation();
        excavation.Commands(lines);
        BigInteger result = excavation.DigOutInterior();
        Console.WriteLine($"Solution to the first part: {result}");


        excavation = new Excavation();
        excavation.Commands(lines, true);
        result = excavation.DigOutInterior();
        Console.WriteLine($"Solution to the second part: {result}");
    }
}

