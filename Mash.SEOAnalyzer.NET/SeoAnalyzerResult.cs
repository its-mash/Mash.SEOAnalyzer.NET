using System.Collections.Generic;

namespace Mash.SEOAnalyzer.NET
{
    public class SeoAnalyzerResult
    {
        internal SeoAnalyzerResult()
        {

        }

        public SeoAnalyzerResult(SeoTextAnalyzerResult textAnalyzerResult)
        {
            this.WordOccurrenceCounts = textAnalyzerResult.WordOccurrenceCounts;
            this.ExternalLinksFoundInTextCount = textAnalyzerResult.ExternalLinksFoundInTextCount;
        }
        public int UniqueExternalLinksFoundInTextCount => this.ExternalLinksFoundInTextCount.Count;
        public Dictionary<string, int> ExternalLinksFoundInTextCount = new Dictionary<string, int>();

        public Dictionary<string, int> WordOccurrenceCounts = new Dictionary<string, int>();

        protected void Fill(SeoTextAnalyzerResult textAnalyzerResult)
        {
            this.WordOccurrenceCounts = textAnalyzerResult.WordOccurrenceCounts;
            this.ExternalLinksFoundInTextCount = textAnalyzerResult.ExternalLinksFoundInTextCount;
        }
    }
}