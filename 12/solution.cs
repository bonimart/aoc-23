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
        string filePath = "input";

        string[] lines = File.ReadAllLines(filePath);
        string[] rows;
        List<int>[] values;
        ParseInput(lines, out rows, out values);

        BigInteger sum = 0;

        for (int i = 0; i < lines.Length; ++i)
        {
            int n = NumOfArrangements(rows[i], values[i]);
            sum += n;
        }

        Console.WriteLine($"Solution of the first part: {sum}");
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
        Dictionary<string, Dictionary<int, int>> memo = new Dictionary<string, Dictionary<int, int>>();

        return NumOfArrangementsRec(row, toAssign, memo);
    }

    static int NumOfArrangementsRec(string row, List<int> toAssign, Dictionary<string, Dictionary<int, int>> memo)
    {
        if (memo.ContainsKey(row) && memo[row].ContainsKey(toAssign.Count()))
        {
            return memo[row][toAssign.Count()];
        }
        if (toAssign.Count() == 0)
        {
            if (row.Contains("#"))
                return 0;
            return 1;
        }
        else if (row.Length == 0)
        {
            return 0;
        }
        int index;
        int result;
        if (row.Contains("?"))
        {
           index = row.IndexOf("?"); 
           string withDot = row.Substring(0, index) + "." + row.Substring(index + 1);
           string withHash = row.Substring(0, index) + "#" + row.Substring(index + 1);
           result = NumOfArrangementsRec(withDot, toAssign, memo) + NumOfArrangementsRec(withHash, toAssign, memo);
           if (!memo.ContainsKey(row))
           {
               memo.Add(row, new Dictionary<int, int>());
           }
           memo[row][toAssign.Count()] = result;
           return result;
        }
        int assigned = toAssign[0];
        index = row.IndexOf("#");
        if (index == -1 || index + assigned > row.Length)
        {
            if (!memo.ContainsKey(row))
            {
                memo.Add(row, new Dictionary<int, int>());
            }
            memo[row][toAssign.Count()] = 0;
            return 0;
        }
        string s = row.Substring(index, assigned);
        if (s.Contains(".") || (row.Length > index + assigned && row[index + assigned] == '#'))
        {
            if (!memo.ContainsKey(row))
            {
                memo.Add(row, new Dictionary<int, int>());
            }
            memo[row][toAssign.Count()] = 0;
            return 0;
        }
        string newRow = index + assigned + 1 >= row.Length ? "" : row.Substring(index + assigned + 1);
        List<int> newToAssign = toAssign.Skip(1).ToList();
        result = NumOfArrangementsRec(newRow, newToAssign, memo);
        if (!memo.ContainsKey(row))
        {
            memo.Add(row, new Dictionary<int, int>());
        }
        memo[row][toAssign.Count()] = result;
        return result;
    }

}

Program.Main();
