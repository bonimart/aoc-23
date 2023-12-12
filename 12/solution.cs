#r "System.Runtime.Numerics.dll"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    public static void Main()
    {
        string filePath = "test";

        string[] lines = File.ReadAllLines(filePath);
        string[] rows;
        List<int>[] values;
        ParseInput(lines, out rows, out values);

    }

    static void ParseInput(string[] lines, out string[] rows, out List<int>[] values)
    {
        rows = new string[lines.Length];
        values = new List<int>[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] split = lines[i].Split(" ");
            rows[i] = split[0];
            values[i] = new List<int>();
            split = split[1].Split(",");
            for (int j = 0; j < split.Length; j++)
            {
                values[i].Add(int.Parse(split[j]));
            }
        }
    }

    static int NumOfArrangements(string row, List<int> toAssign)
    {
        return 0;
    }

}

Program.Main();
