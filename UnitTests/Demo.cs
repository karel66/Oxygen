using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Oxygen;

namespace UnitTests
{
    [TestClass]
    public class Demo : Flow
    {
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

        const string googleUrl = "https://www.google.com/";

        static void GoogleSearch(BrowserBrand browser)
        {

            const string googleSearchBox = "input[name=q]";
            const string googleSearchButton = "input[name=btnK]";

            var result =
                CreateContext(browser, new Uri(googleUrl), 1, true, @"C:\Selenium")
                | AcceptGoogleTerms
                | Find(googleSearchBox)
                | SetText("OxygenFlow")
                | Click(googleSearchButton);

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
                return c;
            };
    }
}
