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
            ulong n = NumOfArrangements(rows[i], values[i]);
            sum += n;
        }

        Console.WriteLine($"Solution of the first part: {sum}");

        Unfold(rows, values);

        sum = 0;
        for (int i = 0; i < lines.Length; ++i)
        {
            ulong n = NumOfArrangements(rows[i], values[i]);
            sum += n;
        }

        Console.WriteLine($"Solution of the second part: {sum}");
    }

    static void Unfold(string[] rows, List<int>[] values, int ratio=4)
    {
        for (int i = 0; i < rows.Length; ++i)
        {
            string row = rows[i];
            List<int> value = values[i];
            values[i] = new List<int>();
            values[i].AddRange(value);
            for (int j = 0; j < ratio; ++j)
            {
                rows[i] = rows[i] + "?" + row;
                values[i].AddRange(value);
            }
        }
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

    static ulong NumOfArrangements(string row, List<int> toAssign)
    {
        Dictionary<string, Dictionary<int, ulong>> memo = new Dictionary<string, Dictionary<int, ulong>>();

        return NumOfArrangementsRec(row, toAssign, memo);
    }

    static ulong NumOfArrangementsRec(string row, List<int> toAssign, Dictionary<string, Dictionary<int, ulong>> memo)
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
        int assigned = toAssign[0];
        index = row.IndexOf("#") == -1 ? row.Length : row.IndexOf("#");
        int indexQuestion = row.IndexOf("?") == -1 ? row.Length : row.IndexOf("?");
        index = Math.Min(index, indexQuestion);
        if (index == row.Length)
        {
            if (!memo.ContainsKey(row))
            {
                memo.Add(row, new Dictionary<int, ulong>());
            }
            memo[row][toAssign.Count()] = 0;
            return 0;
        }

        ulong result = 0;
        if (row[index] == '?')
        {
            result =  NumOfArrangementsRec(row.Substring(index + 1), toAssign, memo);
        }

        if (index + assigned > row.Length)
        {
            if (!memo.ContainsKey(row))
            {
                memo.Add(row, new Dictionary<int, ulong>());
            }
            memo[row][toAssign.Count()] = result;
            return result;
        }
        string s = row.Substring(index, assigned);
        if (s.Contains(".") || (row.Length > index + assigned && row[index + assigned] == '#'))
        {
            if (!memo.ContainsKey(row))
            {
                memo.Add(row, new Dictionary<int, ulong>());
            }
            memo[row][toAssign.Count()] = result;
            return result;
        }
        string newRow = index + assigned + 1 >= row.Length ? "" : row.Substring(index + assigned + 1);
        List<int> newToAssign = toAssign.Skip(1).ToList();
        result += NumOfArrangementsRec(newRow, newToAssign, memo);
        if (!memo.ContainsKey(row))
        {
            memo.Add(row, new Dictionary<int, ulong>());
        }
        memo[row][toAssign.Count()] = result;
        return result;
    }

}

Program.Main();
