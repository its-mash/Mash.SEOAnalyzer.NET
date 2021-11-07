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

        public new SeoTextAnalyzerResult GetResult()
        {
            lock (Lock)
            {
                var seoAnalyzerResult = base.GetResult();
                return seoAnalyzerResult;
            }
        }

    }
}
