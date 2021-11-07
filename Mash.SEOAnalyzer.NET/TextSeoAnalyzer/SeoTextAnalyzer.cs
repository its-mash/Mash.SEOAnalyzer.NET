using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Mash.HelperMethods.NET;
using Mash.HelperMethods.NET.ExtensionMethods;

namespace Mash.SEOAnalyzer.NET
{
    public class SeoTextAnalyzer : SeoAnalyzer, ITextSeoAnalyzer
    {
        private SeoTextAnalyzerResult _result;

        public SeoTextAnalyzer(string stringToSearchIn) : base(stringToSearchIn)
        {
        }
        public ITextSeoAnalyzer SetCalculateWordOccurrences(bool flag)
        {
            base.CalculateWordOccurrences = flag;
            return this;
        }

        public ITextSeoAnalyzer SetCountExternalLinks(bool flag)
        {
            base.CountExternalLinks = flag;
            return this;
        }

        public ITextSeoAnalyzer SetFilterStopWords(bool flag)
        {
            base.FilterStopWords = flag;
            return this;
        }

        //TODO: combine CalculateWordOccurrence and count externalLinks under same iteration of StringToSearch to optimize out Extra Iteration of third party code.
        //TODO: Add further caching to recalculate parts of the result only if the corresponding flag changed
        public SeoTextAnalyzerResult GetResult()
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
                        for (int i = 0; i < _stringToSearchIn.Length; i++)
                        {
                            bool isNoneRepeatableLastCharCopy = isNoneRepeatableLastChar;
                            char ch = _stringToSearchIn[i];
                            if (ch.IsValidWordCharacter(out isNoneRepeatableLastChar) && (!isNoneRepeatableLastCharCopy || !isNoneRepeatableLastChar))
                            {
                                possibleWord.Append(ch);
                                if (!stopWordMatchFailed)
                                {
                                    bool isMatch = _stopWordsAutomaton.GoToCharacter(ch, out int newStateNo);
                                    stopWordMatchFailed = newStateNo == -1;
                                    if (isMatch)
                                    {
                                        if (i + 1 == _stringToSearchIn.Length ||
                                            !_stringToSearchIn[i + 1].IsValidStopWordContinuingCharacter())
                                        {
                                            possibleWord.Clear();

                                        }
                                    }
                                }
                            }
                            else
                            {
                                stopWordMatchFailed = false;
                                _stopWordsAutomaton.ResetSearchState();
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
                        for (int i = 0; i < _stringToSearchIn.Length; i++)
                        {
                            bool isNoneRepeatableLastChar = isNoneRepeatableCurrentChar;
                            char ch = _stringToSearchIn[i];
                            if (ch.IsValidWordCharacter(out isNoneRepeatableCurrentChar) && (!isNoneRepeatableLastChar || !isNoneRepeatableCurrentChar))
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
                    var matchCollection = linkParser.Matches(_stringToSearchIn);
                    foreach (Match match in matchCollection)
                    {
                        if (match.Value[match.Value.Length - 1] != '.')
                            _result.ExternalLinksCount.IncrementValueBy1(match.Value);
                    }


                }

                _flagChangedAfterGeneratingLastResult = false;
            }

            return _result;
        }

    }
}
