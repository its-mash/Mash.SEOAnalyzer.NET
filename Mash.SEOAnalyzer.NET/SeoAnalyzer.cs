using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mash.AhoCoraSick;
using Mash.HelperMethods.NET.ExtensionMethods;

namespace Mash.SEOAnalyzer.NET
{
    public class SeoAnalyzer
    {
        private int _flags = 0;

        protected bool _flagChangedAfterGeneratingLastResult = false;
        protected static AhoCorasickEnglishWordsSetSearch _stopWordsAutomaton;
        protected readonly string _stringToSearchIn;
        private readonly char[] _invalidBeginingCharacters = new char[] { '-', '&', '\'' };
        private readonly char[] _invalidEndingCharacters = new char[] { '-', '&', '\'' };

        protected bool CalculateWordOccurrences
        {
            get => _flags.GetBit(0);
            set
            {
                if (value != _flags.GetBit(0))
                {
                    _flagChangedAfterGeneratingLastResult = true;
                }
                _flags.SetBit(0, value);
            }
        }

        protected bool CountExternalLinks
        {
            get => _flags.GetBit(1);
            set
            {
                if (value != _flags.GetBit(1))
                {
                    _flagChangedAfterGeneratingLastResult = true;
                }
                _flags.SetBit(1, value);
            }
        }

        protected bool FilterStopWords
        {
            get => _flags.GetBit(2);
            set
            {
                if (value != _flags.GetBit(2))
                {
                    _flagChangedAfterGeneratingLastResult = true;
                }
                _flags.SetBit(2, value);
                _flagChangedAfterGeneratingLastResult = true;
            }
        }

        static SeoAnalyzer()
        {
            _stopWordsAutomaton = new AhoCorasickEnglishWordsSetSearch(Constants.StopWords, true);
        }

        internal SeoAnalyzer(string stringToSearchIn)
        {
            _stringToSearchIn = stringToSearchIn;

        }
        protected bool IsValidWordInCurrentContext(string toString, ref string s)
        {
            if (toString.Length < 2) return false;
            s = toString;
            foreach (var ch in toString)
            {
                //contains at least one alphabet, can consider as valid word
                if ((ch >= 'A' && ch < 'Z') || (ch >= 'a' && ch <= 'z')) return true;
            }
            //contains only numbers and special characters;
            return false;
        }
        internal string SanitizeKey(string key)
        {
            if (_invalidBeginingCharacters.Contains(key[0]))
                key = key.Substring(1);
            if (_invalidEndingCharacters.Contains(key[key.Length - 1]))
                key = key.Substring(0, key.Length - 1);
            return key;
        }

    }
}
