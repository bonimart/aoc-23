#r "System.Runtime.Numerics.dll"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    class Galaxy
    {
        public int x;
        public int y;
        public Galaxy(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Galaxy(Galaxy other)
        {
            this.x = other.x;
            this.y = other.y;
        }
        public int Distance(Galaxy other)
        {
            return Math.Abs(this.x - other.x) + Math.Abs(this.y - other.y);
        }
    }

    public static void Main()
    {
        string filePath = "input";

        // Read all lines from the file BigIntegero an array
        string[] lines = File.ReadAllLines(filePath);

        List<Galaxy> galaxies = new List<Galaxy>();

        // assume square grid
        bool[,] canBeExpanded = new bool[2, lines.Length];
        for (int i = 0; i < canBeExpanded.GetLength(0); ++i)
        {
            for (int j = 0; j < canBeExpanded.GetLength(1); ++j)
            {
                canBeExpanded[i, j] = true;
            }
        }

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines.Length; x++)
            {
                // use typical x,y coordinate system
                if (lines[y][x] == '#')
                {
                    galaxies.Add(new Galaxy(x, y));
                    canBeExpanded[0, x] = false;
                    canBeExpanded[1, y] = false;
                }
            }
        }
        
        List<Galaxy> expanded = ExpandUniverse(galaxies, canBeExpanded);

        BigInteger sum = SumOfShortestPaths(expanded);

        Console.WriteLine($"Solution to the first part: {sum}");

        expanded = ExpandUniverse(galaxies, canBeExpanded, 1000000);

        sum = SumOfShortestPaths(expanded);

        Console.WriteLine($"Solution to the second part: {sum}");

    }

    static BigInteger SumOfShortestPaths(List<Galaxy> galaxies)
    {
        BigInteger sum = 0;
        for (int i = 0; i < galaxies.Count; ++i)
        {
            Galaxy from = galaxies[i];
            for (int j = 0; j < i; ++j)
            {
                Galaxy to = galaxies[j];
                sum += new BigInteger(from.Distance(to));
            }
        }
        return sum;
    }

    static List<Galaxy> ExpandUniverse(List<Galaxy> galaxies, bool[,] canBeExpanded, int expansionRate=2)
    {
        List<Galaxy> newGalaxies = new List<Galaxy>();
        foreach (Galaxy g in galaxies)
        {
            newGalaxies.Add(new Galaxy(g));
        }
        for (int x = 0; x < canBeExpanded.GetLength(1); x++)
        {
            bool canExpand = canBeExpanded[0, x];
            if (!canExpand)
                continue;
            for (int i = 0; i < galaxies.Count; ++i)
            {
                if (galaxies[i].x < x)
                    continue;
                newGalaxies[i].x += expansionRate - 1;
            }
        }
        for (int y = 0; y < canBeExpanded.GetLength(1); y++)
        {
            bool canExpand = canBeExpanded[1, y];
            if (!canExpand)
                continue;
            for (int i = 0; i < galaxies.Count; ++i)
            {
                if (galaxies[i].y < y)
                    continue;
                newGalaxies[i].y += expansionRate - 1;
            }
        }
        return newGalaxies;
    }
}

Program.Main();
