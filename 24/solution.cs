using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    struct Trajectory2
    {
        public Vector3 Origin;
        public Vector3 Direction;

        public Trajectory2(Vector3 Origin, Vector3 Direction)
        {
            this.Origin = Origin;
            this.Direction = Direction;
        }

        public Vector3 PointAt(float t)
        {
            return Origin + Direction * t;
        }

        public float Intersection(Trajectory2 other)
        {
            float det = Direction.X * other.Direction.Y - Direction.Y * other.Direction.X;
            if (det == 0)
            {
                return -1;
            }
            float dx = other.Origin.X - Origin.X;
            float dy = other.Origin.Y - Origin.Y;
            float t = (dx * other.Direction.Y - dy * other.Direction.X) / det;
            if (t < 0)
            {
                return -1;
            }
            return t;
        }

        public override string ToString()
        {
            return $"Origin: {Origin}, Direction: {Direction}";
        }
    }

    static Trajectory2 Parse(string line)
    {
       string[] parts = line.Split('@'); 
       var origin = ParseVector3(parts[0]);
       var direction = ParseVector3(parts[1]);
       return new Trajectory2(origin, direction);
    }

    static Vector3 ParseVector3(string line)
    {
        string[] parts = line.Split(',');
        return new Vector3(long.Parse(parts[0]), long.Parse(parts[1]), 0);
    }

    static int Run(string filename, long min, long max)
    {
        List<Trajectory2> trajectories = new List<Trajectory2>();
        foreach (string line in File.ReadLines(filename))
        {
            trajectories.Add(Parse(line));
        }
        var intersections = 0;
        for (int i = 0; i < trajectories.Count; i++)
        {
            for (int j = i + 1; j < trajectories.Count; j++)
            {
                float t1 = trajectories[i].Intersection(trajectories[j]);
                float t2 = trajectories[j].Intersection(trajectories[i]);
                if (t1 < 0 || t2 < 0)
                {
                    continue;
                }
                var p = trajectories[i].PointAt(t1);
                if (p.X >= min && p.X <= max && p.Y >= min && p.Y <= max)
                {
                    intersections++;
                }
            }
        }
        return intersections;
    }

    public static void Main()
    {
        int result;

        var p1 = 2;
        result = Run("test", 7, 27);
        if (result != p1)
        {
            throw new Exception($"Part 1, test failed, expected {p1}, got {result}");
        }

        // part 1
        result = Run("input", 200000000000000, 400000000000000);
        Console.WriteLine($"Part 1: {result}");
    }
}

