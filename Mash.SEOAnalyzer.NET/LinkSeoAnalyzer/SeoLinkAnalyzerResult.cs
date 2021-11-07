using System;
using System.Collections.Generic;
using System.Text;

namespace Mash.SEOAnalyzer.NET.Models
{
    public class SeoLinkAnalyzerResult : SeoAnalyzerResult
    {
        internal SeoLinkAnalyzerResult()
        {

        }

        internal SeoLinkAnalyzerResult(SeoTextAnalyzerResult seoTextAnalyzer) : base(seoTextAnalyzer)
        {

        }
        public int UniqueExternalLinksFoundInHtmlCount => this.ExternalLinksFoundInHtmlCount.Count;
        public Dictionary<string, int> ExternalLinksFoundInHtmlCount = new Dictionary<string, int>();
        public Dictionary<string, int> MetaKeyWordOccurrencesInTextCounts = new Dictionary<string, int>();

        public new void Fill(SeoTextAnalyzerResult textAnalyzerResult)
        {
            base.Fill(textAnalyzerResult);
        }
    }
}
