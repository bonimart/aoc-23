using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string filePath = "input";
        Dictionary<string, int> digits = new Dictionary<string, int>()
        {
            {"one", 1},
            {"two", 2},
            {"three", 3},
            {"four", 4},
            {"five", 5},
            {"six", 6},
            {"seven", 7},
            {"eight", 8},
            {"nine",9},
            {"1", 1},
            {"2", 2},
            {"3", 3},
            {"4", 4},
            {"5", 5},
            {"6", 6},
            {"7", 7},
            {"8", 8},
            {"9",9},
        };

        // Read all lines from the file into an array
        string[] lines = File.ReadAllLines(filePath);

        int sum = 0;
        // Display each line from the array (just for demonstration)
        foreach (string line in lines)
        {
            int firstVal = -1;
            int firstIndex = -1;
            int lastVal = -1;
            int lastIndex = -1;
            foreach(KeyValuePair<string, int> pair in digits)
            {
                int fIndex = line.IndexOf(pair.Key);
                if (fIndex != -1 && (firstIndex == -1 || fIndex < firstIndex))
                {
                    firstIndex = fIndex;
                    firstVal = pair.Value;
                }
                int lIndex = line.LastIndexOf(pair.Key);
                if (lIndex != -1 && (lastIndex == -1 || lIndex > lastIndex))
                {
                    lastIndex = lIndex;
                    lastVal = pair.Value;
                }
            }
            sum += 10*firstVal + lastVal;
        }
        Console.WriteLine(sum);
    }
}
