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
            int matching = GetMatching();
            if (matching == 0)
            {
                return 0;
            }
            return 2 ^ (GetMatching() - 1);
        }

        public int GetMatching()
        {
            int matching = 0;
            foreach (int win in winning)
            {
                foreach (int avail in available)
                {
                    if (win == avail)
                    {
                        matching++;
                    }
                }
            }
            return matching;
        }
    }
    static void Main()
    {
        string filePath = "input";

        // Read all lines from the file into an array
        string[] lines = File.ReadAllLines(filePath);
        Card[] cards = new Card[lines.Length];

        int sum = 0;
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            Card card = ParseLine(line);
            sum += card.GetPoints();
            cards[i] = card;
        }

        Console.WriteLine($"Solution 1: {sum}");

        int scratchcard_sum = 0;
        int[] card_counts = new int[lines.Length];
        for (int i = 0; i < lines.Length; ++i)
        {
            Card card = cards[i];
            card_counts[i]++;
            int matching = card.GetMatching();
            for (int j = 1; j <= matching; ++j)
            {
                card_counts[i+j] += card_counts[i];
            }
            scratchcard_sum += card_counts[i];
        }

        Console.WriteLine($"Solution 2: {scratchcard_sum}");

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
