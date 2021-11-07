using System;
using System.Collections.Generic;
using System.Text;
using Mash.HelperMethods.NET.ExtensionMethods;
using Mash.SEOAnalyzer.NET.Models;

namespace Mash.SEOAnalyzer.NET
{
    public class SeoLinkAnalyzer : SeoAnalyzer, ILinkSeoAnalyzer
    {
        private int _flag = 0;
        private Uri _url;
        private Dictionary<string, string> _headers = new Dictionary<string, string>();


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
                return this;
            }
        }

        public ILinkSeoAnalyzer SetRequestHeaders(Dictionary<string, string> headers)
        {
            _headers = headers;
            return this;
        }


        public SeoLinkAnalyzerResult GetResult()
        {
            throw new NotImplementedException();
        }
    }
}
