using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mash.SEOAnalyzer.NET.Models;

namespace Mash.SEOAnalyzer.NET
{
    public interface ILinkSeoAnalyzer
    {

        ILinkSeoAnalyzer SetCalculateWordOccurrencesInPageText(bool flag);
        ILinkSeoAnalyzer SetCalculateMetaDataKeywordOccurrencesInPageText(bool flag);
        ILinkSeoAnalyzer SetCountExternalLinks(bool flag);
        ILinkSeoAnalyzer SetFilterStopWords(bool flag);
        ILinkSeoAnalyzer SetRequestHeaders(Dictionary<string, string> headers);
        ILinkSeoAnalyzer SetAnalyzeContentAfterRenderingReturnedHtmlAlso(bool flag);
        SeoLinkAnalyzerResult GetResult(bool reloadPage = false);
    }
}
