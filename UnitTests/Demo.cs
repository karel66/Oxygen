using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Elements.Oxygen;

namespace UnitTests
{
    [TestClass]
    public class Demo : Flow
    {
        const string seleniumDriversDirectory = @"C:\Selenium";

        /// <summary>
        /// Run Google search. Chrome browser must be present and matching webdriver in C:\Selenium folder
        /// </summary>
        [TestMethod]
        public void SearchTestChrome()
        {
            GoogleSearch(BrowserBrand.Chrome);
        }

        [TestMethod]
        public void SearchTestEdge()
        {
            GoogleSearch(BrowserBrand.Edge);
        }

        static void GoogleSearch(BrowserBrand browser)
        {
            const string googleUrl = "https://www.google.com/";
            const string googleSearchBox = "input[name=q]";

            var result =
                CreateContext(browser, new Uri(googleUrl), 1, true, seleniumDriversDirectory)
                | AcceptGoogleTerms
                | Find(googleSearchBox)
                | SetText("Oxygen")
                | PressEnter;

            Assert.IsFalse(result.HasProblem, result);
        }

        /// <summary>
        /// Agree to Google terms if presented in iframe
        /// </summary>
        static Context AcceptGoogleTerms(Context context) =>
            context
            | IfExists("button#L2AGLb",
                _ => _
                | Click("button#L2AGLb")
                | Refresh()
              );

        static FlowStep Refresh() =>
            (Context c) =>
            {
                c.Driver.Navigate().GoToUrl(c.Driver.Url);
                return c.EmptyContext();
            };
    }
}
