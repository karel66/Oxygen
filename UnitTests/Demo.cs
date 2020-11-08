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

        [TestMethod]
        public void SearchTestChrome()
        {
            var result =
                CreateContext(BrowserBrand.Chrome, new Uri("https://www.google.com/"), true, @"C:\Selenium")
                | Use((Context context) => context.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30))
                // Agree to Google terms if presented
                | IfExists("iframe",
                    (Context context) => context | Find("iframe") | SwitchToFrame | Find("div#introAgreeButton") | Click)
                //
                | SetText("input[name=q]", "OxygenFlow")
                | Click("input[type=submit]")
                ;


            Assert.IsFalse(result.HasProblem, result);
        }
    }
}
