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
        List<string[]> patterns = new List<string[]>();
        ParseInput(lines, out patterns);

        int sum = countReflections(patterns);

        Console.WriteLine($"Solution of the first part: {sum}");

        sum = countReflections(patterns, 1);

        Console.WriteLine($"Solution of the second part: {sum}");

    }

    static int countReflections(List<string[]> patterns, int smudges=0)
    {
        int sum = 0;
        foreach (string[] pattern in patterns)
        {
            int reflection = findReflection(pattern, smudges: smudges);
            if (reflection != -1)
            {
                sum += reflection;
                continue;
            }
            reflection = findReflection(pattern, false, smudges:smudges);
            if (reflection != -1)
            {
                sum += reflection;
                continue;
            }
        }
        return sum;
    }

    static int findReflection(string[] pattern, bool vertical=true, int horizontal_multiplier=100, int smudges=0)
    {
        Dictionary<int, HashSet<int>> same = new Dictionary<int, HashSet<int>>();
        // index, where the axes of reflection moves
        int width = vertical ?  pattern[0].Length : pattern.Length;
        for (int i = 0; i < width; ++i)
        {
            same[i] = new HashSet<int>();
        }
        for (int i = 1; i < width; ++i)
        {
            //check reflections if mirror is placed at i
            if (canReflect(pattern, i, vertical, same, smudges))
            {
                // because of notation
                return vertical ? i : i * horizontal_multiplier;
            }
        }
        return -1;
    }

    static bool canReflect(string[] pattern, int index, bool vertical, Dictionary<int, HashSet<int>> same, int smudges)
    {
        int height = vertical ? pattern.Length : pattern[0].Length;
        int width = vertical ? pattern[0].Length : pattern.Length;
        int axis_range = Math.Min(index, width - index);
        int smudge_count = 0;
        for(int i = 0; i < axis_range; ++i)
        {
            int former = index - i - 1;
            int image = index + i;
            int temp_smudge_count = 0;
            foreach (int j in same[former])
            {
                if (same[image].Contains(j))
                {
                    makeSame(same, former, image);
                    continue;
                }
            }
            foreach (int j in same[image])
            {
                if (same[former].Contains(j))
                {
                    makeSame(same, former, image);
                    continue;
                }
            }
            for (int j = 0; j < height; ++j)
            {
                if (vertical)
                {
                    if (pattern[j][former] != pattern[j][image])
                        temp_smudge_count++;
                }
                else
                {
                    if (pattern[former][j] != pattern[image][j])
                        temp_smudge_count++;
                }
                if (temp_smudge_count > smudges)
                    return false;
            }
            smudge_count += temp_smudge_count;
            if (temp_smudge_count == 0)
                makeSame(same, former, image);
        }
        if (smudge_count != smudges)
            return false;
        return true;
    }

    static void makeSame(Dictionary<int, HashSet<int>> same, int i, int j)
    {
        same[i].Add(j);
        same[j].Add(i);
    }

    static void ParseInput(string[] lines, out List<string[]> patterns)
    {
        patterns = new List<string[]>();
        int from = 0;
        string[] pattern;
        for (int to = 0; to < lines.Length; ++to)
        {
            if (lines[to] == "")
            {
                pattern = new string[to - from];
                for (int i = from; i < to; ++i)
                {
                    pattern[i - from] = lines[i];
                }
                patterns.Add(pattern);
                from = to + 1;
            }
        }
    }

}

