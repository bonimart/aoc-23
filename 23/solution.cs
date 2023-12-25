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

    class Tile
    {
        public Vec2 pos;
        public Tile(Vec2 pos)
        {
            this.pos = pos;
        }

        public virtual List<Vec2> Neighbours()
        {
            throw new Exception("Not implemented");
        }
    }

    class Path : Tile
    {
        public Path(Vec2 pos) : base(pos)
        {
        }

        public override List<Vec2> Neighbours()
        {
            List<Vec2> neighbours = new List<Vec2>();
            foreach (Vec2 dir in directions.Values)
            {
                neighbours.Add(pos + dir);
            }
            return neighbours;
        }

    }

    class Slope : Tile
    {
        public Vec2 dir;
        public Slope(Vec2 pos, Vec2 dir) : base(pos)
        {
            this.dir = dir;
        }

        public override List<Vec2> Neighbours()
        {
            return new List<Vec2>() { pos + dir };
        }
    }


    class Map 
    {
        Tile[,] Tiles;
        HashSet<Tile> Visitable;
        int Width, Height;

        public Map(Tile[,] tiles)
        {
            Tiles = tiles;
            Width = tiles.GetLength(0);
            Height = tiles.GetLength(1);
            Visitable = new HashSet<Tile>();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (tiles[x, y] is Path || tiles[x, y] is Slope)
                    {
                        Visitable.Add(tiles[x, y]);
                    }

                }
            }
        }

        Tile Start()
        {
            for (int x = 0; x < Width; x++)
            {
                if (Tiles[x, 0] is Path)
                {
                    return Tiles[x, 0];
                }
            }
            return null;
        }

        Tile End()
        {
            for (int x = 0; x < Width; x++)
            {
                if (Tiles[x, Height - 1] is Path)
                {
                    return Tiles[x, Height - 1];
                }
            }
            return null;
        }

        public int LongestPath()
        {
            Tile start = Start();
            Tile end = End();
            HashSet<Tile> visited = new HashSet<Tile>();
            return LongestPathRec(start, end, visited);
        }

        public int LongestPathRec(Tile start, Tile end, HashSet<Tile> visited)
        {
            if (start == end)
            {
                return 0;
            }
            int longest = 0;
            visited.Add(start);
            foreach (Tile neighbour in neighbours(start))
            {
                if (visited.Contains(neighbour))
                {
                    continue;
                }
                visited.Add(neighbour);
                int length = LongestPathRec(neighbour, end, visited);
                if (length > longest)
                {
                    longest = length;
                }
                visited.Remove(neighbour);
            }
            return longest + 1;
        }

        List<Tile> neighbours(Tile tile)
        {
            List<Tile> neighbours = new List<Tile>();
            List<Vec2> positions = tile.Neighbours();
            foreach (Vec2 pos in positions)
            {
                if (pos.X < 0 || pos.X >= Width || pos.Y < 0 || pos.Y >= Height)
                {
                    continue;
                }
                if (!Visitable.Contains(Tiles[pos.X, pos.Y]))
                {
                    continue;
                }
                neighbours.Add(Tiles[pos.X, pos.Y]);
            }
            return neighbours;
        }
    }

    const char FOREST = '#';
    const char PATH = '.';
    static readonly Dictionary<char, Vec2> SLOPES = new Dictionary<char, Vec2>()
    {
        { '^', new Vec2(0, -1) },
        { 'v', new Vec2(0, 1) },
        { '<', new Vec2(-1, 0) },
        { '>', new Vec2(1, 0) }
    };

    static Map Parse(string[] lines)
    {
        int width = lines[0].Length;
        int height = lines.Length;
        Tile[,] tiles = new Tile[width, height];
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                switch (lines[y][x])
                {
                    case FOREST:
                        tiles[x, y] = new Tile(new Vec2(x, y));
                        break;
                    case PATH:
                        tiles[x, y] = new Path(new Vec2(x, y));
                        break;
                    default:
                        char slope = lines[y][x];
                        tiles[x, y] = new Slope(new Vec2(x, y), SLOPES[slope]);
                        break;
                }
            }
        }
        return new Map(tiles);
    }

    static int Run(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        Map map = Parse(lines);
        return map.LongestPath();
    }

    public static void Main()
    {
        BigInteger result;

        var P1 = 94;
        result = Run("test");
        if (result != P1)
        {
            throw new Exception($"Part 1, test failed, expected {P1}, got {result}");
        }

        // part 1
        result = Run("input");
        Console.WriteLine($"Part 1: {result}");
    }
}

