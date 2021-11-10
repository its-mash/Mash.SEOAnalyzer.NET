using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Mash.AhoCoraSick;
using Mash.HelperMethods.NET;
using Mash.HelperMethods.NET.ExtensionMethods;
using Mash.SEOAnalyzer.NET.Exceptions;
using Mash.SEOAnalyzer.NET.Models;
using Mash.SEOAnalyzer.NET.Services;

namespace Mash.SEOAnalyzer.NET
{
    public class SeoLinkAnalyzer : SeoAnalyzer, ILinkSeoAnalyzer
    {
        private int _flag = 0;
        private Uri _url;
        private SeoLinkAnalyzerResult _result;
        private Dictionary<string, string> _headers = new Dictionary<string, string>();
        private AhoCorasickEnglishWordsSetSearch _metaKeywordAutomaton = new AhoCorasickEnglishWordsSetSearch(true);
        private bool _flagChangedAfterGeneratingLastResult = false;
        private Regex _absoluteUrlWithOrWithoutSchemePattern = new Regex(RegexPattern.absoluteUrlCheckWithOrWithoutScheme);



        public SeoLinkAnalyzer(Uri url) : base(string.Empty)
        {
            if (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException("The url provided is not a valid webpage link");

            }
            _url = url;
        }
        public ILinkSeoAnalyzer SetCalculateWordOccurrencesInPageText(bool flag)
        {
            base.CalculateWordOccurrences = flag;
            return this;
        }

        public ILinkSeoAnalyzer SetCountExternalLinks(bool flag)
        {
            base.CountExternalLinks = flag;
            return this;
        }

        public ILinkSeoAnalyzer SetFilterStopWords(bool flag)
        {
            base.FilterStopWords = flag;
            return this;
        }
        private bool CalculateMetaDataKeywordOccurrencesInPageText => _flag.GetBit(0);
        public ILinkSeoAnalyzer SetCalculateMetaDataKeywordOccurrencesInPageText(bool flag)
        {
            lock (Lock)
            {

                _flagChangedAfterGeneratingLastResult = true;
                _flag.SetBit(0, flag);
                return this;
            }
        }

        private bool AnalyzeContentAfterRenderingReturnedHtmlAlso => _flag.GetBit(1);
        public ILinkSeoAnalyzer SetAnalyzeContentAfterRenderingReturnedHtmlAlso(bool flag)
        {
            lock (Lock)
            {

                _flag.SetBit(1, flag);
                _flagChangedAfterGeneratingLastResult = true;
                return this;
            }
        }

        public ILinkSeoAnalyzer SetRequestHeaders(Dictionary<string, string> headers)
        {
            lock (Lock)
            {

                _headers = headers;
                _flagChangedAfterGeneratingLastResult = true;
                return this;
            }
        }


