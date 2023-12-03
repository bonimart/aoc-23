using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

class Program
{
    static int n_of_lines;
    static int line_length;
    static string[] lines;
    static Dictionary<Tuple<int, int>, int> gears = new Dictionary<Tuple<int, int>, int>();
    static Dictionary<Tuple<int, int>, int> gear_ratios = new Dictionary<Tuple<int, int>, int>();

    static void Main()
    {
        string filePath = "input";

        // Read all lines from the file into an array
        lines = File.ReadAllLines(filePath);

        int sum = 0;
        n_of_lines = lines.Length;
        line_length = lines[0].Length;
        // Display each line from the array (just for demonstration)
        for (int i = 0; i < n_of_lines; ++i)
        {
            int number = 0;
            int start = -1;
            int end = -1;
            for (int j = 0; j < line_length; ++j)
            {
                if (char.IsDigit(lines[i][j]))
                {
                    number = number * 10 + (lines[i][j] - '0');
                    start = start == -1 ? j : start;
                    end = j;
                }
                if ((end != -1 && j == line_length - 1) || (end != -1 && !char.IsDigit(lines[i][j])))
                {
                    if (IsPart(number, i, start, end))
                    {
                        sum += number;
                    }
                    start = -1;
                    end = -1;
                    number = 0;
                }

            }
        }
        int gear_ratio_sum = 0;
        foreach (KeyValuePair<Tuple<int, int>, int> gear in gears)
        {
            Tuple<int, int> key = gear.Key;
            int ratio = gear_ratios[key];
            int count = gear.Value;
            if (count != 2)
            {
                continue;
            }
            gear_ratio_sum += ratio;
        }
        Console.WriteLine($"Solution 1: {sum}");
        Console.WriteLine($"Solution 2: {gear_ratio_sum}");
    }

    static bool IsPart(int number, int line, int start, int end)
    {
        string part = "";
        bool output = false;
        for (int i = line-1; i < line+2; ++i)
        {
            for (int j = start-1; j < end+2; ++j)
            {
                if (i < 0 || i >= n_of_lines || j < 0 || j >= line_length)
                {
                    continue;
                }
                part += lines[i][j];
                if (IsSymbol(lines[i][j]))
                {
                    if (lines[i][j] == '*')
                    {
                        Tuple<int, int> key = Tuple.Create(i, j);
                        if(gears.ContainsKey(key))
                        {
                            gear_ratios[key] *= number;
                            gears[key] += 1;
                        }
                        else
                        {
                            gear_ratios.Add(key, number);
                            gears.Add(key, 1);
                        }
                    }
                    output = true;
                }
            }
            part += '\n';
        } 
        return output;
    }

    static bool IsSymbol(char c)
    {
        return !char.IsDigit(c) && c != '.' && c != '\n';
    }
}
