using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

class Program
{
    struct Card
    {
        public int[] winning;
        public int[] available;

        public int GetPoints()
        {
            int points = 0;
            foreach (int win in winning)
            {
                foreach (int avail in available)
                {
                    if (win == avail)
                    {
                        points = points == 0 ? 1 : points * 2;
                    }
                }
            }
            return points;
        }
    }
    static void Main()
    {
        string filePath = "input";

        // Read all lines from the file into an array
        string[] lines = File.ReadAllLines(filePath);

        int sum = 0;
        foreach (string line in lines)
        {
            Card card = ParseLine(line);
            sum += card.GetPoints();
        }
        Console.WriteLine($"Solution 1: {sum}");
    }

    static Card ParseLine(string line)
    {
        string wo_card_name = line.Split(':')[1];
        string[] split = wo_card_name.Split('|');
        string winning = split[0];
        string available = split[1];
        Card card = new Card();

        string pattern = @"\b\d+\b";
        MatchCollection matches = Regex.Matches(winning, pattern);
        card.winning = matches.Cast<Match>().Select(m => int.Parse(m.Value)).ToArray();
        matches = Regex.Matches(available, pattern);
        card.available = matches.Cast<Match>().Select(m => int.Parse(m.Value)).ToArray();

        return card;
    }
}
