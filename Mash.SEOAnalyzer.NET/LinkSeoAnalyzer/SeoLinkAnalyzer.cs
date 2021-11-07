using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
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
                    _metaKeywordAutomaton = new AhoCorasickEnglishWordsSetSearch(true);
                    HtmlDocument htmlDocument;
                    if (AnalyzeContentAfterRenderingReturnedHtmlAlso)
                    {
                        htmlDocument = ProcessRenderedHtml();
                    }
                    else
                    {
                        //TODO: restructure sync bit based locking to SemaphoreSlim to get the actual benefit of async method.
                        htmlDocument = ProcessNoneRenderedHtml().Result;

                    }

                    if (base.CalculateWordOccurrences && !CalculateMetaDataKeywordOccurrencesInPageText)
                    {
                        _result.Fill(base.GetResult());
                    }
                    else if (!base.CalculateWordOccurrences && CalculateMetaDataKeywordOccurrencesInPageText)
                    {
                        CalculateMetaDataKeywordOccurrenceInPageTextCount();
                    }
                    else
                    {
                        CalculatePageTextOccurrenceCountAndMetaDataKeywordOccurrenceCountInPageText();

                    }
                    if (CountExternalLinks)
                    {
                        _result.ExternalLinksFoundInHtmlCount = htmlDocument.GetExternalLinksCountFoundInHtml(this._url);

                    }


                }

                _flagChangedAfterGeneratingLastResult = false;

                return _result;
            }
        }

        private void CalculatePageTextOccurrenceCountAndMetaDataKeywordOccurrenceCountInPageText()
        {
            if (this.FilterStopWords)
            {
                StringBuilder possibleWord = new StringBuilder();
                bool isNoneRepeatableLastChar = false;
                bool stopWordMatchFailed = false;
                bool metaKeyWordMatchFailed = false;
                for (int i = 0; i < StringToSearchIn.Length; i++)
                {
                    bool isNoneRepeatableLastCharCopy = isNoneRepeatableLastChar;
                    char ch = StringToSearchIn[i];
                    if (ch.IsValidWordCharacter(out isNoneRepeatableLastChar) &&
                        (!isNoneRepeatableLastCharCopy || !isNoneRepeatableLastChar))
                    {
                        possibleWord.Append(ch);
                        bool stopWordMatch = stopWordMatchFailed;
                        if (!stopWordMatchFailed)
                        {
                            stopWordMatch = StopWordsAutomaton.GoToCharacter(ch, out int newStateNo);
                            stopWordMatchFailed = newStateNo == -1;
                            if (stopWordMatch)
                            {
                                if (i + 1 == StringToSearchIn.Length ||
                                    !StringToSearchIn[i + 1].IsValidStopWordContinuingCharacter())
                                {
                                    possibleWord.Clear();

                                }
                            }
                        }
                        if (!metaKeyWordMatchFailed)
                        {
                            bool isMatch = _metaKeywordAutomaton.GoToCharacter(ch, out int newStateNo);
                            metaKeyWordMatchFailed = newStateNo == -1;
                            if (isMatch && !stopWordMatch)
                            {
                                if (i + 1 == StringToSearchIn.Length ||
                                    !StringToSearchIn[i + 1].IsValidMetaWordContinuingCharacter())
                                {
                                    _result.MetaKeyWordOccurrencesInTextCounts.IncrementValueBy1(possibleWord.ToString());

                                }
                            }
                        }
                    }
                    else
                    {
                        stopWordMatchFailed = false;
                        StopWordsAutomaton.ResetSearchState();

                        metaKeyWordMatchFailed = false;
                        _metaKeywordAutomaton.ResetSearchState();

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
                bool metaKeyWordMatchFailed = false;
                for (int i = 0; i < StringToSearchIn.Length; i++)
                {
                    bool isNoneRepeatableLastChar = isNoneRepeatableCurrentChar;
                    char ch = StringToSearchIn[i];
                    if (ch.IsValidWordCharacter(out isNoneRepeatableCurrentChar) &&
                        (!isNoneRepeatableLastChar || !isNoneRepeatableCurrentChar))
                    {
                        possibleWord.Append(ch);
                        if (!metaKeyWordMatchFailed)
                        {
                            bool isMatch = _metaKeywordAutomaton.GoToCharacter(ch, out int newStateNo);
                            metaKeyWordMatchFailed = newStateNo == -1;
                            if (isMatch)
                            {
                                if (i + 1 == StringToSearchIn.Length ||
                                    !StringToSearchIn[i + 1].IsValidMetaWordContinuingCharacter())
                                {
                                    _result.MetaKeyWordOccurrencesInTextCounts.IncrementValueBy1(possibleWord.ToString());

                                }
                            }
                        }
                    }
                    else
                    {
                        metaKeyWordMatchFailed = false;
                        _metaKeywordAutomaton.ResetSearchState();

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

        private void CalculateMetaDataKeywordOccurrenceInPageTextCount()
        {
            if (this.FilterStopWords)
            {
                StringBuilder possibleWord = new StringBuilder();
                for (int i = 0; i < StringToSearchIn.Length; i++)
                {
                    char ch = StringToSearchIn[i];
                    possibleWord.Append(ch);
                    bool isMatchStopWord = StopWordsAutomaton.GoToCharacter(ch, out _);
                    bool isMatchMetaKeyword = _metaKeywordAutomaton.GoToCharacter(ch, out int nextMetaKeywordStateNo);
                    if (nextMetaKeywordStateNo == -1)
                    {
                        possibleWord.Clear();
                    }
                    else if (isMatchMetaKeyword && !isMatchStopWord)
                    {
                        if (i + 1 == StringToSearchIn.Length ||
                            !StringToSearchIn[i + 1].IsValidStopWordContinuingCharacter())
                        {
                            _result.MetaKeyWordOccurrencesInTextCounts.IncrementValueBy1(possibleWord.ToString());

                        }

                    }

                }
            }
            else
            {
                //Code repetition to avoid should filter flag checks every time
                StringBuilder possibleWord = new StringBuilder();
                for (int i = 0; i < StringToSearchIn.Length; i++)
                {
                    char ch = StringToSearchIn[i];
                    possibleWord.Append(ch);
                    bool isMatchMetaKeyword = _metaKeywordAutomaton.GoToCharacter(ch, out int nextMetaKeywordStateNo);
                    if (nextMetaKeywordStateNo == -1) possibleWord.Clear();
                    else if (isMatchMetaKeyword)
                    {
                        if (i + 1 == StringToSearchIn.Length ||
                            !StringToSearchIn[i + 1].IsValidStopWordContinuingCharacter())
                        {
                            _result.MetaKeyWordOccurrencesInTextCounts.IncrementValueBy1(possibleWord.ToString());

                        }

                    }

                }

            }


        }

        private async Task<HtmlDocument> GetRemoteHtml()
        {
            try
            {
                var webRequest = WebRequest.Create(_url);
                webRequest.Timeout = 10000;
                foreach (var header in _headers)
                {
                    webRequest.Headers.Add(header.Key, header.Value);
                }

                var resp = (HttpWebResponse)await webRequest.GetResponseAsync();

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

        private async Task<HtmlDocument> ProcessNoneRenderedHtml()
        {

            var htmlDocument = await GetRemoteHtml();
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
                        _result.MetaKeyWordOccurrencesInTextCounts.InitializeValueToZeroIfKeyDoesNotExist(metaKeyword);
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
