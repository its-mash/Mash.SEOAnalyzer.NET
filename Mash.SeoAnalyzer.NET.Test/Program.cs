using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mash.HelperMethods.NET;
using Mash.HelperMethods.NET.ExtensionMethods;
using Mash.SEOAnalyzer.NET;

namespace Mash.SeoAnalyzer.NET.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var rpattern = new Regex(RegexPattern.absoluteUrlCheckWithOrWithoutScheme);
            string url = "ftp://www.stackoverflow.com";
            Console.WriteLine(rpattern.Match(url).Success);
            Console.ReadKey();
            return;

            ILinkSeoAnalyzer x =
                new SeoLinkAnalyzer(new Uri(
                        "https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-5.0"))
                    .SetAnalyzeContentAfterRenderingReturnedHtmlAlso(false).SetCountExternalLinks(true)
                    .SetCalculateMetaDataKeywordOccurrencesInPageText(true).SetCalculateWordOccurrencesInPageText(true)
                    .SetFilterStopWords(true);
            var res = x.GetResult();


            Console.WriteLine("========================Word=======================");
            Console.WriteLine(res.WordOccurrenceCounts.Count);
            foreach (var resWordOccurrenceCount in res.WordOccurrenceCounts)
            {
                Console.WriteLine(resWordOccurrenceCount.Key + "    ->   " + resWordOccurrenceCount.Value.ToString());
            }
            Console.WriteLine();
            Console.WriteLine("=====================Meta==========================");

            Console.WriteLine(res.MetaKeyWordOccurrencesInTextCounts.Count);
            foreach (var resWordOccurrenceCount in res.MetaKeyWordOccurrencesInTextCounts)
            {
                Console.WriteLine(resWordOccurrenceCount.Key + "    ->   " + resWordOccurrenceCount.Value.ToString());
            }
            Console.WriteLine();
            Console.WriteLine("======================LinkInText=========================");

            Console.WriteLine(res.UniqueExternalLinksFoundInTextCount);
            foreach (var externalLinkCount in res.ExternalLinksFoundInTextCount)
            {
                Console.WriteLine(externalLinkCount.Key + "    ->   " + externalLinkCount.Value.ToString());
            }
            Console.WriteLine();
            Console.WriteLine("=======================LinkInHtml========================");

            Console.WriteLine(res.UniqueExternalLinksFoundInHtmlCount);
            foreach (var externalLinkCount in res.ExternalLinksFoundInHtmlCount)
            {
                Console.WriteLine(externalLinkCount.Key + "    ->   " + externalLinkCount.Value.ToString());
            }
            Console.WriteLine();
            Console.WriteLine("===============================================");





            Console.ReadKey();
            return;










            string text = @"Skip to main content
Search

Sign in
Docs .NET ASP.NET Core Overview About ASP.NET Core
Version
Search
Filter by title
ASP.NET Core documentation
What's new in ASP.NET Core docs
About ASP.NET Core
Compare ASP.NET Core and ASP.NET
Compare .NET and .NET Framework
Get started
API reference
Contribute
Introduction to ASP.NET Core
09/22/2021
9 minutes to read






+3
By Daniel Roth, Rick Anderson, and Shaun Luttin

ASP.NET Core is a cross-platform, high-performance, open-source framework for building modern, cloud-enabled, Internet-connected apps. With ASP.NET Core, you can:

Build web apps and services, Internet of Things (IoT) apps, and mobile backends.
Use your favorite development tools on Windows, macOS, and Linux.
Deploy to the cloud or on-premises.
Run on .NET Core.
Why choose ASP.NET Core?
Millions of developers use or have used ASP.NET 4.x to create web apps. ASP.NET Core is a redesign of ASP.NET 4.x, including architectural changes that result in a leaner, more modular framework.

ASP.NET Core provides the following benefits:

A unified story for building web UI and web APIs.
Architected for testability.
Razor Pages makes coding page-focused scenarios easier and more productive.
Blazor lets you use C# in the browser alongside JavaScript. Share server-side and client-side app logic all written with .NET.
Ability to develop and run on Windows, macOS, and Linux.
Open-source and community-focused.
Integration of modern, client-side frameworks and development workflows.
Support for hosting Remote Procedure Call (RPC) services using gRPC.
A cloud-ready, environment-based configuration system.
Built-in dependency injection.
A lightweight, high-performance, and modular HTTP request pipeline.
Ability to host on the following:
Kestrel
IIS
HTTP.sys
Nginx
Apache
Docker
Side-by-side versioning.
Tooling that simplifies modern web development.
Build web APIs and web UI using ASP.NET Core MVC
ASP.NET Core MVC provides features to build web APIs and web apps:

