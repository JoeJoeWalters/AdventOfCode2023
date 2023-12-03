using FluentAssertions;
using System.Drawing;
using Xunit.Abstractions;

namespace AdventOfCode2023
{
    /*
     * --- Day 3: Gear Ratios ---
    You and the Elf eventually reach a gondola lift station; he says the gondola lift will take you up to the water source, but this is as far as he can bring you. You go inside.

    It doesn't take long to find the gondolas, but there seems to be a problem: they're not moving.

    "Aaah!"

    You turn around to see a slightly-greasy Elf with a wrench and a look of surprise. "Sorry, I wasn't expecting anyone! The gondola lift isn't working right now; it'll still be a while before I can fix it." You offer to help.

    The engineer explains that an engine part seems to be missing from the engine, but nobody can figure out which one. If you can add up all the part numbers in the engine schematic, it should be easy to work out which part is missing.

    The engine schematic (your puzzle input) consists of a visual representation of the engine. There are lots of numbers and symbols you don't really understand, but apparently any number adjacent to a symbol, even diagonally, is a "part number" and should be included in your sum. (Periods (.) do not count as a symbol.)

    Here is an example engine schematic:

    467..114..
    ...*......
    ..35..633.
    ......#...
    617*......
    .....+.58.
    ..592.....
    ......755.
    ...$.*....
    .664.598..
    In this schematic, two numbers are not part numbers because they are not adjacent to a symbol: 114 (top right) and 58 (middle right). Every other number is adjacent to a symbol and so is a part number; their sum is 4361.

    Of course, the actual engine schematic is much larger. What is the sum of all of the part numbers in the engine schematic?
    */

    public class Day3
    {
        private readonly ITestOutputHelper output;

        public Day3(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Part1()
        {
            // ARRANGE
            int expectedResult = 554003;
            var schematic = DataToDateSet(File.ReadAllLines(@".\Day3_Part1.txt"));

            // ACT
            var result = Solver(schematic, 1);

            // ASSERT
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void Part2()
        {
            // ARRANGE
            int expectedResult = 87263515;
            var schematic = DataToDateSet(File.ReadAllLines(@".\Day3_Part2.txt"));

            // ACT
            var result = Solver(schematic, 2);

            // ASSERT
            result.Should().Be(expectedResult);
        }

        /// <summary>
        /// A Number and what positions it exists in (as it may span multiple points for >9 numbers)
        /// </summary>
        private record Number
        {
            public int Value { get; set; }
            public HashSet<Point> Positions { get; set; } = new HashSet<Point>();
        }

        /// <summary>
        /// Data broken down into a queryable set with the symbol by position
        /// </summary>
        private record DataSet
        {
            public List<Number> Numbers = new List<Number>();
            public Dictionary<char, HashSet<Point>> Symbols = new Dictionary<char, HashSet<Point>>();
        };

        private int Solver(DataSet dataSet, int part)
        {
            switch (part)
            {
                case 1:

                    var positions = dataSet.Symbols.Values.SelectMany(set => set).ToHashSet();
                    return dataSet.Numbers.Select(num =>
                        {
                            var neighbours = num.Positions.SelectMany(pos => GetNeighbourPoints(pos)).ToHashSet();
                            return (positions.Any(neighbours.Contains)) ? num.Value : 0;
                        }
                    ).Sum();

                default:

                    var gearPositions = dataSet.Symbols['*'];
                    return gearPositions.Select(pos =>
                    {
                        var neighboursNum = dataSet.Numbers.Where(num => num.Positions.Any(GetNeighbourPoints(pos).Contains)).ToList();
                        return (neighboursNum.Count == 2) ? neighboursNum[0].Value * neighboursNum[1].Value : 0;
                    }).Sum();
            }
        }

        private HashSet<Point> GetNeighbourPoints(Point current)
        {
            var set = new HashSet<Point>();

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    set.Add(new Point(x: current.X + x, y: current.Y + y));
                }
            }

            set.Remove(item: current); // I probably shouldn't have added it but meh
            return set;
        }

        private DataSet DataToDateSet(string[] lines)
        {
            var result = new DataSet();

            for (var y = 0; y < lines.Length; y++)
            {
                for (var x = 0; x < lines[0].Length; x++)
                {
                    char symbol = lines[y][x];
                    if (symbol != '.')
                    {
                        // Not an empty space and not a numeric digit (Should have done this on day 1 to be honest instead of checking the ascii value, so I changed it :) )
                        if (!char.IsDigit(symbol))
                        {
                            if (!result.Symbols.ContainsKey(symbol))
                                result.Symbols.Add(symbol, new HashSet<Point>() { new Point(x, y) });
                            else
                                result.Symbols[symbol].Add(item: new Point(x, y));
                            continue;
                        }

                        var positions = new HashSet<Point> { new(x, y) };
                        var span = 1;

                        // How long is the number? We'll need to grab the whole thing
                        // but record it is adjacent
                        while (x + span < lines[0].Length && char.IsDigit(lines[y][x + span]))
                        {
                            positions.Add(item: new Point(x: x + span, y));
                            span++;
                        }

                        // Now parse the whole area and store it
                        var value = int.Parse(lines[y][x..(x + span)]); // Ranges are my new favourtie thing :) 
                        var number = new Number() { Value = value, Positions = positions };

                        result.Numbers.Add(number);
                        x += span - 1; // Force the loop to skip to after the number we just processed
                    }
                }
            }

            return result;
        }
    }
}