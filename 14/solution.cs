using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    enum Direction
    {
        N,
        S,
        E,
        W
    }

    struct Rock
    {
        public int x;
        public int y;
        public Rock(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Rock(Rock r)
        {
            this.x = r.x;
            this.y = r.y;
        }
    }

    class Board
    {
        // (x, y)
        static Dictionary<Direction, Tuple<int, int>> directions = new Dictionary<Direction, Tuple<int, int>>()
        {
            {Direction.N, Tuple.Create(0, -1)},
            {Direction.S, Tuple.Create(0, 1)},
            {Direction.E, Tuple.Create(1, 0)},
            {Direction.W, Tuple.Create(-1, 0)}
        };
        HashSet<Rock> cube_rocks;
        HashSet<Rock> round_rocks;
        int width;
        int height;

        public Board(string[] pattern)
        {
            cube_rocks = new HashSet<Rock>();
            round_rocks = new HashSet<Rock>();
            width = pattern[0].Length;
            height = pattern.Length;
            for (int y = 0; y < height; ++y)
            {
                string row = pattern[y];
                for (int x = 0; x < width; ++x)
                {
                    if (row[x] == '#')
                    {
                        cube_rocks.Add(new Rock(x, y));
                    }
                    else if (row[x] == 'O')
                    {
                        round_rocks.Add(new Rock(x, y));
                    }
                }
            }
        }

        public Board(HashSet<Rock> cube_rocks, HashSet<Rock> round_rocks, int width, int height)
        {
            this.cube_rocks = cube_rocks;
            this.round_rocks = round_rocks;
            this.width = width;
            this.height = height;
        }

        public List<List<Rock>> ToRoll(Direction dir)
        {
            int roll_axis = dir == Direction.N || dir == Direction.S ? width : height;
            List<List<Rock>> to_roll = new List<List<Rock>>();
            for (int i = 0; i < roll_axis; ++i)
            {
                List<Rock> to_add = new List<Rock>();
                if (dir == Direction.N || dir == Direction.S)
                {

                    to_add = round_rocks.Where(r => r.x == i).ToList();
                    if (dir == Direction.N)
                        to_add.Sort((a, b) => a.y.CompareTo(b.y));
                    else
                        to_add.Sort((a, b) => b.y.CompareTo(a.y));
                }
                else
                {
                    to_add = round_rocks.Where(r => r.y == i).ToList();
                    if (dir == Direction.W)
                        to_add.Sort((a, b) => a.x.CompareTo(b.x));
                    else
                        to_add.Sort((a, b) => b.x.CompareTo(a.x));
                }
                to_roll.Add(to_add);
            }
            return to_roll;
        }

        public Board Roll(Direction dir)
        {
            int dx = directions[dir].Item1;
            int dy = directions[dir].Item2;
            int roll_axis = dir == Direction.N || dir == Direction.S ? width : height;
            HashSet<Rock> new_round_rocks = new HashSet<Rock>(round_rocks);
            List<List<Rock>> to_roll = ToRoll(dir);
            for (int i = 0; i < roll_axis; ++i)
            {
                List<Rock> rolling = to_roll[i];
                foreach (Rock r in rolling)
                {
                    Rock after_roll = new Rock(r);
                    new_round_rocks.Remove(r);
                    while (CanRoll(after_roll, dir, new_round_rocks))
                    {
                        after_roll.x += dx;
                        after_roll.y += dy;
                    }
                    new_round_rocks.Add(after_roll);
                }
            }
            return new Board(cube_rocks, new_round_rocks, width, height);
        }

        bool CanRoll(Rock r, Direction dir, HashSet<Rock> round_rocks)
        {
            int dx = directions[dir].Item1;
            int dy = directions[dir].Item2;
            Rock next = new Rock(r.x + dx, r.y + dy);
            if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
                return false;
            if (cube_rocks.Contains(next))
                return false;
            if (round_rocks.Contains(next))
                return false;
            return true;
        }

        public int CalculateLoad(Direction dir)
        {
            bool vertical = dir == Direction.N || dir == Direction.S;
            int reference = dir == Direction.S || dir == Direction.E ? 0 : vertical ? height : width;
            int sum = 0;
            foreach(Rock r in round_rocks)
            {
                if (vertical)
                {
                    sum += Math.Abs(r.y - reference);
                }
                else 
                {
                    sum += Math.Abs(r.x - reference);
                }
            }
            return sum;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Board b = obj as Board;
            if (b == null)
                return false;
            return round_rocks.SetEquals(b.round_rocks);
        }

        public override int GetHashCode()
        {
            return round_rocks.GetHashCode();
        }
    }

    static Board RollCycle(Board board)
    {
        Board tmp = board.Roll(Direction.N);
        tmp = tmp.Roll(Direction.W);
        tmp = tmp.Roll(Direction.S);
        tmp = tmp.Roll(Direction.E);
        return tmp;
    }

    public static void Main()
    {
        string filePath = "input";

        // Read all lines from the file BigIntegero an array
        string[] lines = File.ReadAllLines(filePath);
        Board board = new Board(lines);

        Board rolled_north = board.Roll(Direction.N);
        int load = rolled_north.CalculateLoad(Direction.N);
        Console.WriteLine($"Solution of the first part: {load}");

        int cycles = 1000000000;
        Dictionary<Board, int> seen = new Dictionary<Board, int>();
        int period = 0;
        Board rolled = board;
        for (int i = 0; i < cycles; ++i)
        {
            seen.Add(rolled, i);
            Board tmp = RollCycle(rolled);
            if (seen.ContainsKey(tmp))
            {
                rolled = tmp;
                period = i - seen[tmp] + 1;
                break;
            }
            rolled = tmp;
        }

        int remaining = (cycles - seen[rolled]) % period;
        for (int i = 0; i < remaining; ++i)
        {
            rolled = RollCycle(rolled);
        }

        load = rolled.CalculateLoad(Direction.N);

        Console.WriteLine($"Solution of the second part: {load}");
    }

}

