using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

class Program
{
    struct Range
    {
        public ulong min;
        public ulong max;
        public bool Contains(ulong value)
        {
            return value >= min && value <= max;
        }
        public bool Intersects(Range range)
        {
            return range.Contains(min) || range.Contains(max) || Contains(range.min) || Contains(range.max);
        }
        public Range Intersect(Range range)
        {
            if (!Intersects(range))
            {
                throw new Exception("Ranges do not intersect");
            }
            return new Range { min = Math.Max(min, range.min), max = Math.Min(max, range.max) };
        }

        static public List<Range> Simplify(List<Range> ranges)
        {
            List<Range> simplified = new List<Range>();
            ranges = ranges.OrderBy(x => x.min).ToList();
            for(int i = 0; i < ranges.Count; i++)
            {
                Range range = ranges[i];
                for(int j = i + 1; j < ranges.Count; j++)
                {
                    Range other = ranges[j];
                    if(range.max + 1 == other.min)
                    {
                        range.max = other.max;
                        i++;
                    }
                    else if(range.min <= other.min && range.max >= other.max)
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                simplified.Add(range);
            }
            return simplified;
        }
    }

    class Mapping
    {
        public Mapping()
        {
            map = new Dictionary<Range, Range>();
        }
        public Dictionary<Range, Range> map;
        public ulong Map(ulong value)
        {
            foreach (KeyValuePair<Range, Range> entry in map)
            {
                if (entry.Key.Contains(value))
                {
                    return value - entry.Key.min + entry.Value.min;
                }
            }
            return value;
        }
        
        public List<Range> MapRange(Range range)
        {
            List<Range> out_ranges = new List<Range>();
            List<Range> source_ranges = map.Keys.OrderBy(x => x.min).ToList();
            foreach (Range source_range in source_ranges)
            {
                Range dest_range = map[source_range];
                if (range.max < source_range.min)
                {
                    out_ranges.Add(range);
                    break;
                }
                if (range.min < source_range.min)
                {
                    out_ranges.Add(new Range { min = range.min, max = source_range.min - 1 });
                    range.min = source_range.min;
                }
                if (range.min <= source_range.max && range.max <= source_range.max)
                {
                    out_ranges.Add(new Range { min = dest_range.min + range.min - source_range.min, max = dest_range.min + range.max - source_range.min });
                    break;
                }
                else if (range.min <= source_range.max)
                {
                    out_ranges.Add(new Range { min = dest_range.min + range.min - source_range.min, max = dest_range.max});
                    range.min = source_range.max + 1;
                }
            }
            return out_ranges;
        }
    }
    
    static void Main()
    {
        string filePath = "input";

        // Read all lines from the file into an array
        string[] lines = File.ReadAllLines(filePath);

        List<Mapping> mappings = new List<Mapping>();
        List<ulong> seeds = ParseSeeds(lines[0]);

        ulong[] mapping = new ulong[3];
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].EndsWith(":"))
            {
                mappings.Add(new Mapping());
                continue;
            }
            else if (lines[i] == "")
            {
                continue;
            }
            string[] split = lines[i].Split(' ');
            for (int j = 0; j < 3; j++)
            {
                mapping[j] = ulong.Parse(split[j]);
            }
            ulong range = mapping[2];
            Range from = new Range { min = mapping[1], max = mapping[1] + range - 1};
            Range to = new Range { min = mapping[0], max = mapping[0] + range - 1};
            mappings[mappings.Count - 1].map.Add(from, to);
        }

        ulong result = ulong.MaxValue;
        foreach(ulong seed in seeds)
        {
            ulong value = seed;
            foreach(Mapping map in mappings)
            {
                value = map.Map(value);
            }
            result = result > value ? value : result;
        }

        Console.WriteLine($"Solution to part 1: {result}");


        List<Range> seedRanges = ParseRanges(seeds);
        ulong min = ulong.MaxValue;
        foreach (Range seedRange in seedRanges)
        {
            List<Range> ranges = new List<Range>();
            ranges.Add(seedRange);
            List<Range> newRanges = new List<Range>();
            foreach (Mapping map in mappings)
            {
                foreach (Range range in ranges)
                {
                    newRanges.AddRange(map.MapRange(range));
                }
                newRanges = Range.Simplify(newRanges);
                ranges.Clear();
                ranges.AddRange(newRanges);
                newRanges.Clear();
            }
            foreach (Range range in ranges)
            {
                min = min > range.min ? range.min : min;
            }
        }
        Console.WriteLine($"Solution to part 2: {min}");

    }

    static List<ulong> ParseSeeds(string line)
    {
        List<ulong> seeds = new List<ulong>();
        string[] split = line.Split(':')[1].Split(' ').Where(x => x != "").ToArray();
        foreach (string num in split)
        {
            seeds.Add(ulong.Parse(num));
        }
        return seeds;
    }

    static List<Range> ParseRanges(List<ulong> numbers)
    {
        List<Range> ranges = new List<Range>();
        for (int i = 0; i < numbers.Count; i += 2)
        {
            ranges.Add(new Range { min = numbers[i], max = numbers[i] + numbers[i + 1] - 1});
        }
        return ranges;
    }
}

