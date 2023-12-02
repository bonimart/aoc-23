using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string filePath = "input";

        Dictionary<string, int> bag = new Dictionary<string, int>()
        {
            {"red", 12},
            {"green", 13},
            {"blue", 14}
        };
        // Read all lines from the file into an array
        string[] lines = File.ReadAllLines(filePath);

        int sum = 0;
        // Display each line from the array (just for demonstration)
        foreach (string line in lines)
        {
            int gameId = ParseGameId(line);
            bool possible = true;
            foreach (KeyValuePair<string, int> entry in bag)
            {
                string pattern = @"(\d+) " + entry.Key;
                Regex rgx = new Regex(pattern);
                MatchCollection matches = rgx.Matches(line);
                foreach (Match match in matches)
                {
                    if (Int32.Parse(match.Groups[1].Value) > entry.Value)
                    {
                        possible = false;
                        break;
                    }
                }
            }
            if (possible)
            {
                sum += gameId;
            }
        }
        Console.WriteLine(sum);
    }

    static int ParseGameId(string line)
    {
        string pattern = @"^Game (\d+):";
        Regex rgx = new Regex(pattern);
        MatchCollection matches = rgx.Matches(line);
        if (matches.Count == 0)
        {
            return -1;
        }
        return Int32.Parse(matches[0].Groups[1].Value);
    }
}
