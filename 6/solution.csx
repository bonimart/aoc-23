#r "System.Runtime.Numerics.dll"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

static List<int> Parse(string line)
{
    string pattern = @"(\d+)";
    Regex rgx = new Regex(pattern);
    MatchCollection matches = rgx.Matches(line);
    List<int> list = new List<int>();
    foreach (Match match in matches)
    {
        list.Add(int.Parse(match.Value));
    }
    return list;
}

string filePath = "input";

// Read all lines from the file into an array
string[] lines = File.ReadAllLines(filePath);
string times = lines[0];
string distances = lines[1];


List<int> timesList = Parse(times);
List<int> distanceList = Parse(distances);

BigInteger solution = BigInteger.One;
for(int i = 0; i < timesList.Count; i++)
{
    Dictionary<int, int> possibleDistances = new Dictionary<int, int>();
    int time = timesList[i];
    int record = distanceList[i];
    int beat_record = 0;
    for(int time_holding = 0; time_holding <= time; ++time_holding)
    {
        int time_traveling = time - time_holding;
        int speed = time_holding;
        int distance_traveled = speed * time_traveling;

        if (distance_traveled > record)
        {
            beat_record++;
        }
    }
    solution *= beat_record;
}
Console.WriteLine($"Solution to the first part: {solution}");

// Part 2

solution = BigInteger.One;
ulong time = ulong.Parse((lines[0].Split(':'))[1].Replace(" ", ""));
ulong distance = ulong.Parse((lines[1].Split(':'))[1].Replace(" ", ""));

ulong first_breaking = 0;

for (ulong time_holding = 1; time_holding < time; ++time_holding)
{
    if (time - time_holding > distance / time_holding)
    {
        first_breaking = time_holding;
        break;
    }
}

ulong breaking_count = time - 2*first_breaking + 1;

Console.WriteLine($"Time: {time}");
Console.WriteLine($"Solution to the second part: {breaking_count}");


