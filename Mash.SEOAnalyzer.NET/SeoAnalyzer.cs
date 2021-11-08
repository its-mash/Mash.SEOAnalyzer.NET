using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mash.AhoCoraSick;
using Mash.HelperMethods.NET;
using Mash.HelperMethods.NET.ExtensionMethods;

namespace Mash.SEOAnalyzer.NET
{
    public class SeoAnalyzer
    {
        private int _flags = 0;
        protected readonly object Lock = new object();

        private bool _flagChangedAfterGeneratingLastResult = false;
        protected static AhoCorasickEnglishWordsSetSearch StopWordsAutomaton;
        protected string StringToSearchIn;
        private readonly char[] _invalidBeginningCharacters = new char[] { '-', '&', '\'' };
        private readonly char[] _invalidEndingCharacters = new char[] { '-', '&', '\'' };
        private SeoTextAnalyzerResult _result;

        static SeoAnalyzer()
        {
            StopWordsAutomaton = new AhoCorasickEnglishWordsSetSearch(Constants.StopWords, true);
        }

        internal SeoAnalyzer(string stringToSearchIn)
        {
            StringToSearchIn = stringToSearchIn;

        }
        protected bool CalculateWordOccurrences
        {
            get => _flags.GetBit(0);
            set
            {
                lock (Lock)
                {

                    if (value != _flags.GetBit(0))
                    {
                        _flagChangedAfterGeneratingLastResult = true;
                    }

                    _flags.SetBit(0, value);
                }
            }
        }

        protected bool CountExternalLinks
        {
            get => _flags.GetBit(1);
            set
            {
                lock (Lock)
                {

                    if (value != _flags.GetBit(1))
                    {
                        _flagChangedAfterGeneratingLastResult = true;
                    }
                    _flags.SetBit(1, value);
                }
            }
        }

        protected bool FilterStopWords
        {
            get => _flags.GetBit(2);
            set
            {
                lock (Lock)
                {

                    if (value != _flags.GetBit(2))
                    {
                        _flagChangedAfterGeneratingLastResult = true;
                    }
                    _flags.SetBit(2, value);
                    _flagChangedAfterGeneratingLastResult = true;
                }
            }
        }

        protected void SetReCalculateResult()
        {
            _flagChangedAfterGeneratingLastResult = true;

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

        //TODO: combine CalculateWordOccurrence and count externalLinks under same iteration of StringToSearch to optimize out Extra Iteration of third party code.
        //TODO: Add further caching to recalculate parts of the result only if the corresponding flag changed
        protected SeoTextAnalyzerResult GetResult()
        {

            if (_result == null || _flagChangedAfterGeneratingLastResult)
            {
                _result = new SeoTextAnalyzerResult();
                if (this.CalculateWordOccurrences)
                {
                    if (this.FilterStopWords)
                    {
                        StringBuilder possibleWord = new StringBuilder();
                        bool isNoneRepeatableLastChar = false;
                        bool stopWordMatchFailed = false;
                        int stopWordStateNo = 0;
                        for (int i = 0; i < StringToSearchIn.Length; i++)
                        {
                            bool isNoneRepeatableLastCharCopy = isNoneRepeatableLastChar;
                            char ch =(char) StringToSearchIn[i].ToLowerValueIfUpperCase();
                            if (ch.IsThisLowerCaseCharacterIsValidWordCharacter(out isNoneRepeatableLastChar) &&
                                (!isNoneRepeatableLastCharCopy || !isNoneRepeatableLastChar))
                            {
                                possibleWord.Append(ch);
                                if (!stopWordMatchFailed)
                                {
                                    bool isMatch = StopWordsAutomaton.GoToCharacter(ch, stopWordStateNo, out stopWordStateNo);
                                    if (stopWordStateNo == -1)
                                    {
                                        stopWordStateNo = 0;
                                        stopWordMatchFailed = true;
                                    }
                                    if (isMatch)
                                    {
                                        if (i + 1 == StringToSearchIn.Length ||
                                            !StringToSearchIn[i + 1].IsValidStopWordContinuingCharacter())
                                        {
                                            possibleWord.Clear();

                                        }
                                    }
                                }
                            }
                            else
                            {
                                stopWordMatchFailed = false;
                                stopWordStateNo = 0;
                                string key = null;
                                if (IsValidWordInCurrentContext(possibleWord.ToString(), ref key))
                                {
                                    key = SanitizeKey(key);
                                    _result.WordOccurrenceCounts.IncrementValueBy1(key);
                                }

                                possibleWord.Clear();

                            }

                        }
                    }
                    else
                    {
                        //Code repetition to avoid should filter stopWordCheck every time
                        StringBuilder possibleWord = new StringBuilder();
                        bool isNoneRepeatableCurrentChar = false;
                        for (int i = 0; i < StringToSearchIn.Length; i++)
                        {
                            bool isNoneRepeatableLastChar = isNoneRepeatableCurrentChar;
                            char ch = StringToSearchIn[i];
                            if (ch.IsValidWordCharacter(out isNoneRepeatableCurrentChar) &&
                                (!isNoneRepeatableLastChar || !isNoneRepeatableCurrentChar))
                            {
                                possibleWord.Append(ch);
                            }
                            else
                            {
                                string key = null;
                                if (IsValidWordInCurrentContext(possibleWord.ToString(), ref key))
                                {
                                    key = SanitizeKey(key);
                                    _result.WordOccurrenceCounts.IncrementValueBy1(key);
                                }

                                possibleWord.Clear();
                            }

                        }

                    }
                }

                if (this.CountExternalLinks)
                {

                    var linkParser = new Regex(RegexPattern.LinkPattern3);
                    var matchCollection = linkParser.Matches(StringToSearchIn);
                    foreach (Match match in matchCollection)
                    {
                        if (match.Value[match.Value.Length - 1] != '.')
                            _result.ExternalLinksFoundInTextCount.IncrementValueBy1(match.Value);
                    }


                }

                _flagChangedAfterGeneratingLastResult = false;
            }

            return _result;
        }
        internal string SanitizeKey(string key)
        {
            if (_invalidBeginningCharacters.Contains(key[0]))
                key = key.Substring(1);
            if (_invalidEndingCharacters.Contains(key[key.Length - 1]))
                key = key.Substring(0, key.Length - 1);
            return key;
        }

    }
}
