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
                    if (IsPart(i, start, end))
                    {
                        sum += number;
                    }
                    start = -1;
                    end = -1;
                    number = 0;
                }

            }
        }
        Console.WriteLine($"Solution 1: {sum}");
    }

    static bool IsPart(int line, int start, int end)
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
                    output = true;
                }
            }
            part += '\n';
        } 
        if (output)
        {
            Console.Write(part);
        }
        return output;
    }
    static bool IsSymbol(char c)
    {
        return !char.IsDigit(c) && c != '.' && c != '\n';
    }
}
