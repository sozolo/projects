using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Puzzle
{
    class Program
    {
        //int wordCount = 0;
        //List<string> wordList = new List<string>();
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"Input.txt");
            //check input is in matrix format
            foreach (var line in lines)
            {
                if (line.Length != lines[0].Length)
                {
                    Console.WriteLine("Wrong input format.\nPress any key to exit.");
                    Console.ReadKey();
                    return;
                }
            }

            //the only reason I'm convertin it to char[,] is because it was specified in the requirement document
            char[,] puzzle = new char[lines.Count(), lines[0].Length];
            for (int i = 0; i < lines.Length; i++)
            {
                var charLine = lines[i].ToCharArray();
                for (int j = 0; j < charLine.Length; j++)
                {
                    puzzle[i, j] = charLine[j];
                }
            }
            Console.WriteLine("Total word count: {0}\nPress any key to exit.", PuzzleSolver.FindWords(puzzle));
            Console.ReadKey();
        }
    }

    public static class PuzzleSolver
    {
        public static string[] DICTIONARY = { "OX", "CAT", "TOY", "AT", "DOG", "CATAPULT", "T" };

        static bool IsWord(string testWord)
        {
            if (DICTIONARY.Contains(testWord))
                return true;
            return false;
        }

        public static int FindWords(char[,] puzzle)
        {
            var wordCount = 0;
            var wordList = new List<string>();
            int x = puzzle.GetLength(0);
            int y = puzzle.GetLength(1);

            var isHorizontal = true;
            wordCount += TestStraightLines(puzzle, ref wordList, x, y, isHorizontal);    //horizontal lines
            isHorizontal = false;
            wordCount += TestStraightLines(puzzle, ref wordList, y, x, isHorizontal);     //vertical lines

            var isRotated = false;
            wordCount += TestDiagonals(puzzle, ref wordList, x, y, isRotated);
            isRotated = true;
            wordCount += TestDiagonals(puzzle, ref wordList, x, y, isRotated);
            return wordCount;
        }

        //rotates horizontal and vertical lines depending on the order in which dementions were passed
        public static int TestStraightLines(char[,] puzzle, ref List<string> wordList, int x, int y, bool isHorizontal)
        {
            var count = 0;
            for (int i = 0; i < x; i++)
            {
                //check each horizontal line
                var sb = new StringBuilder();
                for (int j = 0; j < y; j++)
                {
                    if (isHorizontal)
                    {
                        sb.Append(puzzle[i, j]);
                    }
                    else
                    {
                        sb.Append(puzzle[j, i]);
                    }
                }
                var line = sb.ToString();
                count += TestLine(line, ref wordList, isHorizontal); //direct order
                count += TestLine(Reverse(line), ref wordList, false); //reverse order //never test one char word
            }
            return count;
        }

        //rotates diagonals
        public static int TestDiagonals(char[,] puzzle, ref List<string> wordList, int x, int y, bool isRotated)
        {
            var count = 0;
            var lines = x + y - 1;          //total number of lines in each direction
            if (isRotated)
            {
                for (int k = -(y - 1); k < x; k++)
                {
                    var sb = new StringBuilder();
                    for (int j = 0; j <= y - 1; j++)
                    {
                        int i = k + j;
                        if (0 <= i && i < x && j < y)
                        {
                            var a = puzzle[i, j];
                            sb.Append(a);
                        }
                    }
                    var line = sb.ToString();
                    count += TestLine(line, ref wordList, false);            //never test one char word
                    count += TestLine(Reverse(line), ref wordList, false);
                }
            }
            else
            {
                for (int k = 0; k < lines; k++)
                {
                    var sb = new StringBuilder();
                    for (int j = 0; j <= k; j++)
                    {
                        int i = k - j;
                        if (i < x && j < y)
                        {
                            sb.Append(puzzle[i, j]);
                        }
                    }
                    var line = sb.ToString();
                    count += TestLine(line, ref wordList, false);            //never test one char word
                    count += TestLine(Reverse(line), ref wordList, false);
                }
            }
            return count;
        }

        //tests a given line and returns number of matches within one line
        public static int TestLine(string line, ref List<string> wordList, bool testOneCharWord)
        {
            var count = 0;
            var dictionaryList = DICTIONARY.ToList();
            foreach (var word in dictionaryList)
            {
                if (line.Contains(word))
                {
                    //one letter word is only tested in one direction and only horizontal lines
                    if (word.Length == 1 && !testOneCharWord)
                        return count;
                    count += Regex.Matches(line, word).Count;
                    wordList.Add(word);
                    Console.WriteLine(word);
                }
            }
            return count;
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}

