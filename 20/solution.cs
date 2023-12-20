using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    enum Pulse
    {
        HIGH,
        LOW
    }

    const Pulse HIGH = Pulse.HIGH;
    const Pulse LOW = Pulse.LOW;
    const string BROADCASTER = "broadcaster";
    const string FLIP_FLOP = "%";
    const string CONJUNCTION = "&";
    const int PUSH_TIMES = 1000;
    const string OUT = "rx";

    struct Packet
    {
        public string origin;
        public string destination;
        public Pulse pulse;
    }

    class Module
    {
        public struct Sent
        {
            public BigInteger low;
            public BigInteger high;

            public void Increment(Pulse pulse)
            {
                if (pulse == HIGH)
                {
                    high++;
                }
                else
                {
                    low++;
                }
            }
        }
        public static Dictionary<string, Module> modules = new Dictionary<string, Module>();
        protected static Dictionary<string, int> destination_counts = new Dictionary<string, int>();
        public static Queue<Packet> toSend = new Queue<Packet>();
        public static Sent sent = new Sent { low = 0, high = 0 };
        public static int count = 0;

        public static void PushButton(Pulse init)
        {
            toSend.Enqueue(new Packet { origin = BROADCASTER, destination = BROADCASTER, pulse = init });
            sent.Increment(init); 
            while (toSend.Count > 0)
            {
                Packet packet = toSend.Dequeue();
                modules[packet.destination].Receive(packet);
            }
            count++;
        }
        public static void Clear()
        {
            modules.Clear();
            toSend.Clear();
            destination_counts.Clear();
            sent = new Sent { low = 0, high = 0 };
        }
        public static void Print()
        {
            foreach (KeyValuePair<string, Module> entry in modules)
            {
                Console.WriteLine($"{entry.Key} -> {string.Join(", ", entry.Value.destinations)}");
            }
        }

        protected string name;
        protected string[] destinations;
        public Module(string name, string[] destinations)
        {
            modules[name] = this;
            this.name = name;
            this.destinations = destinations;
            foreach (string destination in destinations)
            {
                if (!destination_counts.ContainsKey(destination))
                {
                    destination_counts[destination] = 0;
                }
                destination_counts[destination]++;
            }
        }
        protected void Send(Pulse pulse)
        {
            foreach (string destination in destinations)
            {
                sent.Increment(pulse);
                toSend.Enqueue(new Packet { origin = name, destination = destination, pulse = pulse });
            }
        }
        public virtual void Receive(Packet packet)
        {
        }
    }

    class FlipFlop : Module
    {
        Pulse state;
        public FlipFlop(string name, string[] destinations) : base(name, destinations)
        {
            state = LOW;
        }
        public override void Receive(Packet packet)
        {
            Pulse pulse = packet.pulse;
            if (pulse == HIGH)
            {
                return;
            }
            state = state == HIGH ? LOW : HIGH;
            Send(state);
        }
    }

    class Conjunction : Module
    {
        Dictionary<string, Pulse> memory;
        int highCount;
        public Conjunction(string name, string[] destinations) : base(name, destinations)
        {
            memory = new Dictionary<string, Pulse>();
            highCount = 0;
        }
        public override void Receive(Packet packet)
        {
            Pulse old;
            if (!memory.ContainsKey(packet.origin))
            {
                old = LOW;
            }
            else
            {
                old = memory[packet.origin];
            }
            Pulse pulse = packet.pulse;
            memory[packet.origin] = pulse;
            if (old == LOW && pulse == HIGH)
            {
                highCount++;
            }
            else if (old == HIGH && pulse == LOW)
            {
                highCount--;
            }
            if (highCount == destination_counts[name])
            {
                Send(LOW);
            }
            else
            {
                Send(HIGH);
            }
        }

    }

    class Broadcaster : Module
    {
        public Broadcaster(string name, string[] destinations) : base(name, destinations)
        {
        }
        public override void Receive(Packet packet)
        {
            Send(packet.pulse);
        }
    }
                    
    static void Run(string filePath, out BigInteger firstPart)
    {
        //--Part 1--//
        string[] lines = File.ReadAllLines(filePath);
        HashSet<string> names = new HashSet<string>();
        firstPart = 0;
        foreach (string line in lines)
        {
            Parse(line, names);
        }
        ResolveUnmatched(names);
        for (int i = 0; i < PUSH_TIMES; i++)
        {
            Module.PushButton(LOW);
        }
        firstPart = Module.sent.high * Module.sent.low;
        Module.Clear();
    }

    static void Parse(string line, HashSet<string> names)
    {
        string[] parts = line.Split(" -> ");
        string input = parts[0];
        string[] destinations = parts[1].Split(", ");
        string name;
        if (input == BROADCASTER)
        {
            name = input;
            new Broadcaster(name, destinations);
        }
        else if (input.StartsWith(FLIP_FLOP))
        {
            name = input.Substring(1);
            new FlipFlop(name, destinations);
        }
        else if (input.StartsWith(CONJUNCTION))
        {
            name = input.Substring(1);
            new Conjunction(name, destinations);
        }
        else
        {
            throw new Exception("Unknown input");
        }
        names.Add(name);
        foreach (string destination in destinations)
        {
            names.Add(destination);
        }
    }

    static void ResolveUnmatched(HashSet<string> names)
    {
        foreach (string name in names)
        {
            if (!Module.modules.ContainsKey(name))
            {
                new Module(name, new string[] {});
            }
        }
    }

    public static void Main()
    {
        BigInteger firstPart;

        int T1P1 = 32000000;
        Run("test1", out firstPart);
        if (firstPart != T1P1)
        {
            throw new Exception($"Part 1, test 1 failed, expected {T1P1}, got {firstPart}");
        }

        Run("test2", out firstPart);
        int T2P1 = 11687500;
        if (firstPart != T2P1)
        {
            throw new Exception($"Part 1, test 2 failed, expected {T2P1}, got {firstPart}");
        }

        Run("input", out firstPart);

        Console.WriteLine($"Solution to the first part: {firstPart}");
    }
}