        public SeoLinkAnalyzerResult GetResult(bool reloadPage = false)
        {
            lock (Lock)
            {
                if (_result == null || _flagChangedAfterGeneratingLastResult || reloadPage)
                {
                    _result = new SeoLinkAnalyzerResult();
                    base.SetReCalculateResult();
                    _metaKeywordAutomaton = new AhoCorasickEnglishWordsSetSearch(true, patternIsValidEnglishWord: true);
                    HtmlDocument htmlDocument;
                    if (AnalyzeContentAfterRenderingReturnedHtmlAlso)
                    {
                        htmlDocument = ProcessRenderedHtml();
                    }
                    else
                    {
                        //TODO: restructure sync bit based locking to SemaphoreSlim to get the actual benefit of async method.
                        htmlDocument = ProcessNoneRenderedHtml();

                    }

                    if (base.CalculateWordOccurrences && !CalculateMetaDataKeywordOccurrencesInPageText)
                    {
                        _result.Fill(base.GetResult());
                    }
                    else if (!base.CalculateWordOccurrences && CalculateMetaDataKeywordOccurrencesInPageText)
                    {
                        CalculateMetaDataKeywordOccurrenceInPageTextCount();
                    }
                    else if (this.CalculateWordOccurrences && this.CalculateMetaDataKeywordOccurrencesInPageText)
                    {
                        CalculatePageTextOccurrenceCountAndMetaDataKeywordOccurrenceCountInPageText();

                    }
                    if (CountExternalLinks)
                    {
                        var linkParser = new Regex(RegexPattern.LinkPattern3);
                        var matchCollection = linkParser.Matches(StringToSearchIn);
                        foreach (Match match in matchCollection)
                        {
                            string possibleAbsoluteUrl = match.Value;
                            if (!_absoluteUrlWithOrWithoutSchemePattern.Match(match.Value).Success)
                            {
                                possibleAbsoluteUrl = "http://" + possibleAbsoluteUrl;
                            }
                            if (Uri.TryCreate(possibleAbsoluteUrl, UriKind.Absolute, out Uri tempUri))
                            {
                                if (this._url.Authority != tempUri.Authority)
                                    _result.ExternalLinksFoundInTextCount.IncrementValueBy1(match.Value);
                            }
                        }
                        _result.ExternalLinksFoundInHtmlCount = htmlDocument.GetExternalLinksCountFoundInHtml(this._url);

                    }


                }

                _flagChangedAfterGeneratingLastResult = false;

                return _result;
            }
        }