The Model-View-Controller (MVC) pattern helps make your web APIs and web apps testable.
Razor Pages is a page-based programming model that makes building web UI easier and more productive.
Razor markup provides a productive syntax for Razor Pages and MVC views.
Tag Helpers enable server-side code to participate in creating and rendering HTML elements in Razor files.
Built-in support for multiple data formats and content negotiation lets your web APIs reach a broad range of clients, including browsers and mobile devices.
Model binding automatically maps data from HTTP requests to action method parameters.
Model validation automatically performs client-side and server-side validation.
Client-side development
ASP.NET Core integrates seamlessly with popular client-side frameworks and libraries, including Blazor, Angular, React, and Bootstrap. For more information, see Introduction to ASP.NET Core Blazor and related topics under Client-side development.


ASP.NET Core target frameworks
ASP.NET Core 3.x and later can only target .NET Core. Generally, ASP.NET Core is composed of .NET Standard libraries. Libraries written with .NET Standard 2.0 run on any .NET platform that implements .NET Standard 2.0.

There are several advantages to targeting .NET Core, and these advantages increase with each release. Some advantages of .NET Core over .NET Framework include:

Cross-platform. Runs on Windows, macOS, and Linux.
Improved performance
Side-by-side versioning
New APIs
Open source
Recommended learning path
We recommend the following sequence of tutorials for an introduction to developing ASP.NET Core apps:

Follow a tutorial for the app type you want to develop or maintain.

TABLE 1
App type	Scenario	Tutorial
Web app	New server-side web UI development	Get started with Razor Pages
Web app	Maintaining an MVC app	Get started with MVC
Web app	Client-side web UI development	Get started with Blazor
Web API	RESTful HTTP services	Create a web API†
Remote Procedure Call app	Contract-first services using Protocol Buffers	Get started with a gRPC service
Real-time app	Bidirectional communication between servers and connected clients	Get started with SignalR
Follow a tutorial that shows how to do basic data access.

TABLE 2
Scenario	Tutorial
New development	Razor Pages with Entity Framework Core
Maintaining an MVC app	MVC with Entity Framework Core
Read an overview of ASP.NET Core fundamentals that apply to all app types.

Browse the table of contents for other topics of interest.

†There's also an interactive web API tutorial. No local installation of development tools is required. The code runs in an Azure Cloud Shell in your browser, and curl is used for testing.

Migrate from .NET Framework
For a reference guide to migrating ASP.NET 4.x apps to ASP.NET Core, see Migrate from ASP.NET to ASP.NET Core.

How to download a sample
Many of the articles and tutorials include links to sample code.

Download the ASP.NET repository zip file.
Unzip the AspNetCore.Docs-main.zip file.
To access an article's sample app in the unzipped repository, use the URL in the article's sample link to help you navigate to the sample's folder. Usually, an article's sample link appears at the top of the article with the link text View or download sample code.
Preprocessor directives in sample code
To demonstrate multiple scenarios, sample apps use the #define and #if-#else/#elif-#endif preprocessor directives to selectively compile and run different sections of sample code. For those samples that make use of this approach, set the #define directive at the top of the C# files to define the symbol associated with the scenario that you want to run. Some samples require defining the symbol at the top of multiple files in order to run a scenario.

For example, the following #define symbol list indicates that four scenarios are available (one scenario per symbol). The current sample configuration runs the TemplateCode scenario:

C#

Copy
#define TemplateCode // or LogFromMain or ExpandDefault or FilterInCode
To change the sample to run the ExpandDefault scenario, define the ExpandDefault symbol and leave the remaining symbols commented-out:

C#

Copy
#define ExpandDefault // TemplateCode or LogFromMain or FilterInCode
For more information on using C# preprocessor directives to selectively compile sections of code, see #define (C# Reference) and #if (C# Reference).

Regions in sample code
Some sample apps contain sections of code surrounded by #region and #endregion C# directives. The documentation build system injects these regions into the rendered documentation topics.

Region names usually contain the word snippet. The following example shows a region named snippet_WebHostDefaults:

C#

Copy
#region snippet_WebHostDefaults
            Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    });
            #endregion
            The preceding C# code snippet is referenced in the topic's markdown file with the following line:

Markdown

Copy
[!code - csharp[](sample / SampleApp / Program.cs ? name = snippet_WebHostDefaults)]
You may safely ignore(or remove) the
            #region and #endregion directives that surround the code. Don't alter the code within these directives if you plan to run the sample scenarios described in the topic. Feel free to alter the code when experimenting with other scenarios.

