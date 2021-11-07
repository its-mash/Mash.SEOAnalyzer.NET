using System;
using System.Collections.Generic;
using System.Text;

namespace Mash.SEOAnalyzer.NET
{
    public interface ITextSeoAnalyzer
    {
        ITextSeoAnalyzer SetCalculateWordOccurrences(bool flag);
        ITextSeoAnalyzer SetCountExternalLinks(bool flag);
        ITextSeoAnalyzer SetFilterStopWords(bool flag);
        SeoTextAnalyzerResult GetResult();

    }
}
