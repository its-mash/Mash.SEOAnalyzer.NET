using System;
//using OpenQA.Selenium.Chrome;
//using OpenQA.Selenium.Remote;

namespace Mash.SEOAnalyzer.NET.Services
{

    //public class WebSiteHtmlLoader : IDisposable
    //{
    //    private readonly ChromeDriver _remoteWebDriver;

    //    public WebSiteHtmlLoader(ChromeDriver remoteWebDriver)
    //    {
    //        _remoteWebDriver = remoteWebDriver ?? throw new ArgumentNullException(nameof(remoteWebDriver));
    //    }

    //    public string GetRenderedHtml(Uri webSiteUri)
    //    {
    //        if (webSiteUri == null) throw new ArgumentNullException(nameof(webSiteUri));
    //        _remoteWebDriver.Navigate().GoToUrl(webSiteUri);

    //        return _remoteWebDriver.PageSource;
    //    }

    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }

    //    private void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            _remoteWebDriver?.Quit();
    //        }
    //    }
    //}
}