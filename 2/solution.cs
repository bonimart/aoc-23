using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

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
        int sum_of_powers = 0;
        // Display each line from the array (just for demonstration)
        foreach (string line in lines)
        {
            int gameId = ParseGameId(line);
            bool possible = true;
            // the minimal possible values that could have been in the bag
            Dictionary<string, int> minimal_bag = new Dictionary<string, int>()
            {
                {"red", 0},
                {"green", 0},
                {"blue", 0}
            };
            foreach (KeyValuePair<string, int> entry in bag)
            {
                string pattern = @"(\d+) " + entry.Key;
                Regex rgx = new Regex(pattern);
                MatchCollection matches = rgx.Matches(line);
                foreach (Match match in matches)
                {
                    int value = Int32.Parse(match.Groups[1].Value);
                    if (value > entry.Value)
                    {
                        possible = false;
                    }
                    if (value > minimal_bag[entry.Key])
                    {
                        minimal_bag[entry.Key] = value;
                    }
                }
            }
            int power = minimal_bag.Values.Aggregate(1, (current, value) => current * value);
            sum_of_powers += power;
            if (possible)
            {
                sum += gameId;
            }
        }
        Console.WriteLine($"Solution 1: {sum}");
        Console.WriteLine($"Solution 2: {sum_of_powers}");
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
