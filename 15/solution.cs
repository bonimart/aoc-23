#r "System.Runtime.Numerics.dll"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{

    struct Lens
    {
        public string label;
        public int focalLength;
        public Lens(string label, int focalLength)
        {
            this.label = label;
            this.focalLength = focalLength;
        }
        public Lens(Lens lens)
        {
            this.label = lens.label;
            this.focalLength = lens.focalLength;
        }
    }

    class Box
    {
        List<Lens> slots;

        public Box()
        {
            slots = new List<Lens>();
        }

        public void Insert(Lens lens)
        {
            int index = Find(lens.label);
            if (index == -1)
            {
                slots.Add(lens);
            }
            else
            {
                slots[index] = lens;
            }
        }

        public void Pop(string label)
        {
            int index = Find(label);
            if (index != -1)
            {
                slots.RemoveAt(index);
            }
        }

        public int Find(string label)
        {
            int index = -1;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].label == label)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public void Command(string command)
        {
            if (command.EndsWith("-"))
            {
                Pop(command.Substring(0, command.Length - 1));
                return;
            }
            string[] parts = command.Split('=');
            string label = parts[0];
            int focalLength = int.Parse(parts[1]);
            Insert(new Lens(label, focalLength));
        }

        public int FocusingPower()
        {
            int result = 0;
            for (int i = 0; i < slots.Count; i++)
            {
                result += (i + 1) * slots[i].focalLength;
            }
            return result;
        }

        public void Print()
        {
            Console.WriteLine("Box:");
            foreach (Lens lens in slots)
            {
                Console.WriteLine($"  {lens.label} -> {lens.focalLength}");
            }
        }
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

        result = 0;
        int LEN = 256;
        Box[] boxes = new Box[LEN];

        for (int i = 0; i < LEN; ++i)
        {
            boxes[i] = new Box();
        }

        foreach (string command in commands)
        {
            int index = HashAlgorithm(ParseLabel(command));
            boxes[index].Command(command);
        }

        for (int i = 0; i < LEN; ++i)
        {
            result += (i + 1) * boxes[i].FocusingPower();
        }

        Console.WriteLine($"Solution to the second part: {result}");
    }

    static string ParseLabel(string command)
    {
        if (command.EndsWith("-"))
        {
            return command.Substring(0, command.Length - 1);
        }
        string[] parts = command.Split('=');
        return parts[0];
    }

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

