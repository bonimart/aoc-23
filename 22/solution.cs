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

    struct Vec3
    {
        public int X;
        public int Y;
        public int Z;
        public Vec3(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        public Vec3(Vec3 vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            this.Z = vec.Z;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }

    class Brick
    {
        public Vec3 reference;
        public Vec3 size;
        public List<Brick> supporting;
        public List<Brick> supportedBy;

        public Brick(Vec3 start, Vec3 end)
        {
            Vec3 mi = new Vec3(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y), Math.Min(start.Z, end.Z));
            Vec3 ma = new Vec3(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y), Math.Max(start.Z, end.Z));
            reference = mi;
            size = new Vec3(ma.X - mi.X + 1, ma.Y - mi.Y + 1, ma.Z - mi.Z + 1);
            supporting = new List<Brick>();
            supportedBy = new List<Brick>();
        }

        public bool Supports(Brick other)
        {
            if (other.reference.Z != reference.Z + size.Z)
            {
                return false;
            }
            return (
                (other.reference.X >= reference.X
                && other.reference.X <= reference.X + size.X - 1)
                ||
                (reference.X >= other.reference.X
                && reference.X <= other.reference.X + other.size.X - 1))
                &&
                ((other.reference.Y >= reference.Y
                && other.reference.Y <= reference.Y + size.Y - 1)
                ||
                (reference.Y >= other.reference.Y
                && reference.Y <= other.reference.Y + other.size.Y - 1));
        }

        public void Fall()
        {
            reference.Z--;
        }

        public void Print()
        {
            Console.WriteLine($"Brick {reference} {size}");
        }
    }

    struct State
    {
        public int seen;
        public int neededToFall;
    }

    class Stack
    {
        Dictionary<int, List<Brick>> layers;
        List<Brick> bricks;
        public Stack(List<Brick> bricks)
        {
            layers = new Dictionary<int, List<Brick>>();
            int height = bricks.Max(brick => brick.reference.Z + brick.size.Z);
            for (int i = 1; i < height; i++)
            {
                layers[i] = new List<Brick>();
            }
            foreach (Brick brick in bricks)
            {
                for (int i = 0; i < brick.size.Z; i++)
                {
                    layers[brick.reference.Z + i].Add(brick);
                }
            }
            this.bricks = bricks;
            Fall();
        }

        void Fall()
        {
            Dictionary<int, List<Brick>> afterFall = new Dictionary<int, List<Brick>>();
            int height = layers.Keys.Max();
            for (int i = 1; i <= height; i++)
            {
                afterFall[i] = new List<Brick>();
            }
            for (int layer = 1; layer <= height; layer++)
            {
                foreach (Brick brick in layers[layer])
                {
                    if (layer != brick.reference.Z)
                    {
                        continue;
                    }
                    bool supported = false;
                    if (brick.reference.Z == 1)
                    {
                        supported = true;
                    }
                    int fallen = 0;
                    for (int i = layer - 1; i > 0 && !supported; i--)
                    {
                        foreach (Brick other in afterFall[i])
                        {
                            if (other.Supports(brick))
                            {
                                brick.supportedBy.Add(other);
                                other.supporting.Add(brick);
                                supported = true;
                            }
                        }
                        if (supported)
                        {
                            break;
                        }
                        brick.Fall();
                        fallen++;
                    }
                    for (int i = brick.reference.Z; i < brick.reference.Z + brick.size.Z; i++)
                    {
                        afterFall[i].Add(brick);
                    }
                }
            }
            layers = afterFall;
        }
        public int Disintegrable()
        {
            int disintegrable = 0;
            int i = 0;
            foreach (Brick brick in bricks)
            {
                bool canDisintegrate = true;
                foreach (Brick other in brick.supporting)
                {
                    if (other.supportedBy.Count == 1)
                    {
                        canDisintegrate = false;
                    }
                }
                if (canDisintegrate)
                {
                    disintegrable++;
                }
                i++;

            }
            return disintegrable;
        }

        public int causesToFall(Brick brick)
        {
            int causesToFall = 0;
            Dictionary<Brick, State> table = new Dictionary<Brick, State>();
            Queue<Brick> queue = new Queue<Brick>();
            foreach (Brick other in brick.supporting)
            {
                queue.Enqueue(other);
            }
            while (queue.Count > 0)
            {
                Brick current = queue.Dequeue();
                if (!table.ContainsKey(current))
                {
                    table[current] = new State()
                    {
                        seen = 1,
                        neededToFall = current.supportedBy.Count
                    };
                }
                else
                {
                    table[current] = new State()
                    {
                        seen = table[current].seen + 1,
                        neededToFall = table[current].neededToFall
                    };
                }
                if (table[current].seen != table[current].neededToFall)
                {
                    continue;
                }
                causesToFall++;
                foreach (Brick other in current.supporting)
                {
                    queue.Enqueue(other);
                }
            }
            return causesToFall;
        }

        public int totalCausesToFall()
        {
            int totalCausesToFall = 0;
            foreach (Brick brick in bricks)
            {
                totalCausesToFall += causesToFall(brick);
            }
            return totalCausesToFall;
        }
    }


    static int Run(string filename, bool part2 = false)
    {
        string[] lines = File.ReadAllLines(filename);
        List<Brick> bricks = new List<Brick>();
        foreach (string line in lines)
        {
            bricks.Add(ParseBrick(line));
        }
        Stack stack = new Stack(bricks);
        if (!part2)
        {
            return stack.Disintegrable();
        }
        return stack.totalCausesToFall();
    }

    static Brick ParseBrick(string line)
    {
        string[] parts = line.Split('~');
        string[] start = parts[0].Split(',');
        string[] end = parts[1].Split(',');
        return new Brick(
            new Vec3(int.Parse(start[0]), int.Parse(start[1]), int.Parse(start[2])),
            new Vec3(int.Parse(end[0]), int.Parse(end[1]), int.Parse(end[2]))
        );
    }

    public static void Main()
    {
        int result;

        int p1 = 5;
        result = Run("test");
        if (result != p1)
        {
            throw new Exception($"Expected {p1} for test, got {result}");
        }

        int p2 = 7;
        result = Run("test", true);
        if (result != p2)
        {
            throw new Exception($"Expected {p2} for test, got {result}");
        }

        result = Run("input");

        Console.WriteLine($"Solution to the first part: {result}");

        result = Run("input", true);

        Console.WriteLine($"Solution to the second part: {result}");
    }
}

