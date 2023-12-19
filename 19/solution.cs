using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    const string START_WORKFLOW = "in";
    const string ACCEPT="A";
    const string REJECT="R";

    struct Part
    {
        public int X, M, A, S;

        public int Score()
        {
            return X + M + A + S;
        }
    }

    struct Interval
    {
        public int Start, End;
        public Interval(int start, int end)
        {
            Start = start;
            End = end;
        }
        public bool IsValid()
        {
            return Start <= End;
        }
        public int Length()
        {
            return End - Start + 1;
        }
        public Tuple<Interval, Interval> Split(int value)
        {
            if (value < Start || value > End)
                throw new Exception("Value is not in the interval");
            return new Tuple<Interval, Interval>(
                    new Interval(Start, value),
                    new Interval(value + 1, End));
        }
        public Interval Diff(Interval interval)
        {
            if (interval.Start > End || interval.End < Start)
                return this;
            if (interval.Start <= Start && interval.End >= End)
                return new Interval(0, 0);
            if (interval.Start <= Start)
                return new Interval(interval.End + 1, End);
            return new Interval(Start, interval.Start - 1);
        }
        public Interval Intersect(Interval interval)
        {
            if (interval.Start > End || interval.End < Start)
                return new Interval(0, 0);
            return new Interval(Math.Max(Start, interval.Start), Math.Min(End, interval.End));
        }
    }

    struct PartInterval
    {
        public Interval X, M, A, S;
        public PartInterval(Interval x, Interval m, Interval a, Interval s)
        {
            X = x;
            M = m;
            A = a;
            S = s;
        }
        public PartInterval(PartInterval interval)
        {
            X = interval.X;
            M = interval.M;
            A = interval.A;
            S = interval.S;
        }
        public PartInterval Intersect(PartInterval interval)
        {
            PartInterval intersect = new PartInterval();
            intersect.X = X.Intersect(interval.X);
            intersect.M = M.Intersect(interval.M);
            intersect.A = A.Intersect(interval.A);
            intersect.S = S.Intersect(interval.S);
            return intersect;
        }
        public BigInteger Score()
        {
            BigInteger x = X.Length();
            x *= M.Length();
            x *= A.Length();
            x *= S.Length();
            return x;
        }
    }

    enum Property
    {
        X,
        M,
        A,
        S
    }

    readonly static Dictionary<char, Property> PROPERTY_MAP = new Dictionary<char, Property>()
    {
        { 'x', Property.X },
        { 'm', Property.M },
        { 'a', Property.A },
        { 's', Property.S }
    };

    enum Operator
    {
        LT,
        GT,
    }

    readonly static Dictionary<char, Operator> OPERATOR_MAP = new Dictionary<char, Operator>()
    {
        { '<', Operator.LT },
        { '>', Operator.GT }
    };

    class Rule
    {
        Property Property;
        Operator Operator;
        int Value;
        public string NextWorkflow;

        public Rule(char property, char op, int value, string nextWorkflow)
        {
            Property = PROPERTY_MAP[property];
            Operator = OPERATOR_MAP[op];
            Value = value;
            NextWorkflow = nextWorkflow;
        }

        public bool Check(Part part)
        {
            switch(Property)
            {
                case Property.X:
                    return CheckProperty(part.X);
                case Property.M:
                    return CheckProperty(part.M);
                case Property.A:
                    return CheckProperty(part.A);
                case Property.S:
                    return CheckProperty(part.S);
                default:
                    throw new Exception("Unknown property");
            }
        }

        public PartInterval Check(PartInterval interval, bool opposite = false)
        {
            PartInterval holds = interval;
            switch (Property)
            {
                case Property.X:
                    holds.X = CheckProperty(interval.X, opposite);
                    break;
                case Property.M:
                    holds.M =  CheckProperty(interval.M, opposite);
                    break;
                case Property.A:
                    holds.A = CheckProperty(interval.A, opposite);
                    break;
                case Property.S:
                    holds.S = CheckProperty(interval.S, opposite);
                    break;
                default:
                    throw new Exception("Unknown property");
            }
            return holds;
        }

        bool CheckProperty(int value)
        {
            switch (Operator)
            {
                case Operator.LT:
                    return value < Value;
                case Operator.GT:
                    return value > Value;
                default:
                    throw new Exception("Unknown operator");
            }
        }

        Interval CheckProperty(Interval interval, bool opposite = false)
        {
            switch (Operator)
            {
                case Operator.LT:
                    if (opposite)
                        return new Interval(Value, interval.End);
                    return new Interval(interval.Start, Value - 1);
                case Operator.GT:
                    if (opposite)
                        return new Interval(interval.Start, Value);
                    return new Interval(Value + 1, interval.End);
                default:
                    throw new Exception("Unknown operator");
            }
        }
    }

    class Workflow
    {
        public string Name;
        public string Default;
        public List<Rule> Rules;

        public Workflow(string name, string defaultWorkflow)
        {
            Name = name;
            Default = defaultWorkflow;
            Rules = new List<Rule>();
        }

        public string Run(Part part)
        {
            foreach (Rule rule in Rules)
            {
                if (rule.Check(part))
                    return rule.NextWorkflow;
            }
            return Default;
        }
    }
    

    static void ReadInput(string[] lines, out List<Part> parts, out Dictionary<string, Workflow> workflows)
    {
        parts = new List<Part>();
        workflows = new Dictionary<string, Workflow>();
        bool readingParts = false;
        foreach (string line in lines)
        {
            if (line == "")
            {
                readingParts = true;
                continue;
            }
            if (readingParts)
            {
                parts.Add(ReadPart(line));
            }
            else
            {
                Workflow workflow = ReadWorkflow(line);
                workflows.Add(workflow.Name, workflow);
            }
        }
    }

    const string WORKFLOW_REGEX = @"(\w+){(.*),(\w+)}";
    static Workflow ReadWorkflow(string line)
    {
        Match match = Regex.Match(line, WORKFLOW_REGEX);
        Workflow workflow = new Workflow(match.Groups[1].Value, match.Groups[3].Value);
        string[] rules = match.Groups[2].Value.Split(',');
        foreach (string rule in rules)
        {
            workflow.Rules.Add(ReadRule(rule));
        }
        return workflow;
    }

    const string RULE_REGEX = @"([xmas])([<>])(\d+):(\w+)";
    static Rule ReadRule(string line)
    {
        Match match = Regex.Match(line, RULE_REGEX);
        char property = match.Groups[1].Value[0];
        char op = match.Groups[2].Value[0];
        int value = int.Parse(match.Groups[3].Value);
        string nextWorkflow = match.Groups[4].Value;
        return new Rule(property, op, value, nextWorkflow);
    }

    const string PART_REGEX = @"{x=(\d+),m=(\d+),a=(\d+),s=(\d+)}";
    static Part ReadPart(string line)
    {
        Part part;
        Match match = Regex.Match(line, PART_REGEX);
        part.X = int.Parse(match.Groups[1].Value);
        part.M = int.Parse(match.Groups[2].Value);
        part.A = int.Parse(match.Groups[3].Value);
        part.S = int.Parse(match.Groups[4].Value);
        return part;
    }

    static string RunPart(Part part, Dictionary<string, Workflow> workflows)
    {
        string currentWorkflow = START_WORKFLOW;
        while (currentWorkflow != ACCEPT && currentWorkflow != REJECT)
        {
            Workflow workflow = workflows[currentWorkflow];
            currentWorkflow = workflow.Run(part);
        }
        return currentWorkflow;
    }

    static int Evaluate(List<Part> parts, Dictionary<string, Workflow> workflows)
    {
        int score = 0;
        foreach (Part part in parts)
        {
            string result = RunPart(part, workflows);
            if (result == ACCEPT)
                score += part.Score();
        }
        return score;
    }

    const int PROPERTY_MIN = 1;
    const int PROPERTY_MAX = 4000;
    static readonly PartInterval START_INTERVAL = new PartInterval()
    {
        X = new Interval(PROPERTY_MIN, PROPERTY_MAX),
        M = new Interval(PROPERTY_MIN, PROPERTY_MAX),
        A = new Interval(PROPERTY_MIN, PROPERTY_MAX),
        S = new Interval(PROPERTY_MIN, PROPERTY_MAX),
    };

    static void Accepts(Workflow workflow, Dictionary<string, Workflow> workflows, PartInterval interval, List<PartInterval> accepted)
    {
        PartInterval def = new PartInterval(interval);
        foreach (Rule rule in workflow.Rules)
        {
            PartInterval holds = rule.Check(def);
            PartInterval doesNotHold = rule.Check(def, true);
            def = def.Intersect(doesNotHold);
            if(rule.NextWorkflow == ACCEPT)
            {
                accepted.Add(holds);
            }
            else if (rule.NextWorkflow != REJECT)
            {
                Accepts(workflows[rule.NextWorkflow], workflows, holds, accepted);
            }
        }
        if (workflow.Default == ACCEPT)
        {
            accepted.Add(def);
        }
        else if (workflow.Default != REJECT)
        {
            Accepts(workflows[workflow.Default], workflows, def, accepted);
        }
    }

    static BigInteger MaxPossibleScore(Dictionary<string, Workflow> workflows)
    {
        List<PartInterval> accepted = new List<PartInterval>();
        Accepts(workflows[START_WORKFLOW], workflows, START_INTERVAL, accepted);
        BigInteger score = 0;
        foreach (PartInterval interval in accepted)
        {
            score += interval.Score();
        }
        return score;
    }

    public static void Main()
    {
        string filePath = "input";
        string[] lines = File.ReadAllLines(filePath);
        List<Part> parts;
        Dictionary<string, Workflow> workflows;
        ReadInput(lines, out parts, out workflows);
        int score = Evaluate(parts, workflows);
        Console.WriteLine($"Solution to the first part: {score}");

        BigInteger max_score = MaxPossibleScore(workflows);
        Console.WriteLine($"Solution to the second part: {max_score}");
    }
}

