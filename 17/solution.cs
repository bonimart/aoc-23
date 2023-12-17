#r "System.Runtime.Numerics.dll"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    const int MAX_STRAIGHT = 3;
    const int MAX_STRAIGHT_ULTRA = 10;
    const int MIN_STRAIGHT_ULTRA = 4;
    
    struct Crucible
    {
        public Vector2 position;
        public Vector2 direction;
        int StraightCount;
        bool ultra;

        public Crucible(Vector2 position, Vector2 direction, bool ultra=false)
        {
            this.position = position;
            this.direction = direction;
            this.StraightCount = 0;
            this.ultra = ultra;
        }
        public Crucible(Vector2 position, Vector2 direction, int StraightCount, bool ultra=false)
        {
            this.position = position;
            this.direction = direction;
            this.StraightCount = StraightCount;
            this.ultra = ultra;
        }
            
        public List<Crucible> GetNext()
        {
            if (ultra)
            {
                return GetNextUltra();
            }
            else
            {
                return GetNextNormal();
            }
        }

        List<Crucible> GetNextNormal()
        {
            List<Crucible> crucibles = new List<Crucible>();
            if (StraightCount < MAX_STRAIGHT)
            {
                crucibles.Add(new Crucible(position + direction, direction, StraightCount + 1));
            } 
            Vector2 leftDirection = new Vector2(direction.Y, -direction.X);
            Crucible left = new Crucible(position + leftDirection, leftDirection, 1);
            Vector2 rightDirection = new Vector2(-direction.Y, direction.X);
            Crucible right = new Crucible(position + rightDirection, rightDirection, 1);
            crucibles.Add(left);
            crucibles.Add(right);
            return crucibles;
        }

        List<Crucible> GetNextUltra()
        {
            List<Crucible> crucibles = new List<Crucible>();
            if (StraightCount < MAX_STRAIGHT_ULTRA)
            {
                crucibles.Add(new Crucible(position + direction, direction, StraightCount + 1, true));
            } 
            if (StraightCount >= MIN_STRAIGHT_ULTRA)
            {
                Vector2 leftDirection = new Vector2(direction.Y, -direction.X);
                Crucible left = new Crucible(position + leftDirection, leftDirection, 1, true);
                Vector2 rightDirection = new Vector2(-direction.Y, direction.X);
                Crucible right = new Crucible(position + rightDirection, rightDirection, 1, true);
                crucibles.Add(left);
                crucibles.Add(right);
            }
            return crucibles;
        }
    }

    class Puzzle
    {
        int[,] grid;
        public int width;
        public int height;
        public Puzzle(string[] lines)
        {
            height = lines.Length;
            width = lines[0].Length;
            grid = new int[width, height];
            for (int y = 0; y < height; y++)
            {
                string line = lines[y];
                for (int x = 0; x < width; x++)
                {
                    grid[x, y] = line[x] - '0';
                }
            }
        }

        public int ShortestPath(Crucible from, Vector2 to)
        {
            HashSet<Crucible> seen = new HashSet<Crucible>();
            PriorityQueue<Crucible, int> queue = new PriorityQueue<Crucible, int>();
            Dictionary<Vector2, int> heuristic = Heuristic(to);
            queue.Enqueue(from, heuristic[from.position]);
            while (queue.Count > 0)
            {
                int priority;
                Crucible crucible;
                queue.TryDequeue(out crucible, out priority);
                int distance = priority - heuristic[crucible.position];
                if (seen.Contains(crucible))
                {
                    continue;
                }
                seen.Add(crucible);
                foreach (Crucible next in crucible.GetNext())
                {
                    if (next.position.X < 0 || next.position.X >= width ||
                            next.position.Y < 0 || next.position.Y >= height)
                    {
                        continue;
                    }
                    if (next.position == to)
                    {
                        return distance + grid[(int)next.position.X, (int)next.position.Y];
                    }
                    int nextDistance = distance + grid[(int)next.position.X, (int)next.position.Y];
                    queue.Enqueue(next, nextDistance + heuristic[next.position]);
                }
            }
            return -1;
        }

        Dictionary<Vector2, int> Heuristic(Vector2 to)
        {
            Dictionary<Vector2, int> heuristic = new Dictionary<Vector2, int>();
            HeuristicRec(new Vector2(0, 0), to, heuristic);
            return heuristic;
        }

        int HeuristicRec(Vector2 position, Vector2 to, Dictionary<Vector2, int> map)
        {
            if (map.ContainsKey(position))
            {
                return map[position];
            }
            if (position == to)
            {
                map[position] = grid[(int)position.X, (int)position.Y];
                return map[position];
            }
            if (position.X < 0 || position.X >= width ||
                    position.Y < 0 || position.Y >= height)
            {
                return int.MaxValue;
            }
            int output = Math.Min(
                    HeuristicRec(position + new Vector2(1, 0), to, map),
                    HeuristicRec(position + new Vector2(0, 1), to, map)
                    );
            output += grid[(int)position.X, (int)position.Y];
            map[position] = output;
            return output;
        }

    }

    public static void Main()
    {
        string filePath = "input";

        string[] lines = File.ReadAllLines(filePath);
        Puzzle puzzle = new Puzzle(lines);
        Crucible from = new Crucible(new Vector2(0, 0), new Vector2(1, 0));

        int shortestPath = puzzle.ShortestPath(from, new Vector2(puzzle.width - 1, puzzle.height - 1));

        Console.WriteLine($"Solution to the first part: {shortestPath}");

        from = new Crucible(new Vector2(0, 0), new Vector2(1, 0), true);
        shortestPath = puzzle.ShortestPath(from, new Vector2(puzzle.width - 1, puzzle.height - 1));

        Console.WriteLine($"Solution to the second part: {shortestPath}");
    }

}

Program.Main();

