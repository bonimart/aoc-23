using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

class Program
{
    class Hand
    {
        public string cards;
        public string type;
        public static Dictionary<char, int> cardValues = new Dictionary<char, int>()
        {
            {'2', 2},
            {'3', 3},
            {'4', 4},
            {'5', 5},
            {'6', 6},
            {'7', 7},
            {'8', 8},
            {'9', 9},
            {'T', 10},
            {'J', 1},
            {'Q', 12},
            {'K', 13},
            {'A', 14},
        };
        public static Dictionary<string, int> cardTypes = new Dictionary<string, int>()
        {
            {"High Card", 1},
            {"One Pair", 2},
            {"Two Pairs", 3},
            {"Three of a Kind", 4},
            {"Full House", 5},
            {"Four of a Kind", 6},
            {"Five of a Kind", 7}
        };
        public Hand(string cards)
        {
            this.cards = cards;
            this.type = this.getType();
        }
        public static bool operator <=(Hand a, Hand b)
        {
            if (a.type == b.type){
                    for (int i = 0; i < a.cards.Length; i++)
                    {
                        if (cardValues[a.cards[i]] > cardValues[b.cards[i]])
                        {
                            return false;
                        }
                        else if (cardValues[a.cards[i]] < cardValues[b.cards[i]])
                        {
                            return true;
                        }
                    }
                    return true;
            }
            return cardTypes[a.type] <= cardTypes[b.type];
        }
        public static bool operator >=(Hand a, Hand b)
        {
            return b <= a;
        }
        public virtual string getType()
        {
            int distinctCards = cards.Distinct().Count();
            switch (distinctCards)
            {
                case 5:
                    return "High Card";
                case 4:
                    return "One Pair";
                case 3:
                    if (cards.GroupBy(x => x).Any(g => g.Count() == 3))
                    {
                        return "Three of a Kind";
                    }
                    return "Two Pairs";
                case 2:
                    if (cards.GroupBy(x => x).Any(g => g.Count() == 4))
                    {
                        return "Four of a Kind";
                    }
                    return "Full House";
                case 1:
                    return "Five of a Kind";
            }
            return "Unknown";
        }
    }

    class HandComparer : IComparer<Hand>
    {
        public int Compare(Hand a, Hand b)
        {
            if (a <= b)
            {
                return -1;
            }
            else if (a >= b)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    class WildCardHand : Hand
    {
        public new static Dictionary<char, int> cardValues = new Dictionary<char, int>()
        {
            {'J', 1},
            {'2', 2},
            {'3', 3},
            {'4', 4},
            {'5', 5},
            {'6', 6},
            {'7', 7},
            {'8', 8},
            {'9', 9},
            {'T', 10},
            {'Q', 12},
            {'K', 13},
            {'A', 14},
        };
        public WildCardHand(string cards) : base(cards)
        {
            type = this.getWildType();
        }

        public string getWildType()
        {
            int jokers = cards.Count(x => x == 'J');
            // try to replace jokers with the best possible card
            string type = this.getType();
            int distinctCards = cards.Distinct().Count();
            switch(jokers)
            {
                case 0:
                    return type;
                case 1:
                    if (distinctCards == 2)
                    {
                        return "Five of a Kind";
                    }
                    else if (distinctCards == 3)
                    {
                        int max_count = cards.GroupBy(x => x).Max(g => g.Count());
                        if (max_count == 3)
                        {
                            return "Four of a Kind";
                        }
                        else
                        {
                            return "Full House";
                        }
                    }
                    else if (distinctCards == 4)
                    {
                        return "Three of a Kind";
                    }
                    return "One Pair";
                case 2:
                    if (distinctCards == 2)
                    {
                        return "Five of a Kind";
                    }
                    else if (distinctCards == 3)
                    {
                        return "Four of a Kind";
                    }
                    return "Three of a Kind";
                case 3:
                    if (distinctCards == 2)
                    {
                        return "Five of a Kind";
                    }
                    return "Four of a Kind";
                case 4:
                    return "Five of a Kind";
                case 5:
                    return "Five of a Kind";
                default:
                    return "Unknown";
            }
        }

    }

    class WildCardHandComparer : IComparer<WildCardHand>
    {
        public int Compare(WildCardHand a, WildCardHand b)
        {
            if (a <= b)
            {
                return -1;
            }
            else if (a >= b)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }


    static void Main()
    {
        string filePath = "input";

        // Read all lines from the file into an array
        string[] lines = File.ReadAllLines(filePath);
        List<Hand> hands = new List<Hand>();
        List<WildCardHand> wildHands = new List<WildCardHand>();
        Dictionary<string, int> bids = new Dictionary<string, int>();

        foreach (string line in lines)
        {
            string[] split = line.Split(' ');
            string cards = split[0];
            int bid = int.Parse(split[1]);
            hands.Add(new Hand(cards));
            wildHands.Add(new WildCardHand(cards));
            bids.Add(cards, bid);
        }

        hands.Sort(new HandComparer());

        ulong solution = 0;
        for(int i = 0; i < hands.Count; i++)
        {
            solution += (ulong)bids[hands[i].cards] * (ulong)(i + 1);
        }

        Console.WriteLine($"Solution to the first part: {solution}");

        solution = 0;

        wildHands.Sort(new WildCardHandComparer());

        for(int i = 0; i < wildHands.Count; i++)
        {
            solution += (ulong)bids[wildHands[i].cards] * (ulong)(i + 1);
        }

        Console.WriteLine($"Solution to the second part: {solution}");

        WildCardHand hand = new WildCardHand("4321A");
        Console.WriteLine(hand.type);
    }

}

