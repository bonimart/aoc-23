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
    
    struct Crucible
    {
        public Vector2 position;
        public Vector2 direction;
        int StraightCount;

        public Crucible(Vector2 position, Vector2 direction)
        {
            this.position = position;
            this.direction = direction;
            this.StraightCount = 0;
        }
        public Crucible(Vector2 position, Vector2 direction, int StraightCount)
        {
            this.position = position;
            this.direction = direction;
            this.StraightCount = StraightCount;
        }
            
        public List<Crucible> GetNext()
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
            queue.Enqueue(from, 0);
            // djikstra
            while (queue.Count > 0)
            {
                int distance;
                Crucible crucible;
                queue.TryDequeue(out crucible, out distance);
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
                    queue.Enqueue(next, nextDistance);
                }
            }
            return -1;
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
    }

}

Program.Main();

