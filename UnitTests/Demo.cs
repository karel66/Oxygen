/* 
 * Oxygen Flow 
 * by karel66, 2023
 */
using System;

using Elements.Oxygen;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class Demo : Flow
    {
        const string SeleniumDriversDirectory = @"C:\Selenium";

        /// <summary>
        /// Run DuckDuckGo search. Chrome browser must be present and matching webdriver in {SeleniumDriversDirectory}
        /// </summary>
        [TestMethod]
        public void SearchTestChrome()
        {
            DuckDuckSearch(BrowserBrand.Chrome);
        }
        /// <summary>
        /// Run DuckDuckGo search. Edge browser must be present and matching webdriver in {SeleniumDriversDirectory}
        /// </summary>
        [TestMethod]
        public void SearchTestEdge()
        {
            DuckDuckSearch(BrowserBrand.Edge);
        }
       
        static void DuckDuckSearch(BrowserBrand browser)
        {
            const string url = "https://duckduckgo.com/";
            const string searchBox = "input[name=q]";

            var result =
                CreateContext(browser, new Uri(url), 1, true, SeleniumDriversDirectory)
                | Find(searchBox)
                | SetText("Oxygen")
                | PressEnter;

            Assert.IsFalse(result.HasProblem, result);
        }

        static FlowStep Refresh() =>
            (Context c) =>
            {
                c.Driver.Navigate().GoToUrl(c.Driver.Url);
                return c.EmptyContext();
            };
    }
}
