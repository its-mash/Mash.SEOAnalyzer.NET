using System.Collections.Generic;

namespace Mash.SEOAnalyzer.NET
{
    public class SeoAnalyzerResult
    {

        public int UniqueExternalLinksCount => this.ExternalLinksCount.Count;
        public Dictionary<string, int> ExternalLinksCount = new Dictionary<string, int>();

        public Dictionary<string, int> WordOccurrenceCounts = new Dictionary<string, int>();
    }
}