For more information, see Contribute to the ASP.NET documentation: Code snippets.

Breaking changes and security advisories
Breaking changes and security advisories are reported on the Announcements repo. Announcements can be limited to a specific version by selecting a Label filter.

Next steps
For more information, see the following resources:

Get started with ASP.NET Core
Publish an ASP.NET Core app to Azure with Visual Studio
ASP.NET Core fundamentals
The weekly ASP.NET community standup covers the team's progress and plans. It features new blogs and third-party software.
Recommended content
Get started with ASP.NET Core
A short tutorial that creates and runs a basic Hello World app using ASP.NET Core.

Overview of ASP.NET Core MVC
Learn how ASP.NET Core MVC is a rich framework for building web apps and APIs using the Model-View - Controller design pattern.

dotnet - aspnet - codegenerator command
    The dotnet - aspnet - codegenerator command scaffolds ASP.NET Core projects.

        What's new in ASP.NET Core 3.1
Learn about the new features in ASP.NET Core 3.1.

Feedback
Submit and view feedback for

house home go www.monstermmorpg.com monstermmorpg.com xx   www.monstermmorpg.com xdf file://sdsd.com/dfdf  file://localhosd/sdfd dddd  ftp://monstermmorpg.com dd nice hospital http://www.monstermmorpg.com  10.15.155.144 http://10.15.155.144 this is incorrect url https://www.monstermmorpg.commerged continue

 View all page feedback
Is this page helpful ?
 Yes  No
In this article
Why choose ASP.NET Core ?
Build web APIs and web UI using ASP.NET Core MVC
Client - side development
ASP.NET Core target frameworks
Recommended learning path
Migrate from.NET Framework
How to download a sample
Breaking changes and security advisories
Next steps
Previous Version Docs
Blog
Contribute
Privacy & Cookies
Terms of Use http://xxdfd.com
Trademarks
© Microsoft 2021";

            Console.WriteLine();
            Console.WriteLine("====================Word In Text=========================");
            ITextSeoAnalyzer y = new SeoTextAnalyzer(text).SetCountExternalLinks(true).SetFilterStopWords(true).SetCalculateWordOccurrences(true);
            var textAnalyzerResult = y.GetResult();
            Console.WriteLine(textAnalyzerResult.WordOccurrenceCounts.Count);
            foreach (var resWordOccurrenceCount in textAnalyzerResult.WordOccurrenceCounts)
            {
                Console.WriteLine(resWordOccurrenceCount.Key + "    ->   " + resWordOccurrenceCount.Value.ToString());
            }
            Console.WriteLine();
            Console.WriteLine("====================Link In Text=========================");

            Console.WriteLine(textAnalyzerResult.UniqueExternalLinksFoundInTextCount);
            foreach (var externalLinkCount in textAnalyzerResult.ExternalLinksFoundInTextCount)
            {
                Console.WriteLine(externalLinkCount.Key + "    ->   " + externalLinkCount.Value.ToString());
            }


            //var linkParser = new Regex(RegexPattern.LinkPattern1, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //var rawString = "house home go www.monstermmorpg.com monstermmorpg.com xx   www.monstermmorpg.com xdf file://sdsd.com/dfdf  file://localhosd/sdfd dddd  ftp://monstermmorpg.com dd nice hospital http://www.monstermmorpg.com  10.15.155.144 http://10.15.155.144 this is incorrect url https://www.monstermmorpg.commerged continue";
            //var col = linkParser.Matches(rawString);

            //Console.WriteLine(col.Count);
            //foreach (Match m in col)
            //    Console.WriteLine(m.Value);
            //Console.WriteLine(col.Count);
            //Console.WriteLine("-----------------------------");


            //linkParser = new Regex(RegexPattern.LinkPattern2, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //col = linkParser.Matches(rawString);
            //Console.WriteLine(col.Count);
            //foreach (Match m in col)
            //    Console.WriteLine(m.Value);
            //Console.WriteLine(col.Count);
            //Console.WriteLine("-----------------------------");

            //linkParser = new Regex(RegexPattern.LinkPattern3, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //col = linkParser.Matches(rawString);
            //Console.WriteLine(col.Count);
            //foreach (Match m in col)
            //    Console.WriteLine(m.Value);
            //Console.WriteLine(col.Count);
            //Console.WriteLine("-----------------------------");

            Console.ReadKey();
        }
    }
}
