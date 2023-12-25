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
        struct Neighbour 
        {
            public Tile tile;
            public int distance;
            public Neighbour(Tile tile, int distance)
            {
                this.tile = tile;
                this.distance = distance;
            }
        }

        Tile[,] Tiles;
        HashSet<Tile> Visitable;
        int Width, Height;
        Dictionary<Tile, List<Neighbour>> Neighbours = new Dictionary<Tile, List<Neighbour>>();

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
            Simplify();
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
            return LongestPathRec(start, end, visited) - 1;
        }

        int LongestPathRec(Tile start, Tile end, HashSet<Tile> visited)
        {
            if (start == end)
            {
                return 0;
            }
            int longest = Int32.MinValue;
            visited.Add(start);
            foreach (Neighbour neighbour in Neighbours[start])
            {
                Tile tile = neighbour.tile;
                if (visited.Contains(tile))
                {
                    continue;
                }
                visited.Add(tile);
                int length = LongestPathRec(tile, end, visited) + neighbour.distance;
                if (length > longest)
                {
                    longest = length;
                }
                visited.Remove(tile);
            }
            return longest;
        }

        List<Tile> directNeighbours(Tile tile)
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

        void Simplify()
        {
            foreach (Tile tile in Visitable)
            {
                List<Tile> neighbours = directNeighbours(tile); 
                if (neighbours.Count == 2)
                {
                    continue;
                }
                Neighbours[tile] = new List<Neighbour>();
                foreach (Tile neighbour in neighbours)
                {
                    Neighbour n = FindNeighbour(tile, neighbour);
                    Neighbours[tile].Add(n);
                }
            }
        }

        Neighbour FindNeighbour(Tile source, Tile n)
        {
            Tile last = n;
            int distance = 1;
            HashSet<Tile> visited = new HashSet<Tile>();
            visited.Add(source);
            while(directNeighbours(last).Count <= 2 && !visited.Contains(last))
            {
                distance++;
                visited.Add(last);
                List<Tile> next = directNeighbours(last)
                    .Where(tile => !visited.Contains(tile))
                    .ToList();
                foreach (Tile tile in next)
                {
                    last = tile;
                }
            }
            return new Neighbour(last, distance);
        }

        void Print()
        {
            for (int y = 0; y < Height; y++)
            {
                string line = "";
                for (int x = 0; x < Width; x++)
                {
                    if (Neighbours.Keys.Contains(Tiles[x, y]))
                    {
                        line += $"{Neighbours[Tiles[x, y]].Count}";
                    }
                    else if (Tiles[x, y] is Path)
                    {
                        line += ".";
                    }
                    else if (Tiles[x, y] is Slope)
                    {
                        line += "^";
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

    const char FOREST = '#';
    const char PATH = '.';
    static readonly Dictionary<char, Vec2> SLOPES = new Dictionary<char, Vec2>()
    {
        { '^', new Vec2(0, -1) },
        { 'v', new Vec2(0, 1) },
        { '<', new Vec2(-1, 0) },
        { '>', new Vec2(1, 0) }
    };

    static Map Parse(string[] lines, bool part2 = false)
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
                        if (part2)
                        {
                            tiles[x, y] = new Path(new Vec2(x, y));
                            break;
                        }
                        char slope = lines[y][x];
                        tiles[x, y] = new Slope(new Vec2(x, y), SLOPES[slope]);
                        break;
                }
            }
        }
        return new Map(tiles);
    }

    static int Run(string filename, bool part2 = false)
    {
        string[] lines = File.ReadAllLines(filename);
        Map map = Parse(lines, part2);
        return map.LongestPath();
    }

    public static void Main()
    {
        BigInteger result;

        var P1T2 = 3;
        result = Run("test2");
        if (result != P1T2)
        {
            throw new Exception($"Part 1, test failed, expected {P1T2}, got {result}");
        }

        var P1 = 94;
        result = Run("test");
        if (result != P1)
        {
            throw new Exception($"Part 1, test failed, expected {P1}, got {result}");
        }

        // part 1
        result = Run("input");
        Console.WriteLine($"Part 1: {result}");

        var P2 = 154;
        result = Run("test", true);
        if (result != P2)
        {
            throw new Exception($"Part 2, test failed, expected {P2}, got {result}");
        }

        // part 2
        result = Run("input", true);
        Console.WriteLine($"Part 2: {result}");
    }
}

