using FluentAssertions;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace AdventOfCode2023
{
    /*
    --- Day 1: Trebuchet?! ---
    Something is wrong with global snow production, and you've been selected to take a look. The Elves have even given you a map; on it, they've used stars to mark the top fifty locations that are likely to be having problems.

    You've been doing this long enough to know that to restore snow operations, you need to check all fifty stars by December 25th.

    Collect stars by solving puzzles. Two puzzles will be made available on each day in the Advent calendar; the second puzzle is unlocked when you complete the first. Each puzzle grants one star. Good luck!

    You try to ask why they can't just use a weather machine ("not powerful enough") and where they're even sending you ("the sky") and why your map looks mostly blank ("you sure ask a lot of questions") and hang on did you just say the sky ("of course, where do you think snow comes from") when you realize that the Elves are already loading you into a trebuchet ("please hold still, we need to strap you in").

    As they're making the final adjustments, they discover that their calibration document (your puzzle input) has been amended by a very young Elf who was apparently just excited to show off her art skills. Consequently, the Elves are having trouble reading the values on the document.

    The newly-improved calibration document consists of lines of text; each line originally contained a specific calibration value that the Elves now need to recover. On each line, the calibration value can be found by combining the first digit and the last digit (in that order) to form a single two-digit number.

    For example:

    1abc2
    pqr3stu8vwx
    a1b2c3d4e5f
    treb7uchet
    In this example, the calibration values of these four lines are 12, 38, 15, and 77. Adding these together produces 142.

    Consider your entire calibration document. What is the sum of all of the calibration values?
    */

    public class Day1
    {
        private readonly string[] _data;
        private const int _expectedResult = 52840;

        private enum DecodeStyle
        {
            NumbersOnly = 0,
            NumbersAndWords = 1
        }

        public Day1()
        {
            // Read all of the terminal input
            _data = File.ReadAllLines(@".\Day1.txt");
        }

        [Fact]
        public void Part1()
        {
            // ARRANGE
            int sum = 0;

            // ACT
            _data.ToList().ForEach(line => { sum += DecodeLine(line, DecodeStyle.NumbersAndWords); });

            // ASSERT
            sum.Should().Be(_expectedResult);
        }

        private int DecodeLine(string line, DecodeStyle style)
        {
            List<string> numbers = new List<string>();
            switch (style)
            {
                case DecodeStyle.NumbersAndWords:
                    numbers = DecodeWords(line);
                    break;
                default:
                    numbers = SplitToNumbers(line);
                    break;
            }

            return numbers.Count switch
            {
                0 => 0,
                1 => int.Parse($"{numbers.First()}{numbers.First()}"),
                _ => int.Parse($"{numbers.First()}{numbers.Last()}")
            };
        }

        private List<string> DecodeWords(string line)
        {
            Dictionary<string, string> replacements = new Dictionary<string, string>()
            {
                { "one", "1" },
                { "two", "2" },
                { "three", "3" },
                { "four", "4" },
                { "five", "5" },
                { "six", "6" },
                { "seven", "7" },
                { "eight", "8" },
                { "nine", "9" },
                { "ten", "10" },
                { "eleven", "11" },
                { "twelve", "12" },
                { "thirteen", "13" },
                { "fourteen", "14" },
                { "fifteen", "15" },
                { "sixteen", "16" },
                { "seventeen", "17" },
                { "eighteen", "18" },
                { "nineteen", "19" },
                { "twenty", "20" }
            };

            List<string> split = new List<string>();
            for (var i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                int asc = (int)ch; 
                if (asc >= 48 && asc <= 57)
                {
                    split.Add(ch.ToString());
                }
                else
                {
                    foreach (var rep in replacements.Reverse())
                    {
                        if ((i + rep.Key.Length <= line.Length) && line.Substring(i, rep.Key.Length) == rep.Key)
                        {
                            split.Add(rep.Value);
                            break;
                        }
                    }
                }
            }


            List<string> result = new List<string>();
            split.ForEach(item => { 
                    result.AddRange(SplitToNumbers(item));
            });

            return result;
        }

        private List<string> SplitToNumbers(string item)
            => item.ToCharArray().Where(ch => { int asc = (int)ch; return (asc >= 48 && asc <= 57); }).Select(x => x.ToString()).ToList();
    }
}