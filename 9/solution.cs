#r "System.Runtime.Numerics.dll"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    public static void Main()
    {
        string filePath = "input";

        // Read all lines from the file BigIntegero an array
        string[] lines = File.ReadAllLines(filePath);

        List<List<BigInteger>> sequences = new List<List<BigInteger>>();

        foreach (string line in lines)
        {
            sequences.Add(ParseLine(line));
        }

        BigInteger sum = 0;

        foreach (List<BigInteger> sequence in sequences)
        {
            sum += Extrapolate(sequence);
        }

        Console.WriteLine($"Solution to the first part: {sum}");

        BigInteger sumBackwards = 0;
        foreach (List<BigInteger> sequence in sequences)
        {
            sumBackwards += Extrapolate(sequence, true);
        }

        Console.WriteLine($"Solution to the second part: {sumBackwards}");

    }

    static List<BigInteger> ParseLine(string line)
    {
        List<BigInteger> sequence = new List<BigInteger>();

        string[] numbers = line.Split(' ');

        foreach (string number in numbers)
        {
            sequence.Add(BigInteger.Parse(number));
        }

        return sequence;
    }

    static BigInteger Extrapolate(List<BigInteger> sequence, bool backwards = false)
    {
        if (sequence.TrueForAll(x => x == 0))
        {
            return 0;
        }

        List<BigInteger> newSequence = new List<BigInteger>();
        for (int i = 1; i < sequence.Count; i++)
        {
            newSequence.Add(sequence[i] - sequence[i - 1]);
        }

        if (backwards)
        {
            return sequence[0] - Extrapolate(newSequence, true);
        }

        return Extrapolate(newSequence) + sequence[sequence.Count - 1];
    }
}

Program.Main();
