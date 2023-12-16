#r "System.Numerics.Vectors.dll"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;

class Program
{
    enum Orientation
    {
        Up,
        Down,
        Left,
        Right
    }

    Dictionary<Vector2, Orientation> orientations = new Dictionary<Vector2, Orientation>()
    {
        { new Vector2(0, -1), Orientation.Up },
        { new Vector2(0, 1), Orientation.Down },
        { new Vector2(-1, 0), Orientation.Left },
        { new Vector2(1, 0), Orientation.Right }
    };

    struct Beam
    {
        public Vector2 position;
        public Vector2 direction;
        public Beam(Vector2 position, Vector2 direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }

    class Tile
    {
        public Tile(){}
        public virtual List<Beam> Interact(Beam beam)
        {
            List<Beam> beams = new List<Beam>();
            beams.Add(new Beam(beam.position + beam.direction, beam.direction));
            return beams;
        }
        public virtual void Print()
        {
            Console.Write(".");
        }
    }


    class Mirror : Tile
    {
        // "/" -> Up
        // "\" -> Down
        public Orientation orientation;
        public Mirror(Orientation orientation) : base()
        {
            this.orientation = orientation;
        }
        public override List<Beam> Interact(Beam beam)
        {
            List<Beam> beams = new List<Beam>();
            int sign = orientation == Orientation.Up ? -1 : 1;
            Vector2 direction = new Vector2(sign * beam.direction.Y, sign * beam.direction.X);
            Beam b = new Beam(
                    beam.position + direction,
                    direction);
            beams.Add(b);
            return beams;
        }
        public override void Print()
        {
            if (orientation == Orientation.Up)
            {
                Console.Write("/");
            }
            else
            {
                Console.Write("\\");
            }
        }
    }

    class Splitter : Tile
    {
        // "|" -> Up
        // "-" -> Down
        public Orientation orientation;
        public Splitter(Orientation orientation) : base()
        {
            this.orientation = orientation;
        }
        public override List<Beam> Interact(Beam beam)
        {
            if (
                    orientation == Orientation.Up && beam.direction.X == 0
                    || orientation == Orientation.Down && beam.direction.Y == 0
                )
            {
                return base.Interact(beam);
            }
            List<Beam> beams = new List<Beam>();
            Vector2 direction = new Vector2(beam.direction.Y, beam.direction.X);
            Beam b1 = new Beam(
                    beam.position + direction,
                    direction);
            Beam b2 = new Beam(
                    beam.position - direction,
                    - direction);

            beams.Add(b1);
            beams.Add(b2);
            return beams;
        }

        public override void Print()
        {
            if (orientation == Orientation.Up)
            {
                Console.Write("|");
            }
            else
            {
                Console.Write("-");
            }
        }
    }


    class Contraption
    {
        Tile[,] tiles;
        int width;
        int height;
        
        public Contraption(string[] lines)
        {
            this.width = lines[0].Length;
            this.height = lines.Length;
            this.tiles = new Tile[width, height];
            for (int y = 0; y < height; y++)
            {
                string line = lines[y];
                for (int x = 0; x < width; x++)
                {
                    tiles[x, y] = ParseTile(line[x]);
                }
            }
        }

        public int Energize(Beam beam)
        {
            List<Beam> beams = new List<Beam>();
            HashSet<Vector2> energized = new HashSet<Vector2>();
            HashSet<Beam> seen = new HashSet<Beam>();
            beams.Add(beam);
            while (beams.Count > 0)
            {
                List<Beam> newBeams = new List<Beam>();
                foreach (Beam b in beams)
                {
                    Vector2 position = b.position;
                    if (position.X < 0 || position.X >= width ||
                            position.Y < 0 || position.Y >= height ||
                            seen.Contains(b))
                    {
                        continue;
                    }
                    energized.Add(position);
                    seen.Add(b);
                    Tile tile = tiles[(int)position.X, (int)position.Y];
                    newBeams.AddRange(tile.Interact(b));
                }
                beams = newBeams;
            }

            return energized.Count;
        }

        public int MaxEnergy()
        {
            int maxEnergy = 0;
            for (int y = 0; y < height; y++)
            {
                // left
                Beam b1 = new Beam(new Vector2(0, y), new Vector2(1, 0));
                int energy = Energize(b1);
                if (energy > maxEnergy)
                {
                    maxEnergy = energy;
                }
                // right
                Beam b2 = new Beam(new Vector2(width - 1, y), new Vector2(-1, 0));
                energy = Energize(b2);
                if (energy > maxEnergy)
                {
                    maxEnergy = energy;
                }
            }
            for (int x = 0; x < width; x++)
            {
                // top
                Beam b1 = new Beam(new Vector2(x, 0), new Vector2(0, 1));
                int energy = Energize(b1);
                if (energy > maxEnergy)
                {
                    maxEnergy = energy;
                }
                // bottom
                Beam b2 = new Beam(new Vector2(x, height - 1), new Vector2(0, -1));
                energy = Energize(b2);
                if (energy > maxEnergy)
                {
                    maxEnergy = energy;
                }
            }
            return maxEnergy;
        }

        Tile ParseTile(char c)
        {
            switch (c)
            {
                case '/':
                    return new Mirror(Orientation.Up);
                case '\\':
                    return new Mirror(Orientation.Down);
                case '|':
                    return new Splitter(Orientation.Up);
                case '-':
                    return new Splitter(Orientation.Down);
                default:
                    return new Tile();
            }
        }

        public void Print()
        {
            for (int y = 0; y < height; y++)
            {
                string line = "";
                for (int x = 0; x < width; x++)
                {
                    tiles[x, y].Print();
                }
                Console.WriteLine(line);
            }
        }
    }

    public static void Main()
    {
        string filePath = "test";

        string[] lines = File.ReadAllLines(filePath);
        Contraption contraption = new Contraption(lines);
        Beam beam = new Beam(new Vector2(0, 0), new Vector2(1, 0));
        int result = contraption.Energize(beam);

        Console.WriteLine($"Solution to the first part: {result}");

    }

}

Program.Main();

