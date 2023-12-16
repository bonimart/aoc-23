#r "System.Runtime.Numerics.dll"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    static int HashAlgorithm(string s)
    {
        int hash = 0;
        foreach (char c in s)
        {
            hash += c;
            hash *= 17;
            hash %= 256;
        }
        return hash;
    }

    public static void Main()
    {
        string filePath = "input";

        string[] lines = File.ReadAllLines(filePath);
        List<string> commands = new List<string>();
        ParseInput(lines, out commands);

        BigInteger result = 0;
        foreach (string command in commands)
        {
            result += HashAlgorithm(command);
        }

        Console.WriteLine($"Solution to the first part: {result}");
    }

    static void ParseInput(string[] lines, out List<string> commands)
    {
        commands = new List<string>();
        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            commands.AddRange(parts);
        }
    }

}

Program.Main();

