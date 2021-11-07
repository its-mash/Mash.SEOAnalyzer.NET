using System;

namespace Mash.SEOAnalyzer.NET.Exceptions
{
    public class HtmlContentRetrievingFailure : Exception
    {

        public HtmlContentRetrievingFailure(string message) : base(message)
        {
        }

        public HtmlContentRetrievingFailure(string message, Exception innerException) : base(message, innerException)
        {

        }

    }
}