        private void CalculatePageTextOccurrenceCountAndMetaDataKeywordOccurrenceCountInPageText()
        {
            StringBuilder possibleWord = new StringBuilder();
            bool isNoneRepeatableLastChar = false;
            bool metaKeyWordMatchFailed = false;
            int metaKeywordCurrentStateNo = 0;
            if (this.FilterStopWords)
            {
                bool stopWordMatchFailed = false;
                int stopWordCurrentStateNo = 0;
                for (int i = 0; i < StringToSearchIn.Length; i++)
                {
                    bool isNoneRepeatableLastCharCopy = isNoneRepeatableLastChar;
                    char ch = (char)StringToSearchIn[i].ToLowerValueIfUpperCase();
                    if (ch.IsThisLowerCaseCharacterIsValidWordCharacter(out isNoneRepeatableLastChar) &&
                        (!isNoneRepeatableLastCharCopy || !isNoneRepeatableLastChar))
                    {
                        possibleWord.Append(ch);
                        bool isStopWordMatch = false;
                        if (!stopWordMatchFailed)
                        {
                            isStopWordMatch = StopWordsAutomaton.GoToCharacter(ch, stopWordCurrentStateNo, out stopWordCurrentStateNo);
                            if (stopWordCurrentStateNo == -1)
                            {
                                stopWordMatchFailed = true;
                                stopWordCurrentStateNo = 0;

                            }
                            if (isStopWordMatch)
                            {
                                if (!IsValidMetaKeywordMatch(possibleWord.Length, i))
                                {
                                    isStopWordMatch = false;
                                }
                            }
                        }
                        if (!metaKeyWordMatchFailed)
                        {
                            bool isMatch = _metaKeywordAutomaton.GoToCharacter(ch, metaKeywordCurrentStateNo, out metaKeywordCurrentStateNo);
                            if (metaKeywordCurrentStateNo == -1)
                            {
                                metaKeywordCurrentStateNo = 0;
                                metaKeyWordMatchFailed = true;
                            }

                            if (metaKeywordCurrentStateNo == 0) stopWordCurrentStateNo = 0;
                            if (isMatch)
                            {
                                if (IsValidMetaKeywordMatch(possibleWord.Length, i))
                                {
                                    if (!isStopWordMatch)
                                        _result.MetaKeyWordOccurrencesInTextCounts.IncrementValueBy1(possibleWord.ToString());
                                    else
                                    {
                                        _result.MetaKeyWordOccurrencesInTextCounts.Remove(possibleWord.ToString());
                                    }
                                    stopWordCurrentStateNo = 0;
                                    stopWordMatchFailed = false;
                                    metaKeywordCurrentStateNo = 0;

                                }
                            }
                        }

                        if (isStopWordMatch) possibleWord.Clear();
                    }
                    else
                    {
                        stopWordMatchFailed = false;
                        stopWordCurrentStateNo = 0;

                        metaKeyWordMatchFailed = false;
                        metaKeywordCurrentStateNo = 0;

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
                for (int i = 0; i < StringToSearchIn.Length; i++)
                {
                    bool isNoneRepeatableLastCharCopy = isNoneRepeatableLastChar;
                    char ch = (char)StringToSearchIn[i].ToLowerValueIfUpperCase();
                    if (ch.IsThisLowerCaseCharacterIsValidWordCharacter(out isNoneRepeatableLastChar) &&
                        (!isNoneRepeatableLastCharCopy || !isNoneRepeatableLastChar))
                    {
                        possibleWord.Append(ch);
                        if (!metaKeyWordMatchFailed)
                        {
                            bool isMatch = _metaKeywordAutomaton.GoToCharacter(ch, metaKeywordCurrentStateNo, out metaKeywordCurrentStateNo);
                            if (metaKeywordCurrentStateNo == -1)
                            {
                                metaKeywordCurrentStateNo = 0;
                                metaKeyWordMatchFailed = true;
                            }
                            if (isMatch)
                            {
                                if (IsValidMetaKeywordMatch(possibleWord.Length, i))
                                {
                                    _result.MetaKeyWordOccurrencesInTextCounts.IncrementValueBy1(possibleWord.ToString());

                                }
                            }
                        }
                    }
                    else
                    {
                        metaKeyWordMatchFailed = false;
                        metaKeywordCurrentStateNo = 0;

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

        private bool IsValidMetaKeywordMatch(int possibleWordLength, int i)
        {
            return possibleWordLength != 0 && (i + 1 == StringToSearchIn.Length ||
                                               !StringToSearchIn[i + 1].IsValidMetaWordContinuingCharacter());
        }

        private void CalculateMetaDataKeywordOccurrenceInPageTextCount()
        {
            if (this.FilterStopWords)
            {
                StringBuilder possibleWord = new StringBuilder();
                int stopWordCurrentStateNo = 0;
                int metaKeywordCurrentStateNo = 0;
                bool stopWordMatchFailed = false;
                for (int i = 0; i < StringToSearchIn.Length; i++)
                {
                    char ch = (char)StringToSearchIn[i].ToLowerValueIfUpperCase();
                    possibleWord.Append(ch);

                    bool isStopWordMatch = false;
                    if (!stopWordMatchFailed)
                    {
                        isStopWordMatch =
                            StopWordsAutomaton.GoToCharacter(ch, stopWordCurrentStateNo, out stopWordCurrentStateNo);
                        if (stopWordCurrentStateNo == -1)
                        {
                            stopWordMatchFailed = true;
                            stopWordCurrentStateNo = 0;
                        }

                        if (isStopWordMatch)
                        {
                            if (!IsValidMetaKeywordMatch(possibleWord.Length, i))
                            {
                                isStopWordMatch = false;
                            }
                        }
                    }

                    bool isMatchMetaKeyword = _metaKeywordAutomaton.GoToCharacter(ch, metaKeywordCurrentStateNo, out metaKeywordCurrentStateNo);
                    if (metaKeywordCurrentStateNo == -1)
                    {
                        metaKeywordCurrentStateNo = 0;
                        stopWordMatchFailed = false;
                        possibleWord.Clear();
                    }

                    if (metaKeywordCurrentStateNo == 0) stopWordCurrentStateNo = 0;

                    if (isMatchMetaKeyword)
                    {
                        if (IsValidMetaKeywordMatch(possibleWord.Length, i))
                        {
                            if (!isStopWordMatch)
                                _result.MetaKeyWordOccurrencesInTextCounts.IncrementValueBy1(possibleWord.ToString());
                            else
                            {
                                _result.MetaKeyWordOccurrencesInTextCounts.Remove(possibleWord.ToString());
                            }
                            stopWordCurrentStateNo = 0;
                            stopWordMatchFailed = false;
                            metaKeywordCurrentStateNo = 0;
                            possibleWord.Clear();

                        }


                    }


                }
            }
            else
            {
                //Code repetition to avoid should filter flag checks every time
                int metaKeywordCurrentStateNo = 0;
                StringBuilder possibleWord = new StringBuilder();
                for (int i = 0; i < StringToSearchIn.Length; i++)
                {
                    char ch = (char)StringToSearchIn[i].ToLowerValueIfUpperCase();
                    possibleWord.Append(ch);
                    bool isMatchMetaKeyword = _metaKeywordAutomaton.GoToCharacter(ch, metaKeywordCurrentStateNo, out metaKeywordCurrentStateNo);
                    if (metaKeywordCurrentStateNo == -1)
                    {
                        metaKeywordCurrentStateNo = 0;
                        possibleWord.Clear();
                    }
                    else if (isMatchMetaKeyword)
                    {
                        if (IsValidMetaKeywordMatch(possibleWord.Length, i))
                        {
                            _result.MetaKeyWordOccurrencesInTextCounts.IncrementValueBy1(possibleWord.ToString());

                        }

                    }

                }

            }


        }

        private HtmlDocument GetRemoteHtml()
        {
            try
            {
                var webRequest = WebRequest.Create(_url);
                webRequest.Timeout = 10000;
                if (_headers != null)
                {
                    foreach (var header in _headers)
                    {
                        webRequest.Headers.Add(header.Key, header.Value);
                    }
                }

                var resp = (HttpWebResponse)webRequest.GetResponse();

                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    {
                        var htmlTextSb = new StringBuilder();
                        bool lastCharWhiteSpace = false;
                        while (!stream.EndOfStream)
                        {
                            char ch = (char)stream.Read();
                            bool isWhiteSpace = ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
                            if (!isWhiteSpace || !lastCharWhiteSpace)
                            {
                                htmlTextSb.Append(ch);
                            }

                            lastCharWhiteSpace = isWhiteSpace;

                        }
                        var htmlText = htmlTextSb.ToString();
                        var document = new HtmlDocument();
                        document.LoadHtml(htmlText);
                        if (document.IsValid())
                        {
                            resp.Close();
                            return document;
                        }
                        else
                        {
                            resp.Close();
                            throw new HtmlContentRetrievingFailure(
                                "Failed to retrieve web page content. Server returned invalid Html");


                        }
                    }
                }
                else
                {
                    string respStatusDescription = resp.StatusDescription;
                    resp.Close();
                    throw new HtmlContentRetrievingFailure(
                        "Failed to retrieve web page content. Server Status Message: ");
                }
            }
            catch (HtmlContentRetrievingFailure)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HtmlContentRetrievingFailure("Failed to retrieve web page content. Error Message: " + ex.Message, ex.InnerException);
            }


        }

        private HtmlDocument ProcessNoneRenderedHtml()
        {

            var htmlDocument = GetRemoteHtml();
            htmlDocument.SanitizeContent();
            base.StringToSearchIn = htmlDocument.GetBodyText();

            if (CalculateMetaDataKeywordOccurrencesInPageText)
            {
                var metaKeywords = htmlDocument.GetMetaKeywords();
                foreach (var metaKeyword in metaKeywords)
                {
                    try
                    {

                        _metaKeywordAutomaton.AddEnglishWord(metaKeyword);
                        _result.MetaKeyWordOccurrencesInTextCounts.InitializeValueToZeroIfKeyDoesNotExist(metaKeyword.ToLower());
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                }


            }

            return htmlDocument;


        }

        private HtmlDocument ProcessRenderedHtml()
        {
            throw new NotImplementedException();
            //using (var htmlloader = new WebSiteHtmlLoader(new ChromeDriver("/")))
            //{
            //    var html = htmlloader.GetRenderedHtml(_url);
            //}

        }

    }
}
