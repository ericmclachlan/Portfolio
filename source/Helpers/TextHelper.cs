using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ericmclachlan.Portfolio
{
    public static class TextHelper
    {
        // Constants
        public const string BOS = "<s>";
        public const string EOS = "</s>";

        private static readonly Dictionary<char, char> _letters;

        // Construction

        static TextHelper()
        {
            _letters = new Dictionary<char, char>(26 * 2);
            // Add the lowercase letters to the letters collection.
            for (char i = 'a'; i <= 'z'; i++)
            {
                _letters[i] = i;
            }
            // Add the uppecase letters to the letters collection.
            for (char i = 'A'; i <= 'Z'; i++)
            {
                _letters[i] = i;
            }
        }

        // Public Methods

        public static string[] SplitOnNewline(string text)
        {
            return text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitOnWhitespace(string text)
        {
            return text.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string ToString(IList<string> words)
        {
            return ToString(words, words.Count);
        }

        public static string ToString(IList<string> words, int noOfWords)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            for (int i = 0; i < noOfWords; i++)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(" ");
                sb.Append(words[i]);
            }
            return sb.ToString();
        }


        public static string ToString(TimeSpan timeSpan)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        public static string ToSetNotation(ICollection collection)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            bool isFirst = true;
            foreach (var item in collection)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(",");
                sb.Append(item.ToString());
            }
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>Returns true of the specified letter <c>c</c> is either an uppercase or lowercase a letter in the alphabet.</summary>
        public static bool IsLetter(char c)
        {
            return _letters.ContainsKey(c);
        }

        /// <summary>Returns a string that is the concatenation of each of the strings in <c>outputSymbols</c>.</summary>
        public static string ReassembleOutput(string[] outputSymbols)
        {
            string output;
            var sb = new StringBuilder();
            foreach (var symbol in outputSymbols)
            {
                sb.Append(symbol.Trim('"'));
            }
            output = sb.ToString();
            return output;
        }

        /// <summary>Returns a collection of strings; where each letter in <c>input</c> is returned as a string in the collection.</summary>
        public static string[] DisassembleInput(string input)
        {
            var inputSymbols = new List<string>();
            foreach (char c in input)
            {
                inputSymbols.Add(c.ToString());
            }

            return inputSymbols.ToArray();
        }

        private static Regex _matcher = new Regex(@"\s*(\S*[^\\])/(\S+)\s*");
        public static void BreakPairIntoWordTag(string wordTagPairAsText, out string word, out string tag)
        {
            // Break the word/tag token into a word and a tag.
            // Be cautious of offset '/' characters that form part of the word, which appear as "\/". e.g. "AC\/DC" 
            Match match = _matcher.Match(wordTagPairAsText);
            word = match.Groups[1].Value.Replace(@"\/", "/");
            tag = match.Groups[2].Value;
        }
    }
}
