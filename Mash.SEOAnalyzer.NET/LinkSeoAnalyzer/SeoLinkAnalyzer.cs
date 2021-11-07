using System;
using System.Collections.Generic;
using System.Text;
using Mash.HelperMethods.NET.ExtensionMethods;
using Mash.SEOAnalyzer.NET.Models;

namespace Mash.SEOAnalyzer.NET
{
    public class SeoLinkAnalyzer : SeoAnalyzer, ILinkSeoAnalyzer
    {
        public SeoLinkAnalyzer(string stringToSearchIn) : base(stringToSearchIn)
        {

        }
        public ILinkSeoAnalyzer SetCalculateWordOccurrencesInPageText(bool flag)
        {
            int[] x = new int[24];
            throw new NotImplementedException();
        }

        public ILinkSeoAnalyzer SetCalculateMetaDataKeywordOccurrencesInPageText(bool flag)
        {
            throw new NotImplementedException();
        }

        public ILinkSeoAnalyzer SetCountExternalLinks(bool flag)
        {
            throw new NotImplementedException();
        }

        public ILinkSeoAnalyzer SetFilterStopWords(bool flag)
        {
            throw new NotImplementedException();
        }

        public ILinkSeoAnalyzer SetRequestHeaders(Dictionary<string, string> headers)
        {
            throw new NotImplementedException();
        }

        public ILinkSeoAnalyzer SetAnalyzeContentAfterRenderingReturnedHtmlAlso(bool flag)
        {
            throw new NotImplementedException();
        }

        public SeoLinkAnalyzerResult GetResult()
        {
            throw new NotImplementedException();
        }
    }
}
