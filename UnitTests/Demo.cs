using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Oxygen;

namespace UnitTests
{
    [TestClass]
    public class Demo : Flow
    {
        [TestMethod]
        public void SearchTestFF()
        {
            var result =
                 CreateContext(BrowserBrand.FireFox, new Uri("https://github.com/"))
                 | Find("input[type=text][name=q]")
                 | SetText("OxygenFlow")
                 | Click("ul[id=jump-to-results]")
                 ;

            Assert.IsFalse(result.HasProblem, result);
        }


        /// <summary>
        /// Run Google search. Chrome browser must be present and matching webdriver in C:\Selenium folder
        /// </summary>
        [TestMethod]
        public void SearchTestChrome()
        {
            const string googleUrl = "https://www.google.com/";
            const string googleSearchBox = "input[name=q]";
            const string googleSearchButton = "input[type=submit]";

            var result =
                CreateContext(BrowserBrand.Chrome, new Uri(googleUrl), 30, true, @"C:\Selenium")
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
            (context)
            | IfExists("iframe", _ => 
                _
                | SwitchToFrame("iframe")
                | Click("div#introAgreeButton")
                | SwitchToDefault);
    }
}
